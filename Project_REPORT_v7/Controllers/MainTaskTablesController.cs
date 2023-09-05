using System;
using System.Collections.Generic;
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
    /// MainTaskTablesController class
    /// </summary>
    [CheckSessionTimeOut]
    public class MainTaskTablesController : Controller
    {
        // Private variable for database
        private ReportDBEntities1 db = new ReportDBEntities1();

        /// <summary>
        /// GET: Index page for MainTaskTables
        /// </summary>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public PartialViewResult _index()
        {
            var mainTaskTable = db.MainTaskTable.Include(p => p.ReportTable);
            return PartialView(mainTaskTable.OrderBy(s => s.Time).ToList());
        }

        /// <summary>
        /// GET: Filtered index for filter page
        /// </summary>
        /// <param name="filterMT">string - Shop filter</param>
        /// <param name="mtFromDT">DateTime - date from - nullable</param>
        /// <param name="mtToDT">DateTime - date to - nullable</param>
        /// <param name="mtFulltext">string - Fulltext filter</param>
        /// <param name="mtPage">Integer: page number - nullable</param>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        public PartialViewResult FilterIndex(string filterMT, DateTime? mtFromDT, DateTime? mtToDT, string mtFulltext, int? mtPage)
        {
            // Get all data from MainTaskTable
            var mainTaskTable = db.MainTaskTable.Include(p => p.ReportTable);

            // Declare filtered variable
            IOrderedQueryable<MainTaskTable> filtered;

            // Declare pagination variable size and page number
            int pageSize = 20;
            int pageNumber = (mtPage ?? 1);

            // Declare date variables and get values or defaults values
            DateTime from = mtFromDT.GetValueOrDefault();
            DateTime to = mtToDT.GetValueOrDefault();

            // Filter by company/shop
            if (filterMT == "GLOVIS" || filterMT == "TRANSYS")
            {
                filtered = mainTaskTable.Where(w => w.Shop == filterMT).OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time);
            }
            // Get all data
            else
            {
                filtered = mainTaskTable.OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time);
            }

            // Filter by date when dateTo and dateFrom are not null
            if (mtFromDT != null && mtToDT != null)
            {
                filtered = filtered.Where(w => w.ReportTable.Date >= from && w.ReportTable.Date <= to).OrderByDescending(o => o.ReportTable.Date).ThenBy(t => t.Time);
            }
            // Filter by date when dateTo is null and dateFrom is not null
            else if (mtFromDT != null && mtToDT == null)
            {
                filtered = filtered.Where(w => w.ReportTable.Date >= from).OrderByDescending(o => o.ReportTable.Date).ThenBy(t => t.Time);
            }
            // Filter by date when dateTo is not null and dateFrom is null
            else if (mtFromDT == null && mtToDT != null)
            {
                filtered = filtered.Where(w => w.ReportTable.Date <= to).OrderByDescending(o => o.ReportTable.Date).ThenBy(t => t.Time);
            }

            // Filter by fulltext
            if (!string.IsNullOrEmpty(mtFulltext) && mtFulltext != "undefined")
            {
                filtered = filtered.Where(w => w.System.Contains(mtFulltext) || w.Problem.Contains(mtFulltext) || w.Solution.Contains(mtFulltext)).OrderByDescending(o => o.ReportTable.Date).ThenBy(t => t.Time);
            }

            // Return filtered data
            return PartialView("FilterIndex", filtered.ToPagedList(pageNumber, pageSize));
        }


        /// <summary>
        /// GET: Create page for MainTaskTables showed as Modal window
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
        /// Ajax method for Create page for MainTaskTables showed as Modal window with checking for bad input
        /// </summary>
        /// <param name="mainTaskTable"></param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MainTaskID,Time,Duration,Shop,System,Problem,Solution,Cooperation,ReportID")] MainTaskTable mainTaskTable)
        {
            Session["ErrorMessage"] = "";
            Guid passID;
            // Check if ReportID is not null in Session data
            if (Session["ActiveGUID"] != null)
                passID = (Guid)Session["ActiveGUID"];
            else
            {
                Session["ErrorMessage"] = "No ReportID was found. Refresh the page and fill this form again. If it's happen again, contact web administrator/developer.";
                return Json(this, JsonRequestBehavior.AllowGet);
            }

            // Check if ModelState is valid which means that all required fields are filled correctly
            if (ModelState.IsValid)
            {
                mainTaskTable.MainTaskID = Guid.NewGuid();
                mainTaskTable.ReportID = passID;
                mainTaskTable.System = mainTaskTable.System.ToUpper();
                mainTaskTable.Problem = mainTaskTable.Problem.ToAutoCapitalize();
                mainTaskTable.Solution = mainTaskTable.Solution.ToAutoCapitalize();
                db.MainTaskTable.Add(mainTaskTable);

                // Add log
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "MainTaskTable | Create", $"Time:{mainTaskTable.Time} Duration:{mainTaskTable.Duration} Shop:{mainTaskTable.Shop} System:{mainTaskTable.System}, Problem:{mainTaskTable.Problem} Solution:{mainTaskTable.Solution} Cooperation:{mainTaskTable.Cooperation}", userID);
                }
                catch { }

                // Save changes in database
                db.SaveChanges();

                // Return success message
                return Json(new { success = true });
            }
            // In case if ModelState is not valid return error message and log error
            else
            {
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "MainTaskTable | Create | Error", $"Time:{mainTaskTable.Time} Duration:{mainTaskTable.Duration} Shop:{mainTaskTable.Shop} System:{mainTaskTable.System}, Problem:{mainTaskTable.Problem} Solution:{mainTaskTable.Solution} Cooperation:{mainTaskTable.Cooperation}", userID);
                }
                catch { }
            }

            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", mainTaskTable.ReportID);
            
            // Return error message
            return Json(this, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Edit page for MainTaskTables showed as Modal window
        /// </summary>
        /// <param name="id">Guid: ID of MainTaskTable data row</param>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            // Check if id is null
            if (id == null)
            {
                // Return error message
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Find MainTaskTable data row by id
            MainTaskTable mainTaskTable = db.MainTaskTable.Find(id);
            
            // Check if MainTaskTable data row is null
            if (mainTaskTable == null)
            {
                // Return error message
                return HttpNotFound();
            }

            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", mainTaskTable.ReportID);
            
            // Return data to view in Modal window
            return PartialView(mainTaskTable);
        }

        /// <summary>
        /// POST: Edit page for MainTaskTables showed as Modal window with checking for bad input
        /// </summary>
        /// <param name="mainTaskTable"></param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MainTaskID,Time,Duration,Shop,System,Problem,Solution,Cooperation,ReportID")] MainTaskTable mainTaskTable)
        {
            Session["ErrorMessage"] = "";
            Guid passID;

            // Check if ReportID is not null in Session data
            if (Session["ActiveGUID"] != null)
            {
                passID = (Guid)Session["ActiveGUID"];
            }
            // If ReportID is null return error message
            else
            {
                // Close as 
                Session["ErrorMessage"] = "No ReportID was found. Refresh the page and fill this form again. If it's happen again, contact web administrator/developer.";
                return Json(this, JsonRequestBehavior.AllowGet);
            }

            // Check if ModelState is valid which means that all required fields are filled correctly
            if (ModelState.IsValid)
            {
                mainTaskTable.ReportID = passID;
                mainTaskTable.System = mainTaskTable.System.ToUpper();
                mainTaskTable.Problem = mainTaskTable.Problem.ToAutoCapitalize();
                mainTaskTable.Solution = mainTaskTable.Solution.ToAutoCapitalize();

                // Set EntityState to Modified to update MainTaskTable data row
                db.Entry(mainTaskTable).State = EntityState.Modified;

                // Save changes in database
                db.SaveChanges();

                // Add log
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "MainTaskTable | Edit", $"Time:{mainTaskTable.Time} Duration:{mainTaskTable.Duration} Shop:{mainTaskTable.Shop} System:{mainTaskTable.System}, Problem:{mainTaskTable.Problem} Solution:{mainTaskTable.Solution} Cooperation:{mainTaskTable.Cooperation}", userID);
                }
                catch { }

                // Return success message
                return Json(new { success = true });
            }
            // In case if ModelState is not valid return error message and log error
            else
            {
                // Add log
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "MainTaskTable | Edit | Error", $"Time:{mainTaskTable.Time} Duration:{mainTaskTable.Duration} Shop:{mainTaskTable.Shop} System:{mainTaskTable.System}, Problem:{mainTaskTable.Problem} Solution:{mainTaskTable.Solution} Cooperation:{mainTaskTable.Cooperation}", userID);
                }
                catch { }
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", mainTaskTable.ReportID);
            // Return error message
            return PartialView(new { success = true });
        }

        /// <summary>
        /// GET: Delete page for MainTaskTables showed as Modal window
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Delete(Guid? id)
        {
            // Check if id is null
            if (id == null)
            {
                // Return error message
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Find MainTaskTable data row by id
            MainTaskTable mainTaskTable = db.MainTaskTable.Find(id);

            // Check if MainTaskTable data row is null
            if (mainTaskTable == null)
            {
                // Return error message
                return HttpNotFound();
            }

            // Return data to view in Modal window
            return PartialView("Delete", mainTaskTable);
        }

        /// <summary>
        /// POST: Delete page for MainTaskTables showed as Modal window
        /// </summary>
        /// <param name="id">Guid: ID of MainTaskTable data</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            // Find MainTaskTable data row by id
            MainTaskTable mainTaskTable = db.MainTaskTable.Find(id);

            // Remove MainTaskTable data row from database
            db.MainTaskTable.Remove(mainTaskTable);

            // Add log
            try
            {
                int userID;
                if (int.TryParse(Session["UserID"].ToString(), out userID))
                    LogHelper.AddLog(DateTime.Now, "MainTaskTable | Delete", $"Time:{mainTaskTable.Time} Duration:{mainTaskTable.Duration} Shop:{mainTaskTable.Shop} System:{mainTaskTable.System}, Problem:{mainTaskTable.Problem} Solution:{mainTaskTable.Solution} Cooperation:{mainTaskTable.Cooperation}", userID);
                }
            catch { }

            // Save changes in database
            db.SaveChanges();

            // Return success message
            return Json(new { success = true });
        }

        /// <summary>
        /// Ajax call to get data from MainTaskTable data table System column for autocomplete
        /// </summary>
        /// <param name="term">String: terminology for autocomplete</param>
        /// <param name="cnt">Integer: number of displayed autocomplete results</param>
        /// <returns>Retrun "cnt" of matched "term" data from table column System</returns>
        public JsonResult GetSystem(string term, int cnt)
        {
            // Get data from MainTaskTable data table System column for autocomplete
            var check = db.MainTaskTable.Select(q => new
            {
                System = q.System
            }).Where(
                q => q.System.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);

            // Return data to view
            return Json(check, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Ajax call to get data from MainTaskTable data table Problem column for autocomplete
        /// </summary>
        /// <param name="term">String: terminology for autocomplete</param>
        /// <param name="cnt">Integer: number of displayed autocomplete results</param>
        /// <returns>Retrun "cnt" of matched "term" data from table column Problem</returns>
        public JsonResult GetProblem(string term, int cnt)
        {
            // Get data from MainTaskTable data table Problem column for autocomplete
            var check = db.MainTaskTable.Select(q => new
            {
                Problem = q.Problem
            }).Where(
                q => q.Problem.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);

            // Return data to view
            return Json(check, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Ajax call to get data from MainTaskTable data table Solution column for autocomplete
        /// </summary>
        /// <param name="term">String: terminology for autocomplete</param>
        /// <param name="cnt">Integer: number of displayed autocomplete results</param>
        /// <returns>Retrun "cnt" of matched "term" data from table column Solution</returns>
        public JsonResult GetSolution(string term, int cnt)
        {
            // Get data from MainTaskTable data table Solution column for autocomplete
            var check = db.MainTaskTable.Select(q => new
            {
                Solution = q.Solution
            }).Where(
                q => q.Solution.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);

            // Return data to view
            return Json(check, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method called as Ajax to populate radio buttons for shop sellection
        /// </summary>
        /// <returns>Return shops all distinct and as List</returns>
        public static List<SelectListItem> GetShops()
        {
            ReportDBEntities1 sdb = new ReportDBEntities1();
            var ls = new List<SelectListItem>();

            var temp = sdb.MainTaskTable.Select( s => new
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
            else if(temp.Count <= 2)
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

        /// <summary>
        /// Dispose method for MainTaskTablesController
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
