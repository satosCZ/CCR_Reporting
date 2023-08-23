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
    /// PreCheckTablesController is a controller class for PreCheckTable model.
    /// </summary>
    [CheckSessionTimeOut]
    public class PreCheckTablesController : Controller
    {
        // Private variable for database connection
        private ReportDBEntities1 db = new ReportDBEntities1();

        /// <summary>
        /// GET: PreCheckTables index page.
        /// </summary>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public PartialViewResult _index()
        {
            var preCheckTable = db.PreCheckTable.Include(p => p.ReportTable);
            var passTable = preCheckTable.OrderBy(o => o.Time).ToList();
            return PartialView(passTable);
        }

        /// <summary>
        /// GET: Create new PreCheckTable modal window page
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
        /// POST: Create new PreCheckTable modal window page with valid data from form input and save it to database
        /// </summary>
        /// <param name="preCheckTable"></param>
        /// <returns></returns>
        [CheckSessionTimeOut]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "PreCheckID,Time,System,Check,EmailTime,ReportID")] PreCheckTable preCheckTable)
        {
            Session["ErrorMessage"] = "";
            Guid passID;

            // Check if ReportID is null or not in session data
            if (Session["ActiveGUID"] != null)
                passID = (Guid)Session["ActiveGUID"];
            else
            {
                // Close as 
                Logger.LogError( "No ReportID was found. Refresh the page and fill this form again.", "Project_REPORT_v7.Controllers.PreCheckTablesController" );
                return Json(this, JsonRequestBehavior.AllowGet);
            }

            // Check if model state is valid
            if (ModelState.IsValid)
            {
                preCheckTable.PreCheckID = Guid.NewGuid();
                preCheckTable.ReportID = passID;
                preCheckTable.System = preCheckTable.System.ToCapitalize();
                preCheckTable.Check = preCheckTable.Check.ToUpperCaps();

                // Add new PreCheckTable to database
                db.PreCheckTable.Add(preCheckTable);

                // Save changes to database
                db.SaveChanges();
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PreCheckTable | Create", $"Time:{preCheckTable.Time} System:{preCheckTable.System} Check:{preCheckTable.Check} EmailTime:{preCheckTable.EmailTime} ", userID);
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
                        LogHelper.AddLog(DateTime.Now, "PreCheckTable | Create | Error", $"Time:{preCheckTable.Time} System:{preCheckTable.System} Check:{preCheckTable.Check} EmailTime:{preCheckTable.EmailTime} ", userID);
                }
                catch { }
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", preCheckTable.ReportID);

            // Return error message if model state is invalid
            return Json (this, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Edit PreCheckTable modal window page
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
            PreCheckTable preCheckTable = db.PreCheckTable.Find(id);
            if (preCheckTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", preCheckTable.ReportID);
            return PartialView(preCheckTable);
        }

        /// <summary>
        /// POST: Edit PreCheckTable modal window page with valid data from form input and save it to database
        /// </summary>
        /// <param name="preCheckTable"></param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [CheckSessionTimeOut]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PreCheckID,Time,System,Check,EmailTime,ReportID")] PreCheckTable preCheckTable)
        {
            Session["ErrorMessage"] = "";
            Guid passID;

            // Check if ReportID is null or not in session data
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

            // Check if model state is valid
            if (ModelState.IsValid)
            {
                preCheckTable.ReportID = passID;
                preCheckTable.System = preCheckTable.System.ToCapitalize();
                preCheckTable.Check = preCheckTable.Check.ToUpperCaps();

                // Edit PreCheckTable in database
                db.Entry(preCheckTable).State = EntityState.Modified;

                // Save changes to database
                db.SaveChanges();
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PreCheckTable | Edit", $"Time:{preCheckTable.Time} System:{preCheckTable.System} Check:{preCheckTable.Check} EmailTime:{preCheckTable.EmailTime} ", userID);
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
                        LogHelper.AddLog(DateTime.Now, "PreCheckTable | Edit | Error", $"Time:{preCheckTable.Time} System:{preCheckTable.System} Check:{preCheckTable.Check} EmailTime:{preCheckTable.EmailTime} ", userID);
                }
                catch { }
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", preCheckTable.ReportID);

            // Return error message if model state is invalid
            return PartialView("Edit", preCheckTable);
        }

        /// <summary>
        /// GET: Delete PreCheckTable modal window page
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
            PreCheckTable preCheckTable = db.PreCheckTable.Find(id);
            if (preCheckTable == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", preCheckTable);
        }

        /// <summary>
        /// POST: Delete PreCheckTable modal window page with valid data from form input and delete it from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[AuthorizeAD(Groups = "CCR_Report_Control")]
        [CheckSessionTimeOut]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                PreCheckTable preCheckTable = db.PreCheckTable.Find(id);
                // Delete PreCheckTable from database
                db.PreCheckTable.Remove(preCheckTable);
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PreCheckTable | Delete", $"Time:{preCheckTable.Time} System:{preCheckTable.System} Check:{preCheckTable.Check} EmailTime:{preCheckTable.EmailTime} ", userID);
                }
                catch { }

                // Save changes to database
                db.SaveChanges();

                // Return success message
                return Json(new { success = true });
            }
            catch (Exception)
            {
                // Return error message
                return Json(new { success = false });
            }
            
        }

        /// <summary>
        /// AJAX: Get System column data from PreCheckTable system autocomplete
        /// </summary>
        /// <param name="term">Term of autocomplete imput</param>
        /// <param name="cnt">Number of showed results</param>
        /// <returns></returns>
        public JsonResult GetSystem(string term, int cnt)
        {
            // Get System column data from PreCheckTable system autocomplete
            var check = db.PreCheckTable.Select(q => new
            {
                System = q.System
            }).Where(q => q.System.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);

            // Return data as JSON
            return Json(check, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Dispose PreCheckTable controller class
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
