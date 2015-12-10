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



        public ActionResult Index()
        {
            ViewData["submit"] = false;

            Dictionary<string, int> uniqueQueries = new Dictionary<string, int>();
            var db = new DbWork();

            HashSet<string> items = new HashSet<string>();
            var Tasks = db.GetTasks();
            foreach (var item in Tasks)
            {
                items.Add(item.Query);
            }

            foreach (var _item in items)
            {
                var count = Tasks.Count(item => item.Query == _item);
                uniqueQueries.Add(_item, count);
            }
            return View(from entry in uniqueQueries orderby entry.Value descending select entry);
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