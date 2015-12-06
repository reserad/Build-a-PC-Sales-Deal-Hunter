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
            return View();
        }
        [HttpPost]
        public ActionResult Index(string email, string[] query, string[] lessThan)
        {
            DbWork db = new DbWork();
            //Write to Emails table
            for (int i = 0; i < query.Length; i++)
            {
                try
                {
                    db.AddTask(email.ToLower(), query[i].ToLower(), Convert.ToInt32(lessThan[i]));
                }
                catch(Exception e)
                {
                    db.LogError("[" +e.Message + "] [" + e.InnerException + "] [" + e.Data + "]");
                }
            }
            return View();
        }
        [HttpPost]
        public JsonResult Remove(string email) 
        {
            //Remove from Emails and EmailsSent tables
            DbWork db = new DbWork();
            try
            {
                db.RemoveFromEmailService(email.ToLower());
            }
            catch(Exception e)
            {
                db.LogError("[" +e.Message + "] [" + e.InnerException + "] [" + e.Data + "]");
                return Json(false);
            }
            return Json(true);
        }
    }
}