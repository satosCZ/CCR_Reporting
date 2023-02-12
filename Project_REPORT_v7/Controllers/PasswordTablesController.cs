using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Project_REPORT_v7.App_Start;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    public class PasswordTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: PasswordTables
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician", "ITHaeczMesSection")]
        public PartialViewResult _index()
        {
            var passwordTable = db.PasswordTable.Include(p => p.ReportTable);
            return PartialView(passwordTable.OrderBy(s => s.Time).ToList());
        }

        public PartialViewResult FilterIndex(string filterPW, DateTime? pwFromDT, DateTime? pwToDT, int? pwPage)
        {
            var passwordTable = db.PasswordTable.Include(p => p.ReportTable);

            int pageSize = 20;
            int pageNumber = (pwPage ?? 1);

            //if (filter == "GWMS" || filter == "GCS" || filter == "ELIS" || filter == "GLOVIS AD")
            //{
            //    var filtered = passwordTable.OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time).Where(w => w.System == filter);
            //    return View("FilterIndex", filtered);
            //}

            // Array of Strings for filtering passwords systems
            if (filterPW == "GLOVIS")
            {
                string[] glovisPass = new string[] { "ELIS", "GWMS", "GCS", "GLOVIS AD" };
                // var filtered = passwordTable.OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time).Where(w => glovisPass.Any(w.System.Contains));
                var filters = glovisPass.Select(s => s.ToString()).ToList();
                var filtered = passwordTable.Where(w => filters.Contains(w.System)).OrderByDescending(o => o.ReportTable.Date).ThenBy(t => t.Time);
                return PartialView("FilterIndex", filtered.ToPagedList(pageNumber, pageSize));
            }

            // Filter by Date - From Date to Date || only From Date || only To Date
            if (pwFromDT != null && pwToDT != null)
            {
                var filtered = passwordTable.Where(w => w.ReportTable.Date >= pwFromDT && w.ReportTable.Date <= pwToDT).OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time);
                return PartialView("FilterIndex", filtered.ToPagedList(pageNumber, pageSize));
            }
            else if(pwFromDT != null && pwToDT == null)
            {
                var filtered = passwordTable.Where(w => w.ReportTable.Date >= pwFromDT).OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time);
                return PartialView("FilterIndex", filtered.ToPagedList(pageNumber, pageSize));
            }
            else if (pwFromDT == null && pwToDT != null)
            {
                var filtered = passwordTable.Where(w => w.ReportTable.Date <= pwToDT).OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time);
                return PartialView("FilterIndex", filtered.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return PartialView("FilterIndex", passwordTable.OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time).ToPagedList(pageNumber, pageSize)); 
            }            
        }

        // GET: PasswordTables/Create
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("Create");
        }

        // POST: PasswordTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
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
                //int userID;
                //if (int.TryParse(Session["User"].ToString(), out userID))
                //    LogClass.AddLog(DateTime.Now, "PasswordTable|Create", $"Created new Password issue, Time:{passwordTable.Time} Full Name:{passwordTable.FullName} UserID:{passwordTable.UserID} System:{passwordTable.System} ", userID);
                db.SaveChanges();
                return Json (new { success = true });
            }

            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", passwordTable.ReportID);
            return Json(passwordTable, JsonRequestBehavior.AllowGet);
        }

        // GET: PasswordTables/Edit/5
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
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
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PasswordID,Time,FullName,UserID,System,ReportID")] PasswordTable passwordTable)
        {
            Guid passID = (Guid)TempData["ActiveGUID"];
            if (ModelState.IsValid)
            {
                passwordTable.ReportID = passID;
                db.Entry(passwordTable).State = EntityState.Modified;
                //int userID;
                //if (int.TryParse(Session["User"].ToString(), out userID))
                //    LogClass.AddLog(DateTime.Now, "PasswordTable|Edit", $"Edited Password issue, Time:{passwordTable.Time} Full Name:{passwordTable.FullName} UserID:{passwordTable.UserID} System:{passwordTable.System} ", userID);
                db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", passwordTable.ReportID);
            return PartialView("Edit", passwordTable);
        }

        // GET: PasswordTables/Delete/5
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
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
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            PasswordTable passwordTable = db.PasswordTable.Find(id);
            db.PasswordTable.Remove(passwordTable);
            //int userID;
            //if (int.TryParse(Session["User"].ToString(), out userID))
            //    LogClass.AddLog(DateTime.Now, "PasswordTable|Delete", $"Deleted Password issue, Time:{passwordTable.Time} Full Name:{passwordTable.FullName} UserID:{passwordTable.UserID} System:{passwordTable.System} ", userID);
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
