using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    /// <summary>
    /// PrintersTablesController is a controller class for PrintersTable model.
    /// </summary>
    [CheckSessionTimeOut]
    public class PrintersTablesController : Controller
    {
        // Create an instance of the database context
        private ReportDBEntities1 db = new ReportDBEntities1();

        /// <summary>
        /// GET: PrintersTables - Index page for PrintersTable model.
        /// </summary>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public PartialViewResult _index()
        {
            var printersTable = db.PrintersTable.Include(p => p.ReportTable);
            return PartialView(printersTable.OrderBy(s => s.Time).ToList());
        }

        /// <summary>
        /// GET: Create - Create page for PrintersTable model in modal window.
        /// </summary>
        /// <returns></returns>
        [CheckSessionTimeOut]
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("Create");
        }

        /// <summary>
        /// POST: Create - Get data from modal window input form with check if data is valid. 
        /// </summary>
        /// <param name="printersTable"></param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [CheckSessionTimeOut]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "PrinterID,Time,User,Objective,Printer,ReportID")] PrintersTable printersTable)
        {
            Session["ErrorMessage"] = "";
            Guid passID;

            // Check if ReportID is not null from session data
            if (Session["ActiveGUID"] != null)
                passID = (Guid)Session["ActiveGUID"];
            else
            {
                // Close as 
                Session["ErrorMessage"] = "No ReportID was found. Refresh the page and fill this form again. If it's happen again, contact web administrator/developer.";
                return Json(this, JsonRequestBehavior.AllowGet);
            }

            // Check if data is valid
            if (ModelState.IsValid)
            {
                printersTable.PrinterID = Guid.NewGuid();
                printersTable.ReportID = passID;
                printersTable.User = printersTable.User.ToUpper();
                printersTable.Objective = printersTable.Objective.ToAutoCapitalize();
                printersTable.Printer = printersTable.Printer.ToUpper();

                // Add data to database
                db.PrintersTable.Add(printersTable);

                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PrintersTable | Create", $"Time:{printersTable.Time} Who:{printersTable.User} What:{printersTable.Objective} Printer:{printersTable.Printer}", userID);
                }
                catch { }

                // Save changes
                db.SaveChanges();

                // Return success message
                return Json (new { success = true });
            }
            else
            {
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PrintersTable | Create | Error", $"Time:{printersTable.Time} Who:{printersTable.User} What:{printersTable.Objective} Printer:{printersTable.Printer}", userID);
                }
                catch { }
            }

            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", printersTable.ReportID);

            // Return error message
            return Json(this, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// GET: Edit - Edit page for PrintersTable model in modal window.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckSessionTimeOut]
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrintersTable printersTable = db.PrintersTable.Find(id);
            if (printersTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", printersTable.ReportID);
            return PartialView("Edit", printersTable);
        }

        /// <summary>
        /// POST: Edit - Get data from modal window input form with check if data is valid.
        /// </summary>
        /// <param name="printersTable"></param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [CheckSessionTimeOut]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PrinterID,Time,User,Objective,Printer,ReportID")] PrintersTable printersTable)
        {
            Session["ErrorMessage"] = "";
            Guid passID;

            // Check if ReportID is not null from session data
            if (Session["ActiveGUID"] != null)
            {
                passID = (Guid)Session["ActiveGUID"];
            }
            else
            {
                // Close as 
                Session["ErrorMessage"] = "No ReportID was found. Refresh the page and fill this form again. If it's happen again, contact web administrator/developer.";
                return Json(this, JsonRequestBehavior.AllowGet);
            }

            // Check if data is valid
            if (ModelState.IsValid)
            {
                printersTable.ReportID = passID;
                printersTable.User = printersTable.User.ToUpper();
                printersTable.Objective = printersTable.Objective.ToAutoCapitalize();
                printersTable.Printer = printersTable.Printer.ToUpper();

                // Add data to database
                db.Entry(printersTable).State = EntityState.Modified;

                // Save changes
                db.SaveChanges();
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PrintersTable | Edit", $"Time:{printersTable.Time} Who:{printersTable.User} What:{printersTable.Objective} Printer:{printersTable.Printer}", userID);
                }
                catch { }

                // Return success message
                return Json(new { success = true });
            }
            else
            {
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PrintersTable | Edit | Error", $"Time:{printersTable.Time} Who:{printersTable.User} What:{printersTable.Objective} Printer:{printersTable.Printer}", userID);
                }
                catch { }
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", printersTable.ReportID);

            // Return error message
            return PartialView("Edit", printersTable);
        }

        /// <summary>
        /// GET: Delete - Delete page for PrintersTable model in modal window.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrintersTable printersTable = db.PrintersTable.Find(id);
            if (printersTable == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", printersTable);
        }

        /// <summary>
        /// POST: Delete - Delete data from database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckSessionTimeOut]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            PrintersTable printersTable = db.PrintersTable.Find(id);
            // Add data to database
            db.PrintersTable.Remove(printersTable);
            try
            {
                int userID;
                if (int.TryParse(Session["UserID"].ToString(), out userID))
                    LogHelper.AddLog(DateTime.Now, "PrintersTable | Delete", $"Time:{printersTable.Time} Who:{printersTable.User} What:{printersTable.Objective} Printer:{printersTable.Printer}", userID);
                }
            catch { }
            // Save changes
            db.SaveChanges();

            // Return success message
            return Json(new { success = true });
        }

        /// <summary>
        /// AJAX: GetWho - Get data from database column Who for autocomplete.
        /// </summary>
        /// <param name="term">Term from input autocomplete</param>
        /// <param name="cnt">Numbers of showed results</param>
        /// <returns>Results of "cnt" from searchable "term"</returns>
        public JsonResult GetWho(string term, int cnt)
        {
            var data = db.PrintersTable.Select(q => new
            {
                User = q.User
            }).Where(q => q.User.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AJAX: GetWhat - Get data from database column What for autocomplete.
        /// </summary>
        /// <param name="term">Term from input autocomplete</param>
        /// <param name="cnt">Numbers of showed results</param>
        /// <returns>Results of "cnt" from searchable "term"</returns>
        public JsonResult GetWhat(string term, int cnt)
        {
            var data = db.PrintersTable.Select(q => new
            {
                Objective = q.Objective
            }).Where(q => q.Objective.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AJAX: GetWhere - Get data from database column Where for autocomplete.
        /// </summary>
        /// <param name="term">Term from input autocomplete</param>
        /// <param name="cnt">Numbers of showed results</param>
        /// <returns>Results of "cnt" from searchable "term"</returns>
        public JsonResult GetWhere(string term, int cnt)
        {
            var data = db.PrintersTable.Select(q => new
            {
                Printer = q.Printer
            }).Where(q => q.Printer.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);
            return Json(data, JsonRequestBehavior.AllowGet);
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
