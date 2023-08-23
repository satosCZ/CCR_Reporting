using System.Web.Optimization;

namespace Project_REPORT_v7
{
    /// <summary>
    /// Class BundleConfig
    ///     Automatically generated code from Visual Studio to bundle scripts and stylesheets
    /// </summary>
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Bundle for JQuery
            bundles.Add(new Bundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Bundle for Bootstrap jquery
            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js"));

            // Bundles for JQuerys scripts such as UI,  validation, ajax, cookies, dropzone, and NSB_Box
            bundles.Add(new Bundle("~/bundles/jquery-other").Include(
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/js-cookie.js",
                        "~/Scripts/dropzone/dropzone.min.js",
                        "~/Scripts/NSB_Box.js" ) );

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new Bundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Bundle for CSS stylesheets: bootstrap, NSB_Box, JQuery UI, dropzone and custom styles
            bundles.Add(new Bundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/NSB_Box.css",
                      "~/Content/site.css",
                      "~/Content/themes/base/all.css",
                      "~/Scripts/dropzone/dropzone.min.css" ) );
        }
    }
}
