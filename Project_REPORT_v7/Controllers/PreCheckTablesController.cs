using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Project_REPORT_v7.App_Start;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    public class PreCheckTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: PreCheckTables partial
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician", "ITHaeczMesSection")]
        public PartialViewResult _index()
        {
            var preCheckTable = db.PreCheckTable.Include(p => p.ReportTable);
            var passTable = preCheckTable.OrderBy(o => o.Time).ToList();
            return PartialView(passTable);
        }

        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("Create");
        }

        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "PreCheckID,Time,System,Check,EmailTime,ReportID")] PreCheckTable preCheckTable)
        {
            Guid passID = (Guid)TempData["ActiveGUID"];
            if (ModelState.IsValid)
            {
                preCheckTable.PreCheckID = Guid.NewGuid();
                preCheckTable.ReportID = passID;
                db.PreCheckTable.Add(preCheckTable);
                //int userID;
                //if (int.TryParse(Session["User"].ToString(), out userID))
                //    LogClass.AddLog(DateTime.Now, "PreCheckTable|Create", $"Created new Pre-Check, Time:{preCheckTable.Time} System:{preCheckTable.System} Check:{preCheckTable.Check} EmailTime:{preCheckTable.EmailTime} ", userID);
                db.SaveChanges();
                return Json(new { success = true });
                
            }

            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", preCheckTable.ReportID);
           
            return Json (preCheckTable, JsonRequestBehavior.AllowGet);
        }

        // GET: PreCheckTables/Edit/5
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
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
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PreCheckID,Time,System,Check,EmailTime,ReportID")] PreCheckTable preCheckTable)
        {
            Guid passID = (Guid)TempData["ActiveGUID"];
            if (ModelState.IsValid)
            {
                preCheckTable.ReportID = passID;
                db.Entry(preCheckTable).State = EntityState.Modified;

                //int userID;
                //if (int.TryParse(Session["User"].ToString(), out userID))
                //    LogClass.AddLog(DateTime.Now, "PreCheckTable|Edit", $"Edited Pre-Check, Time:{preCheckTable.Time} System:{preCheckTable.System} Check:{preCheckTable.Check} EmailTime:{preCheckTable.EmailTime} ", userID);

                db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", preCheckTable.ReportID);
            return PartialView("Edit", preCheckTable);
        }

        // GET: PreCheckTables/Delete/5
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
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
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                PreCheckTable preCheckTable = db.PreCheckTable.Find(id);
                db.PreCheckTable.Remove(preCheckTable);
                //int userID;
                //if (int.TryParse(Session["User"].ToString(), out userID))
                //    LogClass.AddLog(DateTime.Now, "PreCheckTable|Delete", $"Deleted Pre-Check, Time:{preCheckTable.Time} System:{preCheckTable.System} Check:{preCheckTable.Check} EmailTime:{preCheckTable.EmailTime} ", userID);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public PartialViewResult SortOrder(string sortCriteria)
        {
            List<PreCheckTable> preCheckTables = null;
            if (sortCriteria.IsNullOrWhiteSpace())
                sortCriteria = sortCriteria.Contains("DSC") ? sortCriteria.Split('_')[0] : string.Format("{0}_DSC", sortCriteria);
            ViewBag.Time = "Time";
            ViewBag.System = "System";
            ViewBag.Check = "Check";

            switch (sortCriteria)
            {
                case "Time":
                    preCheckTables = db.PreCheckTable.Include(e => e.ReportTable).OrderBy(s => s.Time).ToList();
                    ViewBag.Time = "Time";
                    break;
                case "Time_DSC":
                    preCheckTables = db.PreCheckTable.Include(e => e.ReportTable).OrderByDescending(s => s.Time).ToList();
                    ViewBag.Time = "Time_DSC";
                    break;
                case "System":
                    preCheckTables = db.PreCheckTable.Include(e => e.ReportTable).OrderBy(s => s.System).ToList();
                    ViewBag.System = "System";
                    break;
                case "System_DSC":
                    preCheckTables= db.PreCheckTable.Include(e => e.ReportTable).OrderByDescending(s => s.System).ToList();
                    ViewBag.System = "System_DSC";
                    break;
                case "Check":
                    preCheckTables = db.PreCheckTable.Include(e => e.ReportTable).OrderBy(s => s.Check).ToList();
                    ViewBag.Check = "Check";
                    break;
                case "Check_DSC":
                    preCheckTables = db.PreCheckTable.Include(e => e.ReportTable).OrderByDescending(s => s.Check).ToList();
                    ViewBag.Check = "Check_DSC";
                    break;
            }
            return PartialView("_index", preCheckTables);
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
