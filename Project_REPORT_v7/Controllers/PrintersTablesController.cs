using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Project_REPORT_v7.App_Start;
using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    [AuthorizeAD(Groups = "CCR_Report")]
    public class PrintersTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: PrintersTables
        public PartialViewResult _index()
        {
            var printersTable = db.PrintersTable.Include(p => p.ReportTable);
            return PartialView(printersTable.OrderBy(s => s.Time).ToList());
        }

        // GET: PrintersTables/Create
        [AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("Create");
        }

        // POST: PrintersTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "PrinterID,Time,User,Objective,Printer,ReportID")] PrintersTable printersTable)
        {
            Guid passID = (Guid)TempData["ActiveGUID"];
            if (ModelState.IsValid)
            {
                printersTable.PrinterID = Guid.NewGuid();
                printersTable.ReportID = passID;
                db.PrintersTable.Add(printersTable);
                // Remove comments to enable logging

                //int userID;
                //if (int.TryParse(Session["User"].ToString(), out userID))
                //    LogClass.AddLog(DateTime.Now, "PrintersTable|Create", $"Created new Printer issue, Time:{printersTable.Time} Who:{printersTable.User} What:{printersTable.Objective} Printer:{printersTable.Printer}", userID);
                db.SaveChanges();
                return Json (new { success = true });
            }

            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", printersTable.ReportID);
            return Json(printersTable, JsonRequestBehavior.AllowGet);
        }

        // GET: PrintersTables/Edit/5
        [AuthorizeAD(Groups = "CCR_Report_Control")]
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

        // POST: PrintersTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PrinterID,Time,User,Objective,Printer,ReportID")] PrintersTable printersTable)
        {
            Guid passID = (Guid)TempData["ActiveGUID"];
            if (ModelState.IsValid)
            {
                printersTable.ReportID = passID;
                db.Entry(printersTable).State = EntityState.Modified;
                // Remove comments to enable logging

                //int userID;
                //if (int.TryParse(Session["User"].ToString(), out userID))
                //    LogClass.AddLog(DateTime.Now, "PrintersTable|Edit", $"Edited Printer issue, Time:{printersTable.Time} Who:{printersTable.User} What:{printersTable.Objective} Printer:{printersTable.Printer}", userID);
                db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", printersTable.ReportID);
            return PartialView("Edit", printersTable);
        }

        // GET: PrintersTables/Delete/5
        [AuthorizeAD(Groups = "CCR_Report_Control")]
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

        // POST: PrintersTables/Delete/5
        [AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            PrintersTable printersTable = db.PrintersTable.Find(id);
            db.PrintersTable.Remove(printersTable);
            // Remove comments to enable logging

            //int userID;
            //if (int.TryParse(Session["User"].ToString(), out userID))
            //    LogClass.AddLog(DateTime.Now, "PrintersTable|Delete", $"Deleted Printer issue, Time:{printersTable.Time} Who:{printersTable.User} What:{printersTable.Objective} Printer:{printersTable.Printer}", userID);
            db.SaveChanges();
            return Json(new { success = true });
        }

        public JsonResult GetWho(string term)
        {
            var data = db.PrintersTable.Select(q => new
            {
                User = q.User
            }).Where(q => q.User.Contains(term)).Distinct().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWhat(string term)
        {
            var data = db.PrintersTable.Select(q => new
            {
                Objective = q.Objective
            }).Where(q => q.Objective.Contains(term)).Distinct().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWhere(string term)
        {
            var data = db.PrintersTable.Select(q => new
            {
                Printer = q.Printer
            }).Where(q => q.Printer.Contains(term)).Distinct().ToList();
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
