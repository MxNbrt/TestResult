using System.Web;
using System.Web.Optimization;

namespace TestResult
{
    public class BundleConfig
    {
        // Weitere Informationen zu Bundling finden Sie unter "http://go.microsoft.com/fwlink/?LinkId=254725".
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jsfiles").Include(
                "~/scripts/angular.js",
                "~/scripts/angular-route.js",
                "~/scripts/jquery-3.2.1.js",
                "~/scripts/dx.viz-web.debug.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/customscripts").Include(
                "~/customscripts/databasesvc.js",
                "~/customscripts/appareactrl.js",
                "~/customscripts/runctrl.js",
                "~/customscripts/gridutils.js",
                "~/customscripts/formutils.js",
                "~/customscripts/homectrl.js"
            ));

            bundles.Add(new StyleBundle("~/content/cssfiles").Include(
                "~/content/site.css",
                "~/content/dx.spa.css",
                "~/content/dx.common.css",
                "~/content/dx.light.css"
            ));
        }
    }
}