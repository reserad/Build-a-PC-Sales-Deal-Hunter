using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Quartz;
using System.Net;
using Build_a_PC_Sales_Deal_Hunter.Models;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Net.Mail;

namespace Build_a_PC_Sales_Deal_Hunter.Controllers
{
    public class StartTaskController : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            DbWork db = new DbWork();
            var tm = db.GetTasks();
            if (tm.Count == 0)
                return;
            var ListOfStoredProducts = new List<StoredProducts>();
            //Do something ever minute
            using (WebClient wc = new WebClient())
            {
                wc.Proxy = null;
                var json = wc.DownloadString("https://www.reddit.com/r/buildapcsales/search.json?q=&sort=new&restrict_sr=on&t=day");
                var jss = new JavaScriptSerializer();
                var items = jss.Deserialize<dynamic>(json);
                var d2 = items["data"]["children"];
                foreach (var a in d2)
                {
                    ListOfStoredProducts.Add(new StoredProducts() {Title = a["data"]["title"],URL = a["data"]["permalink"]});
                }
            }

            foreach(var task in tm)
            {
                foreach (var product in ListOfStoredProducts)
                {
                    if (product.Title.ToLower().Contains(task.Query.ToLower()))
                    {
                        int price;
                        try
                        {
                            price = PriceFound(product.Title);
                        }
                        catch(Exception e)
                        {
                            break;
                        }
                        
                        if (task.Price > price) 
                        {
                            if (!db.CheckIfEmailSent(product.URL, task.Email)) 
                            {
                                try 
                                {
                                    //You're in business buddy, prepare for an email.
                                    SendMail(new System.Net.Mail.MailMessage("BuildAPcSalesAlert@gmail.com", task.Email, "Sale Alert!", task.Query + " for $" + price + ": http://reddit.com/" + product.URL));
                                    //Write to EmailsSent Table to prevent duplicate emails being sent every minute
                                    db.LogEmailSent(product.URL, task.Email);
                                }
                                catch(Exception e)
                                {
                                    //Log error
                                    db.LogError("[" +e.Message + "] [" + e.InnerException + "] [" + e.Data + "]");
                                }
                            }
                        }
                    }
                }
            }
        }
        public SmtpClient GetSmtpClient() 
        {
            var smtp = new SmtpClient();
            smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSLOnMail"].ToString());
            return smtp;
        }
        public void SendMail(MailMessage message) 
        {
            GetSmtpClient().Send(message);
        }
        public int PriceFound(string Title)
        {
            char ch = Title[0];
            int i = 0;
            while (ch != '$')
            {
                ch = Title[i];
                i++;
            }
            var digits = Title.Substring(i, Title.Length - i).SkipWhile(c => !Char.IsDigit(c))
                .TakeWhile(Char.IsDigit)
                .ToArray();

            var str = new string(digits);
            return int.Parse(str);
        }
        private class StoredProducts
        {
            public string Title;
            public string URL;
        }
    }
}