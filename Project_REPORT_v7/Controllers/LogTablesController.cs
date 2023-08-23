using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Project_REPORT_v7.Models;
using Project_REPORT_v7.Controllers.Addon;
using System.Linq;
using PagedList;
using System.Collections.Generic;

namespace Project_REPORT_v7.Controllers
{
    /// <summary>
    /// This controller is used to display the log table and filter the log table.
    /// </summary>
    public class LogTablesController : Controller
    {
        // Private database connection
        private ReportDBEntities1 db = new ReportDBEntities1();

        /// <summary>
        /// GET: Index page for the log table.
        /// </summary>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report_Admin")]
        public async Task<ActionResult> Index()
        {
            return View();
        }

        /// <summary>
        /// Async method to get the log table with filtering available.
        /// </summary>
        /// <param name="FilterLog">Filtering by type: ALL, ERROR, etc</param>
        /// <param name="UserDD">Filtering by user</param>
        /// <param name="dateFrom">Filter from date</param>
        /// <param name="dateTo">Filter to date</param>
        /// <param name="page">Page of showed data</param>
        /// <returns></returns>
        public async Task<ActionResult> Filter(string FilterLog, string UserDD, DateTime? dateFrom, DateTime? dateTo, int? page)
        {
            // Get the log table variable
            IQueryable<LogTable> logTable = db.LogTable;
            
            // Declare the filtered log table variable
            IOrderedQueryable<LogTable> filtered;

            // Temporary variable for the log count
            ViewBag.LogCount = 0;
            ViewBag.LogCount = logTable.Count();
            
            // Declare the page size and page number
            int pageSize = 35;
            int pageNumber = (page ?? 1);

            // Declare the date variables for filtering by user or by default
            DateTime from = dateFrom ?? DateTime.MinValue;
            DateTime to = dateTo ?? DateTime.MaxValue;

            int userID;

            // Filter data by the type search variable
            if (FilterLog != "All")
            {
                filtered = logTable.Where(w => w.L_TYPE.Contains(FilterLog)).OrderByDescending(o => o.L_DATE);
            }
            // If the type search variable is "All" then show all data
            else
            {
                filtered = logTable.OrderByDescending(o => o.L_DATE);
            }

            // Filter data by the user search variable
            if (int.TryParse(UserDD, out userID))
            {
                filtered = filtered.Where(w => w.L_USER_ID == userID).OrderByDescending(o => o.L_DATE);
            }

            // Filter data by the date search variable
            if (dateFrom != null && dateTo != null)
            {
                filtered = filtered.Where(w => w.L_DATE >= from && w.L_DATE <= to).OrderByDescending(o => o.L_DATE);
            }
            else if (dateFrom != null && dateTo == null)
            {
                filtered = filtered.Where(w => w.L_DATE >= from).OrderByDescending(o => o.L_DATE);
            }
            else if (dateFrom == null && dateTo != null)
            {
                filtered = filtered.Where(w => w.L_DATE <= to).OrderByDescending(o => o.L_DATE);
            }

            // Return the filtered data
            return PartialView("Filter", filtered.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// GET: Delete page for the log table.
        /// 
        /// Right now not in use.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Delete()
        {
            return View();
        }

        /// <summary>
        /// POST: Delete page for the log table.
        /// 
        /// Rigth now not in use.
        /// </summary>
        /// <param name="SpinNumber"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int SpinNumber)
        {
            if (SpinNumber > 0) 
            { 
                var logTable = await db.LogTable.OrderBy(o => o.L_DATE).Take(SpinNumber).ToListAsync();
                db.LogTable.RemoveRange(logTable.ToList());
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Public static variable for the LogTablesController
        public static LogTablesController LTC
        {
            get { return ltc; }
        }

        // Private static variable for the LogTablesController
        private static LogTablesController ltc = new LogTablesController();

        /// <summary>
        /// Static method to add log to the database.
        /// </summary>
        /// <param name="l_date">Date and time</param>
        /// <param name="l_type">CREATE|EDIT}DELETE type</param>
        /// <param name="l_message">Message</param>
        /// <param name="l_user">USER ID</param>
        /// <returns></returns>
        public static bool AddLogToDB(DateTime l_date, string l_type, string l_message, int l_user)
        {
            if (LTC.AddLog(l_date, l_type, l_message, l_user))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Method to add log to the database.
        /// </summary>
        /// <param name="l_date"></param>
        /// <param name="l_type"></param>
        /// <param name="l_message"></param>
        /// <param name="l_user"></param>
        /// <returns></returns>
        public bool AddLog(DateTime l_date, string l_type, string l_message, int l_user)
        {
            try
            {
                db.LogTable.Add(new LogTable()
                {
                    L_DATE = l_date,
                    L_TYPE = l_type,
                    L_MESSAGE = l_message,
                    L_USER_ID = l_user
                });
                // save changes to the database
                db.SaveChangesAsync();
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Method to populate the user drop down list from DB table.
        /// </summary>
        /// <returns>List of members as List of objects</returns>
        public static List<SelectListItem> PopulateMembers()
        {
            // Create the database object
            ReportDBEntities1 sdb = new ReportDBEntities1();

            // Create the list of select list items
            var sli = new List<SelectListItem>();

            // Create the temporary variable for the members table
            var temp = sdb.MembersTable.Select( s => new
            {
                Name = s.Name,
                ID = s.MemberID
            }).Distinct().ToList();

            // Populate the select list items
            sli = temp.ConvertAll(c =>
            {
                return new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.ID.ToString(),
                    Selected = false
                };
            });

            // Return the list of select list items
            return sli.ToList();
        }

        /// <summary>
        /// Dispose method for the LogTablesController
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
