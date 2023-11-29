using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    /// <summary>
    /// PasswordTablesController a controller class
    /// </summary>
    [CheckSessionTimeOut]
    public class PasswordTablesController : Controller
    {
        // Database connection
        private ReportDBEntities1 db = new ReportDBEntities1();

        /// <summary>
        /// GET: Index page of PasswordTables
        /// </summary>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public PartialViewResult _index()
        {
            // Get the all data from password table that have same reportID as the active report
            var passwordTable = db.PasswordTable.Include(p => p.ReportTable);

            // Return the data to the view
            return PartialView(passwordTable.OrderBy(s => s.Time).ToList());
        }

        /// <summary>
        /// GET: Filtered index page of PasswordTables
        /// </summary>
        /// <param name="filterPW">Filter by shops choosen with radiobuttons</param>
        /// <param name="pwFromDT">Date from</param>
        /// <param name="pwToDT">Date to</param>
        /// <param name="pwPage">Pagination</param>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin" )]
        public PartialViewResult FilterIndex(string filterPW, DateTime? pwFromDT, DateTime? pwToDT, int? pwPage)
        {
            var passwordTable = db.PasswordTable.Include(p => p.ReportTable);

            int pageSize = 20;
            int pageNumber = (pwPage ?? 1);
            
            // Get the date from and to or set it to default
            DateTime from = pwFromDT.GetValueOrDefault();
            DateTime to = pwToDT.GetValueOrDefault();

            IQueryable<PasswordTable> filterByCompany;

            // Define the shops that will be filtered
            string[] glovisPass = new string[] { "ELIS", "GWMS", "GCS", "GLOVIS AD" };
            var filters = glovisPass.Select(s => s.ToLower()).ToList();
            IOrderedQueryable<PasswordTable> filtered;

            // Filter the data by the shops
            if (filterPW == "GLOVIS")
            {
                filterByCompany = passwordTable.Where(w => filters.Contains(w.System));
            }
            else
            {
                filterByCompany = passwordTable;
            }

            // Filter the data by the date      
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

            // Return the data to the view
            return PartialView("FilterIndex", filtered.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// GET: Create page of PasswordTables
        /// </summary>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("Create");
        }

        /// <summary>
        ///  POST: Getting data from txtboxes and save it to database with ajax and check if the data is valid. Ajax method
        /// </summary>
        /// <param name="passwordTable">Data from html form</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "PasswordID,Time,FullName,UserID,System,ReportID")] PasswordTable passwordTable)
        {
            Session["ErrorMessage"] = "";
            Guid passID;
            // Get the active reportID from session data
            if (Session["ActiveGUID"] != null)
                passID = (Guid)Session["ActiveGUID"];
            else
            {
                // Close as 
                Session["ErrorMessage"] = "No ReportID was found. Refresh the page and fill this form again. If it's happen again, contact web administrator/developer.";
                return Json(this, JsonRequestBehavior.AllowGet);
            }

            // Check if the data is valid
            if (ModelState.IsValid)
            {
                passwordTable.PasswordID = Guid.NewGuid();
                passwordTable.ReportID = passID;
                passwordTable.FullName = passwordTable.FullName.ToCapitalize();
                passwordTable.UserID = passwordTable.UserID.ToUpperCaps();
                passwordTable.System = passwordTable.System.ToUpperCaps();

                // Add the data to database
                db.PasswordTable.Add(passwordTable);
                // Remove comments to enable logging
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PasswordTable | Create", $"Time:{passwordTable.Time} Full Name:{passwordTable.FullName} UserID:{passwordTable.UserID} System:{passwordTable.System} ", userID);
                }
                catch { }

                // Save the changes
                db.SaveChanges();
                return Json (new { success = true });
            }
            else
            {
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PasswordTable | Create | Error", $"Time:{passwordTable.Time} Full Name:{passwordTable.FullName} UserID:{passwordTable.UserID} System:{passwordTable.System} ", userID);
                }
                catch { }
            }

            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", passwordTable.ReportID);

            // Return success to ajax
            return Json(this, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Edit page of PasswordTables with ajax
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

            // Return the data to the view and open the modal window
            return PartialView(passwordTable);
        }

        /// <summary>
        /// POST: Getting data from txtboxes and save it to database with ajax and check if the data is valid. Ajax method
        /// </summary>
        /// <param name="passwordTable"></param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PasswordID,Time,FullName,UserID,System,ReportID")] PasswordTable passwordTable)
        {
            Session["ErrorMessage"] = "";
            Guid passID;
            // Get the active reportID from session data
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

            // Check if the data is valid
            if (ModelState.IsValid)
            {
                passwordTable.ReportID = passID;
                passwordTable.FullName = passwordTable.FullName.ToCapitalize();
                passwordTable.UserID = passwordTable.UserID.ToUpperCaps();
                passwordTable.System = passwordTable.System.ToUpperCaps();
                // Make the data as modified
                db.Entry(passwordTable).State = EntityState.Modified;

                // Save the changes
                db.SaveChanges();
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PasswordTable | Edit", $"Time:{passwordTable.Time} Full Name:{passwordTable.FullName} UserID:{passwordTable.UserID} System:{passwordTable.System} ", userID);
                }
                catch { }

                // Return success to ajax
                return Json(new { success = true });
            }
            else
            {
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "PasswordTable | Edit | Error", $"Time:{passwordTable.Time} Full Name:{passwordTable.FullName} UserID:{passwordTable.UserID} System:{passwordTable.System} ", userID);
                }
                catch { }
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", passwordTable.ReportID);
            return PartialView("Edit", passwordTable);
        }

        /// <summary>
        /// GET: Delete page of PasswordTables with ajax
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

            // Return the data to the view and open the modal window
            return PartialView("Delete", passwordTable);
        }

        /// <summary>
        /// POST: Delete PasswordTables with ajax
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            // Find the data with the given id
            PasswordTable passwordTable = db.PasswordTable.Find(id);

            // Remove the data
            db.PasswordTable.Remove(passwordTable);
            // Log the action
            try
            {
                int userID;
                if (int.TryParse(Session["UserID"].ToString(), out userID))
                    LogHelper.AddLog(DateTime.Now, "PasswordTable | Delete", $"Time:{passwordTable.Time} Full Name:{passwordTable.FullName} UserID:{passwordTable.UserID} System:{passwordTable.System} ", userID);
                }
            catch { }

            // Save the changes
            db.SaveChanges();

            // Return success to ajax
            return Json(new { success = true });
        }

        /// <summary>
        /// Ajax method. Get the data from database and return it to the view
        /// </summary>
        /// <param name="term"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public JsonResult GetWhere(string term, int cnt)
        {
            var data = db.PasswordTable.Select(q => new
            {
                System = q.System
            }).Where(q => q.System.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Dispose the database connection
        /// </summary>
        /// <param name="disposing"></param>
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
