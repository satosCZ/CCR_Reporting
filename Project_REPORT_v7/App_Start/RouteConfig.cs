using System.Web.Mvc;
using System.Web.Routing;

namespace Project_REPORT_v7
{
    /// <summary>
    /// RouteConfig class
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Regiesters the routes
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
