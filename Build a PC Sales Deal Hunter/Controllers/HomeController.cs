using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Build_a_PC_Sales_Deal_Hunter.Models;
using RedditNotifier.Data;

namespace Build_a_PC_Sales_Deal_Hunter.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var Summary = new List<SummaryItemModel>();
            try
            {
                ViewData["submit"] = false;
                Summary = SummaryItemModel.GetSummaryResults(DbWork.GetAllTasks());
            }
            catch (Exception e)
            {
                Logging.LogError("[" + e.Message + "] [" + e.InnerException + "] [" + e.Data + "]");
            }
            return View(Summary);
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
            return View(SummaryItemModel.GetSummaryResults(DbWork.GetAllTasks()));
        }
        [HttpPost]
        public JsonResult Remove(string email) 
        {
            //Remove from Emails and EmailsSent tables
            if (!string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    DbWork.RemoveFromEmailServiceByEmail(email.ToLower());
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
            //Gets wanted-to-be-searched-for tasks by specifc user
            var tasks = new List<TaskModel>();
            try
            {
                tasks = DbWork.GetTasksByIndividual(email);
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
            //Delete search task by Email Query and Price.
            try
            {
                DbWork.DeleteTaskByIndividual(email, query, price);
            }
            catch (Exception e)
            {
                Logging.LogError("[" + e.Message + "] [" + e.InnerException + "] [" + e.Data + "]");
            }
            return Json(true);
        }
        public ActionResult Stats() 
        {
            //Obtains stats page data.
            var status = new StatsInfoModel();
            try
            {
                status = DbWork.GetStats();
            }
            catch (Exception e)
            {
                Logging.LogError("[" + e.Message + "] [" + e.InnerException + "] [" + e.Data + "] Stats");
            }
            return View(status);
        }
    }
}