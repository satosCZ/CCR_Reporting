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
    public class PrintersTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: PrintersTables
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public PartialViewResult _index()
        {
            var printersTable = db.PrintersTable.Include(p => p.ReportTable);
            return PartialView(printersTable.OrderBy(s => s.Time).ToList());
        }

        // GET: PrintersTables/Create
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("Create");
        }

        // POST: PrintersTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "PrinterID,Time,User,Objective,Printer,ReportID")] PrintersTable printersTable)
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
                printersTable.PrinterID = Guid.NewGuid();
                printersTable.ReportID = passID;
                printersTable.User = printersTable.User.ToUpper();
                printersTable.Objective = printersTable.Objective.ToAutoCapitalize();
                printersTable.Printer = printersTable.Printer.ToUpper();
                db.PrintersTable.Add(printersTable);
                // Remove comments to enable logging
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PrintersTable | Create", $"Time:{printersTable.Time} Who:{printersTable.User} What:{printersTable.Objective} Printer:{printersTable.Printer}", userID);
                }
                catch { }
                db.SaveChanges();
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
            return Json(this, JsonRequestBehavior.AllowGet);
        }

        // GET: PrintersTables/Edit/5
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

        // POST: PrintersTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PrinterID,Time,User,Objective,Printer,ReportID")] PrintersTable printersTable)
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
                printersTable.ReportID = passID;
                printersTable.User = printersTable.User.ToUpper();
                printersTable.Objective = printersTable.Objective.ToAutoCapitalize();
                printersTable.Printer = printersTable.Printer.ToUpper();
                db.Entry(printersTable).State = EntityState.Modified;
                db.SaveChanges();
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PrintersTable | Edit", $"Time:{printersTable.Time} Who:{printersTable.User} What:{printersTable.Objective} Printer:{printersTable.Printer}", userID);
                }
                catch { }
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
            return PartialView("Edit", printersTable);
        }

        // GET: PrintersTables/Delete/5
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

        // POST: PrintersTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            PrintersTable printersTable = db.PrintersTable.Find(id);
            db.PrintersTable.Remove(printersTable);
            // Remove comments to enable logging
            try
            {
                int userID;
                if (int.TryParse(Session["UserID"].ToString(), out userID))
                    LogHelper.AddLog(DateTime.Now, "PrintersTable | Delete", $"Time:{printersTable.Time} Who:{printersTable.User} What:{printersTable.Objective} Printer:{printersTable.Printer}", userID);
                }
            catch { }
            db.SaveChanges();
            return Json(new { success = true });
        }

        public JsonResult GetWho(string term, int cnt)
        {
            var data = db.PrintersTable.Select(q => new
            {
                User = q.User
            }).Where(q => q.User.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWhat(string term, int cnt)
        {
            var data = db.PrintersTable.Select(q => new
            {
                Objective = q.Objective
            }).Where(q => q.Objective.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

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
