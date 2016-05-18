using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Build_a_PC_Sales_Deal_Hunter.Models
{
    public class StatsInfoModel
    {
        public List<Error> Errors {get; set;}
        public int UniqueUsers {get; set;}
        public int EmailsSent { get; set; }
        public int Downloads { get; set; }
    }
    public class Error 
    {
        public DateTime Time;
        public string ErrorMessage;
    }
}
