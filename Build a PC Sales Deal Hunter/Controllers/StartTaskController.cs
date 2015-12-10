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
using System.Threading;

namespace Build_a_PC_Sales_Deal_Hunter.Controllers
{
    public class StartTaskController : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //Search for matches every 1 minute, sends email if match found, logs match to prevent duplicate emails from being sent.
            var db = new DbWork();
            var tm = db.GetTasks();
            if (tm.Count == 0)
                return;
            var ListOfStoredProducts = new List<StoredProducts>();
            using (var wc = new WebClient())
            {
                wc.Proxy = null;
                var items = new JavaScriptSerializer().Deserialize<dynamic>(wc.DownloadString("https://www.reddit.com/r/buildapcsales/search.json?q=&sort=new&restrict_sr=on&t=day"));
                foreach (var a in items["data"]["children"])
                {
                    ListOfStoredProducts.Add(new StoredProducts() { Title = a["data"]["title"], URL = a["data"]["permalink"] });
                }
            }
            FindMatches(tm, ListOfStoredProducts, db);
        }
        private SmtpClient GetSmtpClient() 
        {
            var smtp = new SmtpClient();
            smtp.Port = 25;
            smtp.EnableSsl = true;
            smtp.Timeout = 10000;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            return smtp;
        }
        private void SendMail(MailMessage message) 
        {
            GetSmtpClient().Send(message);
        }
        private int PriceFound(string Title)
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
        private void FindMatches(List<TaskModel> tm, List<StoredProducts> ListOfStoredProducts, DbWork db) 
        {
            foreach (var task in tm)
            {
                foreach (var product in ListOfStoredProducts)
                {
                    if (product.Title.ToLower().Contains(" " + task.Query.ToLower()) || product.Title.ToLower().Contains(task.Query.ToLower() + " ") || product.Title.ToLower().Contains("[" + task.Query.ToLower() + "]"))
                    {
                        int price;
                        try
                        {
                            price = PriceFound(product.Title);
                        }
                        catch (Exception e)
                        {
                            break;
                        }

                        if (task.Price >= price)
                        {
                            try
                            {
                                if (!db.CheckIfEmailSent(product.URL, task.Email))
                                {
                                    //Write to EmailsSent Table to prevent duplicate emails being sent every minute
                                    db.LogEmailSent(product.URL, task.Email);
                                    //You're in business buddy, prepare for an email.
                                    MailMessage mm = new MailMessage("BuildAPcSalesAlert@gmail.com", task.Email, "Sale Alert!", task.Query + " for $" + price + ": http://reddit.com/" + product.URL);
                                    mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                    SendMail(mm);
                                    mm.Dispose();
                                }
                            }
                            catch (Exception e)
                            {
                                //Log error
                                db.LogError("[" + e.Message + "] [" + e.TargetSite + "] [" + e.Source + "] [" + e.Data + "]");
                            }
                        }
                    }
                }
            }
        }
    }
}