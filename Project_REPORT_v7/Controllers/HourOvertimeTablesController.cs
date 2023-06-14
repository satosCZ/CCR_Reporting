using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    public class HourOvertimeTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: HourOvertimeTables
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public PartialViewResult _index()
        {
            var hourOvertimeTable = db.HourOvertimeTable.Include(p => p.ReportTable);
            return PartialView(hourOvertimeTable.OrderByDescending(o => o.Time).ToList());
        }

        // GET: HourOvertimeTables/Create
        //[AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpGet]
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Create()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("Create");
        }

        // POST: HourOvertimeTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "OvertimeID,Time,Duration,Shop,Type,Description,Cooperation,ReportID")] HourOvertimeTable hourOvertimeTable)
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
                hourOvertimeTable.OvertimeID = Guid.NewGuid();
                hourOvertimeTable.ReportID = passID;
                hourOvertimeTable.Type = hourOvertimeTable.Type.ToAutoCapitalize();
                hourOvertimeTable.Description = hourOvertimeTable.Description.ToAutoCapitalize();
                db.HourOvertimeTable.Add(hourOvertimeTable);
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "HourOvertime | Create", $"Time:{hourOvertimeTable.Time} Duration:{hourOvertimeTable.Duration} Shop:{hourOvertimeTable.Shop} Type:{hourOvertimeTable.Type} Description:{hourOvertimeTable.Description} Cooperation:{hourOvertimeTable.Cooperation}", userID);
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
                        LogHelper.AddLog(DateTime.Now, "HourOvertime | Create | Error", $"Time:{hourOvertimeTable.Time} Duration:{hourOvertimeTable.Duration} Shop:{hourOvertimeTable.Shop} Type:{hourOvertimeTable.Type} Description:{hourOvertimeTable.Description} Cooperation:{hourOvertimeTable.Cooperation}", userID);
                }
                catch { }
            }

            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", hourOvertimeTable.ReportID);
            //return Json(new { success = false });
            return Json(this, JsonRequestBehavior.AllowGet);
        }

        // GET: HourOvertimeTables/Edit/5
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HourOvertimeTable hourOvertimeTable = db.HourOvertimeTable.Find(id);
            if (hourOvertimeTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", hourOvertimeTable.ReportID);
            return PartialView(hourOvertimeTable);
        }

        // POST: HourOvertimeTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OvertimeID,Time,Duration,Shop,Type,Description,Cooperation,ReportID")] HourOvertimeTable hourOvertimeTable)
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
                hourOvertimeTable.ReportID = passID;
                hourOvertimeTable.Type = hourOvertimeTable.Type.ToAutoCapitalize();
                hourOvertimeTable.Description = hourOvertimeTable.Description.ToAutoCapitalize();
                db.Entry(hourOvertimeTable).State = EntityState.Modified;
                db.SaveChanges();
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "HourOvertime | Edit", $"Time:{hourOvertimeTable.Time} Duration:{hourOvertimeTable.Duration} Shop:{hourOvertimeTable.Shop} Type:{hourOvertimeTable.Type} Description:{hourOvertimeTable.Description} Cooperation:{hourOvertimeTable.Cooperation}", userID);
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
                        LogHelper.AddLog(DateTime.Now, "HourOvertime | Edit | Error", $"Time:{hourOvertimeTable.Time} Duration:{hourOvertimeTable.Duration} Shop:{hourOvertimeTable.Shop} Type:{hourOvertimeTable.Type} Description:{hourOvertimeTable.Description} Cooperation:{hourOvertimeTable.Cooperation}", userID);
                }
                catch { }
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", hourOvertimeTable.ReportID);
            return Json(new { success = false });
        }

        // GET: HourOvertimeTables/Delete/5
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HourOvertimeTable hourOvertimeTable = db.HourOvertimeTable.Find(id);
            if (hourOvertimeTable == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", hourOvertimeTable);
        }

        // POST: HourOvertimeTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            HourOvertimeTable hourOvertimeTable = db.HourOvertimeTable.Find(id);
            db.HourOvertimeTable.Remove(hourOvertimeTable);
            try
            {
                int userID;
                if (int.TryParse(Session["UserID"].ToString(), out userID))
                    LogHelper.AddLog(DateTime.Now, "HourOvertime | Delete", $"Time:{hourOvertimeTable.Time} Duration:{hourOvertimeTable.Duration} Shop:{hourOvertimeTable.Shop} Type:{hourOvertimeTable.Type} Description:{hourOvertimeTable.Description} Cooperation:{hourOvertimeTable.Cooperation}", userID);
                }
            catch { }
            db.SaveChanges();
            return Json(new { success = true });
        }

        public static List<SelectListItem> GetShops()
        {
            ReportDBEntities1 sdb = new ReportDBEntities1();
            var ls = new List<SelectListItem>();

            var temp = sdb.MainTaskTable.Select(s => new
            {
                Shop = s.Shop
            }).Distinct().ToList();


            if (temp.Count > 2)
            {
                ls = temp.ConvertAll(a =>
                {
                    return new SelectListItem()
                    {
                        Text = a.Shop.ToString(),
                        Value = a.Shop.ToString(),
                        Selected = false
                    };
                });
            }
            else if (temp.Count <= 2)
            {
                ls.Add(
                    new SelectListItem()
                    {
                        Text = "HMMC",
                        Value = "HMMC",
                        Selected = false
                    });
                ls.Add(
                    new SelectListItem()
                    {
                        Text = "GLOVIS",
                        Value = "GLOVIS",
                        Selected = false
                    });
                ls.Add(
                    new SelectListItem()
                    {
                        Text = "TRANSYS",
                        Value = "TRANSYS",
                        Selected = false
                    });
            }
            var result = ls.Distinct().ToList();
            result[1].Selected = true;

            return result;
        }

        public JsonResult GetTypes(string term, int cnt)
        {
            var check = db.HourOvertimeTable.Select( s => new
            {
                Type = s.Type
            }).Where(w => w.Type.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);

            return Json(check, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDescriptions(string term, int cnt)
        {
            var check = db.HourOvertimeTable.Select(s => new
            {
                Description = s.Description
            }).Where(w => w.Description.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);

            return Json(check, JsonRequestBehavior.AllowGet);
        }   

        public JsonResult GetCooperations(string term, int cnt)
        {
            var check = db.HourOvertimeTable.Select(s => new
            {
                Cooperation = s.Cooperation
            }).Where(w => w.Cooperation.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);

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
