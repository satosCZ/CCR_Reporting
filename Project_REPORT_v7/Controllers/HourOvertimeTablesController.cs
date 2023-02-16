using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_REPORT_v7.App_Start;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    public class HourOvertimeTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: HourOvertimeTables
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician", "ITHaeczMesSection")]
        public PartialViewResult _index()
        {
            var hourOvertimeTable = db.HourOvertimeTable.Include(p => p.ReportTable);
            return PartialView(hourOvertimeTable.OrderByDescending(o => o.Time).ToList());
        }

        // GET: HourOvertimeTables/Create
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("Create");
        }

        // POST: HourOvertimeTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "OvertimeID,Time,Duration,Shop,Type,Description,Cooperation,ReportID")] HourOvertimeTable hourOvertimeTable)
        {
            try
            {
                Guid passID = (Guid)TempData["ActiveGUID"];
                if (ModelState.IsValid)
                {
                    hourOvertimeTable.OvertimeID = Guid.NewGuid();
                    hourOvertimeTable.ReportID = passID;
                    db.HourOvertimeTable.Add(hourOvertimeTable);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", hourOvertimeTable.ReportID);
            return Json(new { success = false });    
        }

        // GET: HourOvertimeTables/Edit/5
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
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
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OvertimeID,Time,Duration,Shop,Type,Description,Cooperation,ReportID")] HourOvertimeTable hourOvertimeTable)
        {
            try
            {
                Guid passID = (Guid)TempData["ActiveGUID"];
                if (ModelState.IsValid)
                {
                    hourOvertimeTable.ReportID = passID;
                    db.Entry(hourOvertimeTable).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", hourOvertimeTable.ReportID);
            return Json(new { success = false });
        }

        // GET: HourOvertimeTables/Delete/5
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
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
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            HourOvertimeTable hourOvertimeTable = db.HourOvertimeTable.Find(id);
            db.HourOvertimeTable.Remove(hourOvertimeTable);
            db.SaveChanges();
            return Json(new { success = true });
        }

        public static List<SelectListItem> GetShops()
        {
            ReportDBEntities1 sdb = new ReportDBEntities1();
            var ls = new List<SelectListItem>();

            var temp = sdb.HourOvertimeTable.Select(s => new
            {
                Shop = s.Shop
            }).Distinct().ToList();

            ls = temp.ConvertAll(a =>
            {
                return new SelectListItem()
                {
                    Text = a.Shop.ToString(),
                    Value = a.Shop.ToString(),
                    Selected = false
                };
            });
            if (ls.Count() > 1)
                ls[1].Selected = true;

            return ls;
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
