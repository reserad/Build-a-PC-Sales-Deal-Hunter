using System;
using System.Collections.Generic;
using Quartz;
using System.Net;
using Build_a_PC_Sales_Deal_Hunter.Models;
using System.Web.Script.Serialization;
using System.Net.Mail;
using RedditNotifier.Data;

namespace Build_a_PC_Sales_Deal_Hunter.Controllers
{
    public class StartTaskController : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //Search for matches every 2 minutes, sends email if match found, logs match to prevent duplicate emails from being sent.
            var tm = DbWork.GetAllTasks();
            if (tm.Count == 0)
                return;
            var ListOfStoredProducts = new List<StoredProductsModel>();
            using (var wc = new WebClient())
            {
                wc.Proxy = null;
                wc.Headers.Add("User-Agent", "android:com.example.alec.buildapcsalesnotifier:v1.0.0 (by /u/reserad)");
                var items = new JavaScriptSerializer().Deserialize<dynamic>(wc.DownloadString("https://www.reddit.com/r/buildapcsales/search.json?q=&sort=new&restrict_sr=on&t=day"));

                if (string.IsNullOrEmpty(DbWork.GetJson()))
                    DbWork.AddJson(new JavaScriptSerializer().Serialize(items));
                else
                    DbWork.UpdateJson(new JavaScriptSerializer().Serialize(items));

                foreach (var a in items["data"]["children"])
                {
                    if (ListOfStoredProducts.Count == 2)
                        break;
                    ListOfStoredProducts.Add(new StoredProductsModel() { Title = a["data"]["title"], URL = a["data"]["permalink"] });
                }
            }
            FindMatches(tm, ListOfStoredProducts);
        }
        private SmtpClient GetSmtpClient() 
        {
            var smtp = new SmtpClient();
            smtp.Port = 80;
            smtp.EnableSsl = false;
            smtp.Timeout = 10000;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new SmtpClient().Credentials;
            return smtp;
        }
        private void SendMail(MailMessage message) 
        {
            GetSmtpClient().Send(message);
        }
        private int PriceFound(string Title)
        {
            int i = 0;
            while (Title[i] != '$')
                i++;

            i += 1;
            int j = i;
            while (j < Title.Length && (Char.IsDigit(Title[j]) || Title[j] == ' ' || Title[j] == ','))
                j++;

            string[] items = Title.Substring(i, (j - i)).Trim().Replace(",", "").Split(' ');
            return Convert.ToInt32(items[0].Trim());
        }

        private static bool findSearchTermsInTitle(string title, string searchTerm)
        {
            //Parses 'searchTerm' into a String array, looks for all cases in 'title' and determines if the SearchTerm is sufficient ('acceptancePercentage')
            string[] terms = searchTerm.Split(' ');
            double acceptancePercentage = 0.5;
            int correctHits = 0;
            foreach (string term in terms)
            {
                if (title.Contains(term))
                    correctHits++;
            }

            if (terms.Length > 2 && (double)correctHits / (double)terms.Length > acceptancePercentage)
                return true;
            else if(terms.Length == correctHits)
                return true;

            return false;
        }

        private void FindMatches(List<TaskModel> tm, List<StoredProductsModel> ListOfStoredProducts) 
        {
            foreach (var task in tm)
            {
                foreach (var product in ListOfStoredProducts)
                {
                    if (findSearchTermsInTitle(product.Title.ToLower(), task.Query.ToLower()))
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
                                if (!DbWork.CheckIfEmailSentToUser(product.URL, task.Email))
                                {
                                    string OriginalUrl = "http://reddit.com/" + product.URL;

                                    //You're in business buddy, prepare for an email.
                                    MailMessage mm = new MailMessage(
                                        "BuildAPcSalesAlert@gmail.com",
                                        task.Email, "Sale Alert!",
                                        "<div style='padding: 10px; background-color:#d9d9d9'><h1>Build A PC Sales Email Service</h1>" +
                                         " <a href='" + OriginalUrl + "'>" + task.Query + "</a> for $" + price  + "</div>" +
                                         "<p><a href='http://gotshrekt.com/Home/Android'>Android app available!</a></p>" +
                                    "<p><a href='http://www.gotshrekt.com'>Build a PC Sales Deal Hunter</a></p>");
                                    mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                    mm.IsBodyHtml = true;
                                    SendMail(mm);
                                    mm.Dispose();

                                    //Write to EmailsSent Table to prevent duplicate emails being sent every minute
                                    DbWork.LogEmailSent(product.URL, task.Email);
                                }
                            }
                            catch (Exception e)
                            {
                                //Log error
                                Logging.LogError("[" + e.Message + "] [" + e.TargetSite + "] [" + e.Source + "] [" + e.Data + "]" + " FindMatches");
                            }
                        }
                    }
                }
            }
        }
    }
}
