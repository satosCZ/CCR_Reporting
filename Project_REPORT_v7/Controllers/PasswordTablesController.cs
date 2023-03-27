using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    //[AuthorizeAD(Groups = "CCR_Report")]
    public class PasswordTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: PasswordTables
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public PartialViewResult _index()
        {
            var passwordTable = db.PasswordTable.Include(p => p.ReportTable);
            return PartialView(passwordTable.OrderBy(s => s.Time).ToList());
        }

        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        public PartialViewResult FilterIndex(string filterPW, DateTime? pwFromDT, DateTime? pwToDT, int? pwPage)
        {
            var passwordTable = db.PasswordTable.Include(p => p.ReportTable);

            int pageSize = 20;
            int pageNumber = (pwPage ?? 1);

            DateTime from = pwFromDT.GetValueOrDefault();
            DateTime to = pwToDT.GetValueOrDefault();

            IQueryable<PasswordTable> filterByCompany;

            string[] glovisPass = new string[] { "ELIS", "GWMS", "GCS", "GLOVIS AD" };
            var filters = glovisPass.Select(s => s.ToLower()).ToList();
            IOrderedQueryable<PasswordTable> filtered;

            if (filterPW == "GLOVIS")
            {
                filterByCompany = passwordTable.Where(w => filters.Contains(w.System));
            }
            else
            {
                filterByCompany = passwordTable;
            }

            if (pwFromDT != null && pwToDT != null)
            {
                filtered = filterByCompany.Where(w => w.ReportTable.Date >= from && w.ReportTable.Date <= to).OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time);
            }
            else if (pwFromDT != null && pwToDT == null)
            {
                filtered = filterByCompany.Where(w => w.ReportTable.Date >= from).OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time);
            }
            else if (pwFromDT == null && pwToDT != null)
            {
                filtered = filterByCompany.Where(w => w.ReportTable.Date <= to).OrderByDescending(o => o.ReportTable.Date).ThenBy(t => t.Time);
            }
            else
            {
                filtered = filterByCompany.OrderByDescending(o => o.ReportTable.Date).ThenBy(t => t.Time);
            }
            return PartialView("FilterIndex", filtered.ToPagedList(pageNumber, pageSize));
        }

        // GET: PasswordTables/Create
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("Create");
        }

        // POST: PasswordTables/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "PasswordID,Time,FullName,UserID,System,ReportID")] PasswordTable passwordTable)
        {
            Guid passID = (Guid)TempData["ActiveGUID"];
            if (ModelState.IsValid)
            {
                passwordTable.PasswordID = Guid.NewGuid();
                passwordTable.ReportID = passID;
                db.PasswordTable.Add(passwordTable);
                // Remove comments to enable logging
                int userID;
                if (int.TryParse(Session["UserID"].ToString(), out userID))
                    LogHelper.AddLog(DateTime.Now, "PasswordTable | Create", $"Time:{passwordTable.Time} Full Name:{passwordTable.FullName} UserID:{passwordTable.UserID} System:{passwordTable.System} ", userID);
                db.SaveChanges();
                return Json (new { success = true });
            }

            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", passwordTable.ReportID);
            return Json(passwordTable, JsonRequestBehavior.AllowGet);
        }

        // GET: PasswordTables/Edit/5
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PasswordTable passwordTable = db.PasswordTable.Find(id);
            if (passwordTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", passwordTable.ReportID);
            return PartialView(passwordTable);
        }

        // POST: PasswordTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PasswordID,Time,FullName,UserID,System,ReportID")] PasswordTable passwordTable)
        {
            Guid passID = (Guid)TempData["ActiveGUID"];
            if (ModelState.IsValid)
            {
                passwordTable.ReportID = passID;
                db.Entry(passwordTable).State = EntityState.Modified;
                // Remove comments to enable logging

                int userID;
                if (int.TryParse(Session["UserID"].ToString(), out userID))
                    LogHelper.AddLog(DateTime.Now, "PasswordTable | Edit", $"Time:{passwordTable.Time} Full Name:{passwordTable.FullName} UserID:{passwordTable.UserID} System:{passwordTable.System} ", userID);
                db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", passwordTable.ReportID);
            return PartialView("Edit", passwordTable);
        }

        // GET: PasswordTables/Delete/5
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PasswordTable passwordTable = db.PasswordTable.Find(id);
            if (passwordTable == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", passwordTable);
        }

        // POST: PasswordTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            PasswordTable passwordTable = db.PasswordTable.Find(id);
            db.PasswordTable.Remove(passwordTable);
            // Remove comments to enable logging

            int userID;
            if (int.TryParse(Session["UserID"].ToString(), out userID))
                LogHelper.AddLog(DateTime.Now, "PasswordTable | Delete", $"Time:{passwordTable.Time} Full Name:{passwordTable.FullName} UserID:{passwordTable.UserID} System:{passwordTable.System} ", userID);
            db.SaveChanges();
            return Json(new { success = true });
        }

        public JsonResult GetWhere(string term)
        {
            var data = db.PasswordTable.Select(q => new
            {
                System = q.System
            }).Where(q => q.System.Contains(term)).Distinct().ToList();
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
