using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PagedList;
using Project_REPORT_v7.App_Start;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{

    public class ReportTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician", "ITHaeczMesSection")]
        public ViewResult Index(int? page)
        {
            var reportTable = db.ReportTable.Include(r => r.MembersTable).Include(r => r.MembersTable1);

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(reportTable.OrderByDescending(s => s.Date).ToPagedList(pageNumber, pageSize));
        }

        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician", "ITHaeczMesSection")]
        // GET: HomePage - ReportTables
        public ActionResult IndexHome()
        {
            var reportTable = db.ReportTable.Include(r => r.MembersTable).Include(r => r.MembersTable1).OrderByDescending(s => s.Date).ThenBy(t => t.Shift);
            return PartialView("IndexHome", reportTable.Take(5));
        }

        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician", "ITHaeczMesSection")]
        public ActionResult Filter(DateTime? fromDT, DateTime? toDT, int? page)
        {
            var reportTable = db.ReportTable.Include(r => r.MembersTable).Include(r => r.MembersTable1);
            int pageSize = 20;
            int pageNumber = (page ?? 1);

            DateTime from = fromDT.GetValueOrDefault();
            DateTime to = toDT.GetValueOrDefault().AddDays(1);

            //(fromDT != DateTime.MinValue) && (toDT == DateTime.MinValue) && 
            if ((fromDT != null && toDT == null))
            {

                var filteredDate = reportTable.Where(w => w.Date >= from).OrderByDescending(s => s.Date);
                return View("Index", filteredDate.ToPagedList(pageNumber, pageSize));
            }
            //(fromDT == DateTime.MinValue) && (toDT != DateTime.MinValue) && 
            else if ((fromDT == null && toDT != null))
            {
                var filteredDate = reportTable.Where(w => w.Date <= to).OrderByDescending(s => s.Date);
                return View("Index", filteredDate.ToPagedList(pageNumber, pageSize));
            }
            //(fromDT != DateTime.MinValue) && (toDT != DateTime.MinValue) &&
            else if ((fromDT != null && toDT != null))
            {
                var filteredDate = reportTable.Where(w => w.Date >= from && w.Date <= to).OrderByDescending(s => s.Date);
                return View("Index", filteredDate.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return View("Index",reportTable.OrderByDescending(s =>s.Date).ThenBy(t => t.Shift).ToPagedList(pageNumber, pageSize));
            }
        }

        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician", "ITHaeczMesSection")]
        // GET: ReportTables/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportTable reportTable = db.ReportTable.Find(id);
            TempData["ActiveGUID"] = (Guid)id;
            ViewData["ActiveGUID"] = (Guid)id;
            ViewBag.Parent = (Guid)id;
            if (reportTable == null)
            {
                return HttpNotFound();
            }
            return View(reportTable);
        }

        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        // GET: ReportTables/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReportTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReportID,Date,Shift,Member_One_ID,Member_Two_ID")] ReportTable reportTable)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    reportTable.ReportID = Guid.NewGuid();
                    try
                    {
                        switch (reportTable.Shift)
                        {
                            case "Morning":
                                reportTable.Date = reportTable.Date.AddHours(6);
                                break;
                            case "Afternoon":
                                reportTable.Date = reportTable.Date.AddHours(14);
                                break;
                            case "Night":
                                reportTable.Date = reportTable.Date.AddHours(22);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("ReportTablesController || Create || " + ex.Message);
                    }
                    finally
                    {
                        //try
                        //{
                        //    int userID;
                        //    if (int.TryParse(Session["User"].ToString(), out userID))
                        //        LogClass.AddLog(DateTime.Now, "ReportTable|Create", $"Created new Report,ID:{reportTable.ReportID} Date:{reportTable.Date} Shift:{reportTable.Shift} M1:{reportTable.Member_One_ID} M2:{reportTable.Member_Two_ID}", userID);
                        //}
                        //catch (Exception ex)
                        //{
                        //    Debug.WriteLine("ReportTablesController || Create-LOG || " + ex.Message);
                        //}

                    }
                    db.ReportTable.Add(reportTable);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch { }

            ViewBag.Member_One_ID = new SelectList(db.MembersTable, "MemberID", "FirstName", reportTable.Member_One_ID);
            ViewBag.Member_Two_ID = new SelectList(db.MembersTable, "MemberID", "FirstName", reportTable.Member_Two_ID);
            return View(reportTable);
        }


        // GET: ReportTables/Edit/5
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportTable reportTable = db.ReportTable.Find(id);
            if (reportTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.Member_One_ID = new SelectList(db.MembersTable, "MemberID", "FirstName", reportTable.Member_One_ID);
            ViewBag.Member_Two_ID = new SelectList(db.MembersTable, "MemberID", "FirstName", reportTable.Member_Two_ID);
            return View(reportTable);
        }

        // POST: ReportTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReportID,Date,Shift,Member_One_ID,Member_Two_ID")] ReportTable reportTable)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(reportTable).State = EntityState.Modified;
                    //int userID;
                    //if (int.TryParse(Session["User"].ToString(), out userID))
                    //    LogClass.AddLog(DateTime.Now, "ReportTable|Edit", $"Edited Report,ID:{reportTable.ReportID} Date:{reportTable.Date} Shift:{reportTable.Shift} M1:{reportTable.Member_One_ID} M2:{reportTable.Member_Two_ID}", userID);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            ViewBag.Member_One_ID = new SelectList(db.MembersTable, "MemberID", "FirstName", reportTable.Member_One_ID);
            ViewBag.Member_Two_ID = new SelectList(db.MembersTable, "MemberID", "FirstName", reportTable.Member_Two_ID);
            return View(reportTable);
        }

        // GET: ReportTables/Delete/5
        //[GroupAuthorize("ITMesAdmin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportTable reportTable = db.ReportTable.Find(id);
            if (reportTable == null)
            {
                return HttpNotFound();
            }
            return View(reportTable);
        }

        // POST: ReportTables/Delete/5
        //[GroupAuthorize("ITMesAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ReportTable reportTable = db.ReportTable.Find(id);
            foreach (var item in reportTable.ReIssueTable.Where(x => x.ReportID == reportTable.ReportID).ToList())
            {
                db.ReIssueTable.Remove(item);
            }
            foreach (var item in reportTable.PrintersTable.Where(x => x.ReportID == reportTable.ReportID).ToList())
            {
                db.PrintersTable.Remove(item);
            }
            foreach (var item in reportTable.PasswordTable.Where(x => x.ReportID == reportTable.ReportID).ToList())
            {
                db.PasswordTable.Remove(item);
            }
            foreach (var item in reportTable.PreCheckTable.Where(x => x.ReportID == reportTable.ReportID).ToList())
            {
                db.PreCheckTable.Remove(item);
            }
            foreach (var item in reportTable.HourOvertimeTable.Where(x => x.ReportID == reportTable.ReportID).ToList())
            {
                db.HourOvertimeTable.Remove(item);
            }
            foreach (var item in reportTable.MainTaskTable.Where(x => x.ReportID == reportTable.ReportID).ToList())
            {
                db.MainTaskTable.Remove(item);
            }
            
            db.ReportTable.Remove(reportTable);
            //int userID;
            //if (int.TryParse(Session["User"].ToString(), out userID))
            //    LogClass.AddLog(DateTime.Now, "ReportTable|Delete", $"Deleted entire Report with everything that was inside,ID:{reportTable.ReportID} Date:{reportTable.Date} Shift:{reportTable.Shift} M1:{reportTable.Member_One_ID} M2:{reportTable.Member_Two_ID}", userID);
            db.SaveChanges();
            return RedirectToAction("Index");
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
