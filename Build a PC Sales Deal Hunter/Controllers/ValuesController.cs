using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Build_a_PC_Sales_Deal_Hunter.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/<controller>
        public ActionResult Get()
        {

            using (var wc = new WebClient())
            {
                wc.Proxy = null;
                var items = new JavaScriptSerializer().Deserialize<dynamic>(wc.DownloadString("https://www.reddit.com/r/buildapcsales/search.json?q=&sort=new&restrict_sr=on&t=day"));
                JsonResult result = new JsonResult();
                result.Data = items;

                string json = new JavaScriptSerializer().Serialize(result.Data);

                return result;
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