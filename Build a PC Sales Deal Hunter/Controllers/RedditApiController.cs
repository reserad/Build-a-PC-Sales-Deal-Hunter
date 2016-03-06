using Newtonsoft.Json.Linq;
using RedditNotifier.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Mvc;

namespace Build_a_PC_Sales_Deal_Hunter.Controllers
{
    public class RedditApiController : ApiController
    {
        // GET api/<controller>
        public JObject Get()
        {
            try 
            {
                return JObject.Parse(DbWork.GetJson());
            } 
            catch(Exception e) 
            {
                Logging.LogError("[" + e.Message + "] [" + e.TargetSite + "] [" + e.Source + "] [" + e.Data + "]" + " API");
            }
            return null;
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}