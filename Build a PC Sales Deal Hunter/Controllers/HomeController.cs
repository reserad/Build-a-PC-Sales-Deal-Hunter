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
            //Write to Emails Table
            for (int i = 0; i < query.Length; i++)
            {
                DbWork.AddTask(email, query[i], Convert.ToInt32(lessThan[i]));
            }
            return View();
        }
    }
}