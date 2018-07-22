using System.Web;
using System.Web.Optimization;

namespace IconCaptcha.NET
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/icon-captcha").Include(
                      "~/Scripts/icon-captcha.min.js",
                      "~/Scripts/icon-captcha-main.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/icon-captcha.css",
                      "~/Content/demo.css"));
        }
    }
}
