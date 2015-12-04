using System.Web;
using System.Web.Mvc;

namespace Build_a_PC_Sales_Deal_Hunter
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
