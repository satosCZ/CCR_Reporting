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
    public class PreCheckTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: PreCheckTables partial
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public PartialViewResult _index()
        {
            var preCheckTable = db.PreCheckTable.Include(p => p.ReportTable);
            var passTable = preCheckTable.OrderBy(o => o.Time).ToList();
            return PartialView(passTable);
        }

        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("Create");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "PreCheckID,Time,System,Check,EmailTime,ReportID")] PreCheckTable preCheckTable)
        {
            Session["ErrorMessage"] = "";
            Guid passID;
            if (Session["ActiveGUID"] != null)
                passID = (Guid)Session["ActiveGUID"];
            else
            {
                // Close as 
                Session["ErrorMessage"] = "No ReportID was found. Refresh the page and fill this form again. If it's happen again, contact web administrator/developer.";
                return Json(this, JsonRequestBehavior.AllowGet);
            }
            if (ModelState.IsValid)
            {
                preCheckTable.PreCheckID = Guid.NewGuid();
                preCheckTable.ReportID = passID;
                db.PreCheckTable.Add(preCheckTable);
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PreCheckTable | Create", $"Time:{preCheckTable.Time} System:{preCheckTable.System} Check:{preCheckTable.Check} EmailTime:{preCheckTable.EmailTime} ", userID);
                }
                catch { }
                db.SaveChanges();
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
            return Json (preCheckTable, JsonRequestBehavior.AllowGet);
        }

        // GET: PreCheckTables/Edit/5
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

        // POST: PreCheckTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PreCheckID,Time,System,Check,EmailTime,ReportID")] PreCheckTable preCheckTable)
        {
            Session["ErrorMessage"] = "";
            Guid passID;
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
            if (ModelState.IsValid)
            {
                preCheckTable.ReportID = passID;
                db.Entry(preCheckTable).State = EntityState.Modified;
                // Remove comments to enable logging
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PreCheckTable | Edit", $"Time:{preCheckTable.Time} System:{preCheckTable.System} Check:{preCheckTable.Check} EmailTime:{preCheckTable.EmailTime} ", userID);
                }
                catch { }

                db.SaveChanges();
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
            return PartialView("Edit", preCheckTable);
        }

        // GET: PreCheckTables/Delete/5
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

        // POST: PreCheckTables/Delete/5
        //[AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                PreCheckTable preCheckTable = db.PreCheckTable.Find(id);
                db.PreCheckTable.Remove(preCheckTable);
                // Remove comments to enable logging
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PreCheckTable | Delete", $"Time:{preCheckTable.Time} System:{preCheckTable.System} Check:{preCheckTable.Check} EmailTime:{preCheckTable.EmailTime} ", userID);
                }
                catch { }
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
            
        }

        public JsonResult GetSystem(string term)
        {
            var check = db.PreCheckTable.Select(q => new
            {
                System = q.System
            }).Where(q => q.System.Contains(term)).Distinct();
            return Json(check, JsonRequestBehavior.AllowGet);
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
