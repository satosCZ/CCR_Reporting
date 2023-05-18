using System;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    //[AuthorizeAD(Groups ="CCR_Report")]
    [CheckSessionTimeOut]
    public class ReportTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public ViewResult Index(int? page)
        {
            var reportTable = db.ReportTable.Include(r => r.MembersTable).Include(r => r.MembersTable1);

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(reportTable.OrderByDescending(s => s.Date).ToPagedList(pageNumber, pageSize));
        }

        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult IndexHome()
        {
            var reportTable = db.ReportTable.Include(r => r.MembersTable).Include(r => r.MembersTable1).OrderByDescending(s => s.Date).ThenBy(t => t.Shift);
            return PartialView("IndexHome", reportTable.Take(5));
        }

        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Filter(DateTime? fromDT, DateTime? toDT, int? page)
        {
            var reportTable = db.ReportTable.Include(r => r.MembersTable).Include(r => r.MembersTable1);
            int pageSize = 20;
            int pageNumber = (page ?? 1);

            if ((fromDT != null && toDT == null))
            {
                var filteredDate = reportTable.Where(w => w.Date >= fromDT).OrderByDescending(s => s.Date);
                return View("Index", filteredDate.ToPagedList(pageNumber, pageSize));
            }
            else if ((fromDT == null && toDT != null))
            {
                var filteredDate = reportTable.Where(w => w.Date <= toDT).OrderByDescending(s => s.Date);
                return View("Index", filteredDate.ToPagedList(pageNumber, pageSize));
            }
            else if ((fromDT != null && toDT != null))
            {
                var filteredDate = reportTable.Where(w => w.Date >= fromDT && w.Date <= toDT).OrderByDescending(s => s.Date);
                return View("Index", filteredDate.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return View("Index",reportTable.OrderByDescending(s =>s.Date).ThenBy(t => t.Shift).ToPagedList(pageNumber, pageSize));
            }
        }

        // GET: ReportTables/Details/5
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Details(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportTable reportTable = db.ReportTable.Find(id);
            Session["ActiveGUID"] = (Guid)id;
            if (reportTable.Date >= DateTime.Now.AddHours(-9).AddMinutes(-10) || Session["isAdmin"].ToString() == "Admin")
            {
                Session["Closed"] = "false";
            }
            else
            {
                Session["Closed"] = "true";
            }
            if (reportTable == null)
            {
                return HttpNotFound();
            }
            return View(reportTable);
        }

        // GET: ReportTables/Create
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Create()
        {
            
            try
            {
                ReportTable reportTable = db.ReportTable.OrderByDescending(o => o.Date).First();
                switch (reportTable.Shift)
                {
                    case "Morning":
                        ViewBag.Shift = "A";
                        break;
                    case "Afternoon":
                        ViewBag.Shift = "N"; 
                        break;
                    case "Night":
                        ViewBag.Shift = "M";
                        break;
                    default:
                        ViewBag.Shift = "M";
                        break;
                }

                int tempShift;
                switch (reportTable.ShiftID)
                {
                    case 1:
                        ViewBag.ShiftID = tempShift = 3;
                        break;
                    case 2:
                        ViewBag.ShiftID = tempShift = 1;
                        break;
                    case 3:
                        ViewBag.ShiftID = tempShift = 2;
                        break;
                    default:
                        ViewBag.ShiftID = tempShift = 1;
                        break;
                }

                var members = db.MembersTable.Where(w => w.ShiftID == tempShift).ToList();

                // Save selected members for assigning to members select as default.
                ViewBag.Members = members;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "ReportTable | Create[GET] | Error", $"Message: {ex.ToString()}", userID);
                }
                catch { }
            }
            return View();
        }

        // POST: ReportTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReportID,Date,Shift,Member_One_ID,Member_Two_ID")] ReportTable reportTable)
        {
            if (ModelState.IsValid)
            {
                reportTable.ReportID = Guid.NewGuid();
                int shiftID = reportTable.ShiftID.Value;
                if (shiftID != 0)
                {
                    reportTable.ShiftID = shiftID;
                }
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
                    try
                    {
                        int userID;
                        if (int.TryParse(Session["UserID"].ToString(), out userID))
                            LogHelper.AddLog(DateTime.Now, "ReportTable | Create[POST] | Error", $"Error in switch(reportTable.Shift): {ex.ToString()}", userID);
                    }
                    catch { }
                }
                finally
                {
                    // Remove comments to enable logging

                    try
                    {
                        int userID;
                        if (int.TryParse(Session["UserID"].ToString(), out userID))
                            LogHelper.AddLog(DateTime.Now, "ReportTable | Create", $"ID:{reportTable.ReportID} Date:{reportTable.Date} Shift:{reportTable.Shift} M1:{reportTable.Member_One_ID} M2:{reportTable.Member_Two_ID}", userID);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("ReportTablesController || Create-LOG || " + ex.Message);
                    }

                }
                db.ReportTable.Add(reportTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Member_One_ID = new SelectList(db.MembersTable, "MemberID", "FirstName", reportTable.Member_One_ID);
            ViewBag.Member_Two_ID = new SelectList(db.MembersTable, "MemberID", "FirstName", reportTable.Member_Two_ID);
            return View(reportTable);
        }


        // GET: ReportTables/Edit/5
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
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
            ViewBag.Date = reportTable.Date.ToString("yyyy-MM-dd");
            return View(reportTable);
        }

        // POST: ReportTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReportID,Date,Shift,Member_One_ID,Member_Two_ID")] ReportTable reportTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reportTable).State = EntityState.Modified;

                // Check if date in report is withouth a time set
                if (reportTable.Date.TimeOfDay.Ticks == 0)
                {
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
                        try
                        {
                            int userID;
                            if (int.TryParse(Session["UserID"].ToString(), out userID))
                                LogHelper.AddLog(DateTime.Now, "ReportTable | Edit[POST] | Error", $"Error in switch(reportTable.Shift) adding time: {ex.ToString()}", userID);
                        }
                        catch { }
                    }
                }
                // Remove comments to enable logging

                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "ReportTable | Edit", $"ID:{reportTable.ReportID} Date:{reportTable.Date} Shift:{reportTable.Shift} M1:{reportTable.Member_One_ID} M2:{reportTable.Member_Two_ID}", userID);
                }
                catch { }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Member_One_ID = new SelectList(db.MembersTable, "MemberID", "FirstName", reportTable.Member_One_ID);
            ViewBag.Member_Two_ID = new SelectList(db.MembersTable, "MemberID", "FirstName", reportTable.Member_Two_ID);
            return View(reportTable);
        }

        // GET: ReportTables/Delete/5
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
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
        //[AuthorizeAD(Groups = "CCR_Report_Admin")]
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

            // Remove comments to enable logging

            int userID;
            if (int.TryParse(Session["UserID"].ToString(), out userID))
                LogHelper.AddLog(DateTime.Now, "ReportTable | Delete", $"ID:{reportTable.ReportID} Date:{reportTable.Date} Shift:{reportTable.Shift} M1:{reportTable.Member_One_ID} M2:{reportTable.Member_Two_ID}", userID);
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
