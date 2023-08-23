using System.Web.Mvc;

namespace Project_REPORT_v7
{
    /// <summary>
    /// FilterConfig class
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// RegisterGlobalFilters method
        /// </summary>
        /// <param name="filters"> </param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
