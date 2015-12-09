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
        [TestMethod]
        public void SendBlastEmail()
        {
            DbWork db = new DbWork();
            List<string> uniqueEmails = new List<string>();

            foreach (var task in db.GetTasks())
            {
                if(!uniqueEmails.Contains(task.Email))
                    uniqueEmails.Add(task.Email);
            }

            foreach(var item in uniqueEmails)
            {
                try 
                {
                    SendMail(new System.Net.Mail.MailMessage("BuildAPcSalesAlert@gmail.com", item, "subject", "body"));
                }
                catch(Exception e)
                {
                    break;
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
