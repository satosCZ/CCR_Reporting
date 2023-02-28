using Project_REPORT_v7.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public void Application_Error(Object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Server.ClearError();

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", "Error404");
            routeData.Values.Add("exception", exception);

            if (exception.GetType() == typeof(HttpException))
            {
                routeData.Values.Add("statusCode", ((HttpException)exception).GetHttpCode());
            }
            else
            {
                routeData.Values.Add("statusCode", 404);
            }

            Response.TrySkipIisCustomErrors = true;
            IController controller = new ErrorController();
            controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            Response.End();

            //var httpContext = ((MvcApplication)sender).Context;
            //var currentController = " ";
            //var currentAction = " ";
            //var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            //if (currentRouteData!= null)
            //{
            //    if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
            //    {
            //        currentController = currentRouteData.Values["controller"].ToString();
            //    }
            //    if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
            //    {
            //        currentAction = currentRouteData.Values["action"].ToString();
            //    }
            //}

            //var ex = Server.GetLastError();
            //var routeData = new RouteData();
            //var action = "Error";

            //if (ex is HttpException)
            //{
            //    var httpEx = ex as HttpException;
            //    switch(httpEx.GetHttpCode())
            //    {
            //        case 404:
            //            action = "Error404";
            //            break;
            //    }
            //}

            //httpContext.ClearError();
            //httpContext.Response.Clear();
            //httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
            //httpContext.Response.TrySkipIisCustomErrors = true;

            //routeData.Values["controller"] = "Error";
            //routeData.Values["action"] = action;
            //routeData.Values["exception"] = new HandleErrorInfo(ex, currentController, currentAction);

            //IController errorManagerController = new ErrorController();
            //HttpContextWrapper wrapper = new HttpContextWrapper(httpContext);
            //var rc = new RequestContext(wrapper, routeData);
            //errorManagerController.Execute(rc);
        }
    }
}
