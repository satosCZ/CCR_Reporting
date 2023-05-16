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
    public class LogTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: LogTables
        [AuthorizeAD(Groups = "CCR_Report_Admin")]
        public async Task<ActionResult> Index(string FilterLog, string UserDD, DateTime? dateFrom, DateTime? dateTo, int? page)
        {
            IQueryable<LogTable> logTable = db.LogTable;
            IOrderedQueryable<LogTable> filtered;

            int pageSize = 35;
            int pageNumber = (page ?? 1);

            DateTime from = dateFrom ?? DateTime.MinValue;
            DateTime to = dateTo ?? DateTime.MaxValue;

            int userID;

            if (FilterLog != "All")
            {
                filtered = logTable.Where(w => w.L_TYPE.Contains(FilterLog)).OrderByDescending(o => o.L_DATE);
            }
            else
            {
                filtered = logTable.OrderByDescending(o => o.L_DATE);
            }

            if (int.TryParse(UserDD, out userID))
            {
                filtered = filtered.Where(w => w.L_USER_ID ==  userID).OrderByDescending(o => o.L_DATE);
            }

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
            return View("Index", filtered.ToPagedList(pageNumber, pageSize));
        }

        public static LogTablesController LTC
        {
            get { return ltc; }
        }

        private static LogTablesController ltc = new LogTablesController();

        public static bool AddLogToDB(DateTime l_date, string l_type, string l_message, int l_user)
        {
            if (LTC.AddLog(l_date, l_type, l_message, l_user))
                return true;
            else
                return false;
        }

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
                db.SaveChangesAsync();
                return true;
            }
            catch { }
            return false;
        }

        public static List<SelectListItem> PopulateMembers()
        {
            ReportDBEntities1 sdb = new ReportDBEntities1();
            var sli = new List<SelectListItem>();
            var temp = sdb.MembersTable.Select( s => new
            {
                Name = s.Name,
                ID = s.MemberID
            }).Distinct().ToList();

            sli = temp.ConvertAll(c =>
            {
                return new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.ID.ToString(),
                    Selected = false
                };
            });

            return sli.ToList();
        }

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
