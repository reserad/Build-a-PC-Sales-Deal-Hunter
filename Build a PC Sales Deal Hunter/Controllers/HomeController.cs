﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Build_a_PC_Sales_Deal_Hunter.Models;
using RedditNotifier.Data;

namespace Build_a_PC_Sales_Deal_Hunter.Controllers
{
    public class HomeController : Controller
    {
        private List<SummaryItemModel> GetSummaryResults(List<TaskModel> Tasks) 
        {
            var returnObject = new List<SummaryItemModel>();
            var uniqueQueries = new Dictionary<string, int>();
            var items = new HashSet<string>();
            foreach (var item in Tasks)
            {
                items.Add(item.Query);
            }

            foreach (var _item in items)
            {
                uniqueQueries.Add(_item, Tasks.Count(item => item.Query == _item));
            }

            var myList = uniqueQueries.ToList();

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
            return View(GetSummaryResults(DbWork.GetTasks()));
        }
        [HttpPost]
        public ActionResult Index(string email, string[] query, string[] lessThan)
        {
            //Write to Emails table
            for (int i = 0; i < query.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(query[i])) 
                {
                    try
                    {
                        DbWork.AddTask(email.ToLower(), query[i].ToLower(), Convert.ToInt32(lessThan[i]));
                        ViewData["submit"] = true;
                    }
                    catch (Exception e)
                    {
                        Logging.LogError("[" + e.Message + "] [" + e.InnerException + "] [" + e.Data + "]");
                    }
                }
            }
            return View(GetSummaryResults(DbWork.GetTasks()));
        }
        [HttpPost]
        public JsonResult Remove(string email) 
        {
            //Remove from Emails and EmailsSent tables
            if (!string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    DbWork.RemoveFromEmailService(email.ToLower());
                }
                catch (Exception e)
                {
                    Logging.LogError("[" + e.Message + "] [" + e.InnerException + "] [" + e.Data + "]");
                    return Json(false);
                }
            }
            return Json(true);
        }
        [HttpPost]
        public JsonResult IndividualTask(string email) 
        {
            var tasks = new List<TaskModel>();
            try
            {
                tasks = DbWork.GetIndividualTask(email);
            }
            catch (Exception e)
            {
                Logging.LogError("[" + e.Message + "] [" + e.InnerException + "] [" + e.Data + "]");
            }

            return Json(tasks);
        }
        [HttpPost]
        public JsonResult DeleteIndividualTask(string email, string query, int price) 
        {
            try
            {
                DbWork.DeleteIndividualTask(email, query, price);
            }
            catch (Exception e)
            {
                Logging.LogError("[" + e.Message + "] [" + e.InnerException + "] [" + e.Data + "]");
                return Json(false);
            }
            return Json(true);
        }
        public ActionResult Stats() 
        {
            return View(DbWork.GetStatus());
        }
    }
}