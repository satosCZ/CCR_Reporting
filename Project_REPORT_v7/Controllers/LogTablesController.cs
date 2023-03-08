using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_REPORT_v7.Models;
using System.Diagnostics;
using Project_REPORT_v7.App_Start;

namespace Project_REPORT_v7.Controllers
{
    public class LogTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: LogTables
        [GroupAuthorize("ITMesAdmin")]
        public async Task<ActionResult> Index()
        {
            return View(await db.LogTable.ToListAsync());
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
