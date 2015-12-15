using System.Web;
using System.Web.Optimization;

namespace Build_a_PC_Sales_Deal_Hunter
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/sweet-alert.min.js",
                      "~/Scripts/jquery.dataTables.min.js",
                      "~/Scripts/jqbar.js",
                      "~/Scripts/typeahead.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/sweet-alert.css",
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/jqbar.css",
                      "~/Content/typeahead.js-bootstrap3.css",
                      "~/Content/site.css"));
        }
    }
}
