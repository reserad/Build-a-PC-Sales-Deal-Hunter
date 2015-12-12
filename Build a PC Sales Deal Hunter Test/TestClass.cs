using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using Build_a_PC_Sales_Deal_Hunter.Controllers;
using System.Net.Mail;
using System.Configuration;

namespace Build_a_PC_Sales_Deal_Hunter_Test
{
    [TestClass]
    public class TestClass
    {
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
        [TestMethod]
        public void SendBlastEmail()
        {
            var db = new DbWork();
            var tm = db.GetTasks();

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

                                    //Shorten URL to AdFly if need be.
                                    UrlShortService u = new UrlShortService();
                                    string OriginalUrl = "http://reddit.com/" + product.URL;
                                    string ShortenedUrl = db.GetShortendedUrl(OriginalUrl);

                                    if (String.IsNullOrEmpty(ShortenedUrl))
                                        ShortenedUrl = u.GenerateShortUrl(OriginalUrl);
                                    //Log URL to prevent generating extra URL.
                                    db.LogUrlUsed(OriginalUrl, ShortenedUrl);

                                    //You're in business buddy, prepare for an email.
                                    MailMessage mm = new MailMessage(
                                        "BuildAPcSalesAlert@gmail.com",
                                        task.Email, "Sale Alert!",
                                        "<div style='padding: 10px; background-color:#d9d9d9'><h1>Build A PC Sales Email Service</h1>" +
                                        task.Query + " for $" + price + " <a href='" + ShortenedUrl +"'>" + OriginalUrl + "</a>");
                                    mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                    mm.IsBodyHtml = true;
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
        private SmtpClient GetSmtpClient()
        {
            var smtp = new SmtpClient();
            smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSLOnMail"].ToString());
            return smtp;
        }
        private void SendMail(MailMessage message)
        {
            GetSmtpClient().Send(message);
        }
    }
}
