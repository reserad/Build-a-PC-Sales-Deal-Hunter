using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Quartz;
using System.Net;
using Build_a_PC_Sales_Deal_Hunter.Models;
using System.Web.Script.Serialization;

namespace Build_a_PC_Sales_Deal_Hunter.Controllers
{
    public class StartTaskController : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var tm = DbWork.GetTasks();
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
                    ListOfStoredProducts.Add(new StoredProducts() {Title = a["data"]["title"],URL = a["data"]["url"]});
                }
            }

            foreach(var task in tm)
            {
                foreach (var product in ListOfStoredProducts)
                {
                    if (product.Title.Contains(task.Query))
                    {
                        var price = PriceFound(product.Title);
                        if (task.Price > price) 
                        {
                            //You're in business buddy, prepare for an email.
                            //Write to EmailsSent Table to prevent duplicate emails being sent every minute
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
        private class StoredProducts
        {
            public string Title;
            public string URL;
        }
    }


}