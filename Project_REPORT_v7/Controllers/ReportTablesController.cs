using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    static class BLogData
    {
        public static string Log { get; set; } = "";
        public static bool IsError { get; set; } = false;
    }

    /// <summary>
    /// ReportTablesController is the main controller for the application.
    /// </summary>
    [CheckSessionTimeOut]
    public class ReportTablesController : Controller
    {
        // Private variable for db connection.
        private ReportDBEntities1 db = new ReportDBEntities1();

        /// <summary>
        /// GET: Index page for the reports.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public ViewResult Index(int? page)
        {
            var reportTable = db.ReportTable.Include(r => r.MembersTable).Include(r => r.MembersTable1);
            if (BLogData.IsError)
            {
                JSConsoleLog.Error(BLogData.Log);
            }
            else
            {
                JSConsoleLog.ConsoleLog(BLogData.Log);
            }
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(reportTable.OrderByDescending(s => s.Date).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// GET: Index page with the latest 5 reports showed on home page.
        /// </summary>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult IndexHome()
        {
            var reportTable = db.ReportTable.Include(r => r.MembersTable).Include(r => r.MembersTable1).OrderByDescending(s => s.Date).ThenBy(t => t.Shift);
            return PartialView("IndexHome", reportTable.Take(5));
        }

        /// <summary>
        /// POST: Filter the reports by date.
        /// </summary>
        /// <param name="fromDT"></param>
        /// <param name="toDT"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Filter(DateTime? fromDT, DateTime? toDT, int? page)
        {
            var reportTable = db.ReportTable.Include(r => r.MembersTable).Include(r => r.MembersTable1);
            int pageSize = 20;
            int pageNumber = (page ?? 1);

            // Filter by date. If fromDT is not null and toDT is null, filter by fromDT.
            if ((fromDT != null && toDT == null))
            {
                var filteredDate = reportTable.Where(w => w.Date >= fromDT).OrderByDescending(s => s.Date);
                return View("Index", filteredDate.ToPagedList(pageNumber, pageSize));
            }
            // Filter by date. If fromDT is null and toDT is not null, filter by toDT.
            else if ((fromDT == null && toDT != null))
            {
                var filteredDate = reportTable.Where(w => w.Date <= toDT).OrderByDescending(s => s.Date);
                return View("Index", filteredDate.ToPagedList(pageNumber, pageSize));
            }

            // Filter by date. If fromDT and toDT are not null, filter by both.
            else if ((fromDT != null && toDT != null))
            {
                var filteredDate = reportTable.Where(w => w.Date >= fromDT && w.Date <= toDT).OrderByDescending(s => s.Date);
                return View("Index", filteredDate.ToPagedList(pageNumber, pageSize));
            }

            // If fromDT and toDT are null, return the whole list.
            else
            {
                return View("Index",reportTable.OrderByDescending(s =>s.Date).ThenBy(t => t.Shift).ToPagedList(pageNumber, pageSize));
            }
        }

        /// <summary>
        /// GET: Details page with the report details. Inside the details page, there is a partial views for other index pages such as Re-Issue, Printers, PreCheck, Password, Hour OverTime, MainTask
        /// </summary>
        /// <param name="id">ID of the reportID</param>
        /// <returns></returns>
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

        /// <summary>
        /// GET: Method to get the next reportID and redirect to the details page.
        /// </summary>
        /// <param name="id">ID of the current opened report</param>
        /// <returns></returns>
        public ActionResult NextReport(Guid id)
        {
            var reportTable = db.ReportTable.Include(r => r.MembersTable).Include(r => r.MembersTable1).OrderByDescending(s => s.Date).ThenBy(t => t.Shift);
            int index = reportTable.ToList().FindIndex(f => f.ReportID == id);
            if (index == 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Details", new { id = reportTable.ToList() [index - 1].ReportID });
            }
        }

        /// <summary>
        /// GET: Method to get the previous reportID and redirect to the details page.
        /// </summary>
        /// <param name="id">ID of the current opened report</param>
        /// <returns></returns>
        public ActionResult PreviousReport(Guid id)
        {
            var reportTable = db.ReportTable.Include(r => r.MembersTable).Include(r => r.MembersTable1).OrderByDescending(s => s.Date).ThenBy(t => t.Shift);
            int index = reportTable.ToList().FindIndex(f => f.ReportID == id);
            if (index == reportTable.ToList().Count - 1)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Details", new { id = reportTable.ToList() [index + 1].ReportID });
            }
        }

        /// <summary>
        /// GET: Create page with the current date and shift calculated by last created report.
        /// </summary>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Create()
        {
            try
            {
                // Get the last created report.
                ReportTable reportTable = db.ReportTable.OrderByDescending(o => o.Date).First();

                // Calculate the next shift.
                switch (reportTable.Shift)
                {
                    case "Day":
                        ViewBag.Shift = "A";
                        break;
                    case "Afternoon":
                        ViewBag.Shift = "N"; 
                        break;
                    case "Night":
                        ViewBag.Shift = "D";
                        break;
                    default:
                        ViewBag.Shift = "D";
                        break;
                }

                // Get the next shift by ID of ShiftTable
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

                // Get the members of the next shift.
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

        /// <summary>
        /// POST: Create after submit the form with auto backup DB.
        /// </summary>
        /// <param name="reportTable"></param>
        /// <param name="MembersTable_Name"></param>
        /// <param name="MembersTable1_Name"></param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReportID,Date,Shift,Member_One_ID,Member_Two_ID,ShiftID")] ReportTable reportTable, [Bind(Prefix = "MembersTable.Name")] string MembersTable_Name, [Bind(Prefix = "MembersTable1.Name")] string MembersTable1_Name)
        {
            // Backup DB
            #region DB Backup
            bool doBackup;
            // Check if getting data from Web.config is successfully parsed as boolean
            if ( bool.TryParse( ConfigurationManager.AppSettings ["DoDBBackup"].ToString(), out doBackup ) )
            {
                // Check if doBackup is true aka backup is enabled
                if ( doBackup )
                {
                    try
                    {
                        // Get the connection string from Web.config
                        var sqlcon = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ReportDBEntities1"].ConnectionString);

                        // Get the current DB name
                        var curDB = "[C:\\MES\\WWWROOT\\APP_DATA\\REPORTDB.MDF]";

                        // Create the query string
                        var queryString = "BACKUP DATABASE " + curDB + " TO DISK = 'D:\\DB BACKUP\\REPORTDB_"+ DateTime.Now.ToString("yyyyMMdd") + ".BAK' WITH FORMAT, MEDIANAME = 'Z_SQLServerBackups', NAME = 'Full Backup of " + curDB + "';";
                        //var queryString = "BACKUP DATABASE " + curDB + " TO DISK = 'C:\\MES\\WWWROOT\\APP_DATA\\REPORTDB.BAK' WITH FORMAT, MEDIANAME = 'Z_SQLServerBackups', NAME = 'Full Backup of " + curDB + "';";

                        // Execute the query string
                        using ( var con = new System.Data.SqlClient.SqlConnection( sqlcon.ProviderConnectionString ) )
                        {
                            using ( var cmd = new System.Data.SqlClient.SqlCommand( queryString, con ) )
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                            }
                        }
                        Logger.LogInfo( BLogData.Log, "Create|POST");
                    }
                    catch ( Exception ex )
                    {
                        Logger.LogError( BLogData.Log, "Create|POST");
                    }
                }
            }
            #endregion

            // Check if the model is valid
            if ( ModelState.IsValid)
            {
                // Create new GUID for ReportID
                reportTable.ReportID = Guid.NewGuid();

                // Check if MembersTable_Name is same as members name & ID in DB
                if (db.MembersTable.Any(a => a.Name.ToLower() == MembersTable_Name.ToLower() && a.MemberID != reportTable.Member_One_ID))
                {
                    reportTable.Member_One_ID = db.MembersTable.Where(w => w.Name == MembersTable_Name).Select(s => s.MemberID).FirstOrDefault();
                }

                if (db.MembersTable.Any(a => a.Name.ToLower() == MembersTable1_Name.ToLower() && a.MemberID != reportTable.Member_Two_ID))
                {
                    reportTable.Member_Two_ID = db.MembersTable.Where(w => w.Name == MembersTable1_Name).Select(s => s.MemberID).FirstOrDefault();
                }

                // Check if the shift is not null
                int shiftID = reportTable.ShiftID.Value;
                if (shiftID != 0)
                {
                    reportTable.ShiftID = shiftID;
                }

                // Adding time for the date based on the shift
                try
                {
                    switch (reportTable.Shift)
                    {
                        case "Day":
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

                // Add the report to DB
                db.ReportTable.Add(reportTable);

                // Save changes
                db.SaveChanges();

                // Redirect to Index
                return RedirectToAction("Index");
            }

            ViewBag.Member_One_ID = new SelectList(db.MembersTable, "MemberID", "FirstName", reportTable.Member_One_ID);
            ViewBag.Member_Two_ID = new SelectList(db.MembersTable, "MemberID", "FirstName", reportTable.Member_Two_ID);
            return View(reportTable);
        }


        /// <summary>
        /// GET: Search for a reportID in DB and open it for editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// POST: After editing a report, save the changes to DB
        /// </summary>
        /// <param name="reportTable"></param>
        /// <returns></returns>
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

                // Test: Disabled code bellow in region to temperorary because unnecessary
                #region Add Time to Date
                // Check if date in report is withouth a time set
                if ( reportTable.Date.TimeOfDay.Ticks == 0 )
                {
                    try
                    {

                        switch ( reportTable.Shift )
                        {
                            case "Day":
                                reportTable.Date = reportTable.Date.AddHours( 6 );
                                break;
                            case "Afternoon":
                                reportTable.Date = reportTable.Date.AddHours( 14 );
                                break;
                            case "Night":
                                reportTable.Date = reportTable.Date.AddHours( 22 );
                                break;
                        }
                    }
                    catch ( Exception ex )
                    {
                        Debug.WriteLine( "ReportTablesController || Edit || " + ex.Message );
                        try
                        {
                            int userID;
                            if ( int.TryParse( Session ["UserID"].ToString(), out userID ) )
                                LogHelper.AddLog( DateTime.Now, "ReportTable | Edit | Error", $"Error in switch(reportTable.Shift) adding time: {ex.ToString()}", userID );
                        }
                        catch { }
                    }
                }
                // Remove comments to enable logging
                #endregion

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

        /// <summary>
        /// GET: Get a reportID from DB and open it for deleting
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// POST: Deleteing a report from DB also deletes all the data related to it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
            try
            {
                db.SaveChanges();
                if ( int.TryParse( Session ["UserID"].ToString(), out userID ) )
                    LogHelper.AddLog( DateTime.Now, "ReportTable | Delete", $"ID:{reportTable.ReportID} Date:{reportTable.Date} Shift:{reportTable.Shift} M1:{reportTable.Member_One_ID} M2:{reportTable.Member_Two_ID}", userID );

            }
            catch (Exception ex)
            {
                if ( int.TryParse( Session ["UserID"].ToString(), out userID ) )
                    LogHelper.AddLog( DateTime.Now, "ReportTable | Error", $"DELETE - GUID: {reportTable.ReportID} | Date: {reportTable.Date} | Message: {ex.Message} ", userID );
            }


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
