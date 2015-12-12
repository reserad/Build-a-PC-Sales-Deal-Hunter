using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Build_a_PC_Sales_Deal_Hunter.Models;
using System.Net;
using System.Web.Script.Serialization;

namespace Build_a_PC_Sales_Deal_Hunter.Controllers
{
    public class HomeController : Controller
    {
        private List<SummaryItemModel> GetSummaryResults(List<TaskModel> Tasks) 
        {
            List<SummaryItemModel> returnObject = new List<SummaryItemModel>();
            Dictionary<string, int> uniqueQueries = new Dictionary<string, int>();
            HashSet<string> items = new HashSet<string>();
            foreach (var item in Tasks)
            {
                items.Add(item.Query);
            }

            foreach (var _item in items)
            {
                uniqueQueries.Add(_item, Tasks.Count(item => item.Query == _item));
            }

            List<KeyValuePair<string, int>> myList = uniqueQueries.ToList();

            myList.Sort((x, y) => x.Value.CompareTo(y.Value));
            myList.Reverse();

            uniqueQueries = myList.ToDictionary(pair => pair.Key, pair => pair.Value);

            var returnList = uniqueQueries.Keys.ToList();
            int increment = 0;
            foreach (var i in returnList)
            {
                if (increment == 5)
                    break;
                returnObject.Add(new SummaryItemModel()
                {
                    Count = Convert.ToInt32((double)uniqueQueries[i] / (double)uniqueQueries.Count() * (double)100),
                    Query = i
                });
                increment++;
            }
            return returnObject;
        }

        public ActionResult Index()
        {
            ViewData["submit"] = false;
            var db = new DbWork();           
            return View(GetSummaryResults(db.GetTasks()));
        }
        [HttpPost]
        public ActionResult Index(string email, string[] query, string[] lessThan)
        {
            var db = new DbWork();
            //Write to Emails table
            for (int i = 0; i < query.Length; i++)
            {
                if (!String.IsNullOrWhiteSpace(query[i])) 
                {
                    try
                    {
                        db.AddTask(email.ToLower(), query[i].ToLower(), Convert.ToInt32(lessThan[i]));
                        ViewData["submit"] = true;
                    }
                    catch (Exception e)
                    {
                        db.LogError("[" + e.Message + "] [" + e.InnerException + "] [" + e.Data + "]");
                    }
                }
            }
            return View();
        }
        [HttpPost]
        public JsonResult Remove(string email) 
        {
            //Remove from Emails and EmailsSent tables
            DbWork db = new DbWork();
            if (!String.IsNullOrWhiteSpace(email))
            {
                try
                {
                    db.RemoveFromEmailService(email.ToLower());
                }
                catch (Exception e)
                {
                    db.LogError("[" + e.Message + "] [" + e.InnerException + "] [" + e.Data + "]");
                    return Json(false);
                }
            }
            return Json(true);
        }
        [HttpPost]
        public JsonResult IndividualTask(string email) 
        {
            var db = new DbWork();
            var tasks = new List<TaskModel>();
            try
            {
                tasks = db.GetIndividualTask(email);
            }
            catch (Exception e)
            {
                db.LogError("[" + e.Message + "] [" + e.InnerException + "] [" + e.Data + "]");
            }

            return Json(tasks);
        }
        [HttpPost]
        public JsonResult DeleteIndividualTask(string email, string query, int price) 
        {
            var db = new DbWork();
            try
            {
                db.DeleteIndividualTask(email, query, price);
            }
            catch (Exception e)
            {
                db.LogError("[" + e.Message + "] [" + e.InnerException + "] [" + e.Data + "]");
                return Json(false);
            }

            return Json(true);
        }
        public ActionResult Stats() 
        {
            var db = new DbWork();
            return View(db.GetStatus());
        }
    }
}