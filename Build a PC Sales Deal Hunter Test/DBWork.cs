using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace Build_a_PC_Sales_Deal_Hunter_Test
{
    [TestClass]
    public class DbWork
    {

            public class tempI
            {
                public int P;
                public string Q;
            }

        [TestMethod]
        public void FindMatches()
        {
            //Build_a_PC_Sales_Deal_Hunter.Controllers.DbWork db = new Build_a_PC_Sales_Deal_Hunter.Controllers.DbWork();

            List<Build_a_PC_Sales_Deal_Hunter.Models.TaskModel> tm = new List<Build_a_PC_Sales_Deal_Hunter.Models.TaskModel>();
            int count = 0;
            List<string> Queries = new List<string>();
            Queries.Add("GTX 970");
            Queries.Add("monitor 25");
            Queries.Add("corsair hydro");
            Queries.Add("msi z170a");
            Queries.Add("ips led 4k");
            Queries.Add("visiontek");
            Queries.Add("SSD");
            Queries.Add("Mechanical");
            Queries.Add("Gaming Keyboard");
            Queries.Add("Gaming Mouse");
            Queries.Add("i7-5820k");
            Random R = new Random();
            for (int i = 0; i < 10000; i++) 
            {
                Build_a_PC_Sales_Deal_Hunter.Models.TaskModel _tm = new Build_a_PC_Sales_Deal_Hunter.Models.TaskModel();
                _tm.Email = "test123@gmail.com";
                _tm.Query = Queries[R.Next(0, 7)];
                _tm.Price = R.Next(40, 500);
                tm.Add(_tm);
            }

            if (tm.Count == 0)
                return;
            var ListOfStoredProducts = new List<StoredProducts>();
            //Do something ever minute
            using (WebClient wc = new WebClient())
            {
                wc.Proxy = null;
                var items = new JavaScriptSerializer().Deserialize<dynamic>(wc.DownloadString("https://www.reddit.com/r/buildapcsales/search.json?q=&sort=new&restrict_sr=on&t=day"));
                foreach (var a in items["data"]["children"])
                {
                    ListOfStoredProducts.Add(new StoredProducts() { Title = a["data"]["title"], URL = a["data"]["permalink"] });
                }
            }
            List<tempI> foundItems = new List<tempI>();
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
                                tempI _item = new tempI();
                                _item.Q = task.Query;
                                _item.P = price;
                                foundItems.Add(_item);
                                //if (!db.CheckIfEmailSent(product.URL, task.Email))
                               // {
                                    //Write to EmailsSent Table to prevent duplicate emails being sent every minute
                                    //db.LogEmailSent(product.URL, task.Email);
                                    //You're in business buddy, prepare for an email.
                                    //SendMail(new System.Net.Mail.MailMessage("BuildAPcSalesAlert@gmail.com", task.Email, "Sale Alert!", task.Query + " for $" + price + ": http://reddit.com/" + product.URL));
                                //}
                            }
                            catch (Exception e)
                            {
                                //Log error
                                //db.LogError("[" + e.Message + "] [" + e.TargetSite + "] [" + e.Source + "] [" + e.Data + "]");
                            }
                        }
                    }
                }
            }
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
    }
    public class StoredProducts
    {
        public string Title;
        public string URL;
    }
}
