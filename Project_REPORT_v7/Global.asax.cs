using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Project_REPORT_v7
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute(), 2);
            filters.Add(new HandleErrorAttribute { View = "~/Error/Error.cshtml" }, 1);
        }

        protected void Application_Start()
        {
            BundleTable.EnableOptimizations = false;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("cs-CZ");
        }

        protected void Application_BeginRequest()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("cs-CZ");

            // Redirect HTTP to HTTPS
            if (!Context.Request.IsSecureConnection)
            {
                Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
            }
        }
    }
}
