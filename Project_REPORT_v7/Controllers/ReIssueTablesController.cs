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
    /// <summary>
    /// ReIssueTablesController is a controller class for ReIssueTable model.
    /// </summary>
    [CheckSessionTimeOut]
    public class ReIssueTablesController : Controller
    {
        // Private variables for database connection and session
        private ReportDBEntities1 db = new ReportDBEntities1();

        /// <summary>
        /// GET: ReIssueTables - Index page for ReIssueTable model.
        /// </summary>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public PartialViewResult _index()
        {
            var reIssueTable = db.ReIssueTable.Include(p => p.ReportTable).OrderBy(s => s.Time);
            return PartialView(reIssueTable.ToList());
        }

        /// <summary>
        /// GET: CreateMultiple - modal window page with inputs
        /// </summary>
        /// <returns></returns>
        [CheckSessionTimeOut]
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult CreateMultiple()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("CreateMultiple");
        }

        /// <summary>
        /// POST: Create multiple ReIssueTable records from modal window inputs with checking for input errors. Due to the fact that the modal window is not a form, the data is passed through the ajax request.
        /// By default, the modal window is closed after the request is completed. If the request is successful, the page is reloaded. If the request is unsuccessful, the modal window remains open and the error message is displayed.
        /// Inputs: time, user, objective, bodyNum are separated due to possibility of multiple bodyNum inputs.
        /// </summary>
        /// <param name="time">input Time</param>
        /// <param name="user">input User</param>
        /// <param name="objective">input Objective</param>
        /// <param name="bodyNum">input Body Number/s</param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to, for more details se https://go.microsoft.com/fwlink/?LinkId=317598 .
        [CheckSessionTimeOut]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateMultiple(TimeSpan time, string user, string objective, string bodyNum)
        {
            Session["ErrorMessage"] = "";
            Guid passID;
            // Check if ReportID is not null
            if (Session["ActiveGUID"] != null)
                passID = (Guid)Session["ActiveGUID"];
            else
            {
                // Close as
                Session["ErrorMessage"] = "No ReportID was found. Refresh the page and fill this form again. If it's happen again, contact web administrator/developer.";
                return Json(this, JsonRequestBehavior.AllowGet);
            }

            // Check if inputs are not null
            if (ModelState.IsValid)
            {
                // Convert inputs to uppercase
                user = user.ToUpperCaps();
                objective = objective.ToUpperCaps();
                bodyNum = bodyNum.ToUpperCaps();

                // Check if bodyNum contains multiple body numbers separated by new line
                if (bodyNum.Contains("\r\n"))
                {
                    // Split bodyNum by new line
                    string[] separators = new string[] { "\r\n" };

                    // Create list of ReIssueTable records
                    List<ReIssueTable> multiple = new List<ReIssueTable>();

                    // Add each body number to the list
                    List<string> bodys = bodyNum.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();

                    // Add each body number to the list
                    foreach (string body in bodys)
                    {
                        multiple.Add(new ReIssueTable()
                        {
                            ReIssueID = Guid.NewGuid(),
                            ReportID = passID,
                            Time = time,
                            User = user.ToUpperCaps(),
                            Objective = objective.ToUpperCaps(),
                            BodyNum = body.ToUpperCaps()
                        });
                        try
                        {
                            int userId;
                            if (int.TryParse(Session["UserID"].ToString(), out userId))
                                LogHelper.AddLog(DateTime.Now, "ReIssueTable | CreateMultiple", $"Time:{time} Who:{user} Where:{objective} BodyNum:{body}", userId);
                        }
                        catch { }
                    }
                    // Add list of ReIssueTable records to the database
                    db.ReIssueTable.AddRange(multiple);
                    
                    // Save changes
                    db.SaveChanges();

                    // Return success message
                    return Json(new { success = true });
                }
                // If bodyNum contains only one body number
                else
                {
                    // Create new ReIssueTable record
                    ReIssueTable reIssueTable = new ReIssueTable();
                    reIssueTable.ReIssueID = Guid.NewGuid();
                    reIssueTable.ReportID = passID;
                    reIssueTable.Time = time;
                    reIssueTable.User= user.ToUpperCaps();
                    reIssueTable.Objective = objective.ToUpperCaps();
                    reIssueTable.BodyNum = bodyNum.ToUpperCaps();

                    // Add ReIssueTable record to the database
                    db.ReIssueTable.Add(reIssueTable);
                    // Remove comments to enable logging

                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "ReIssueTable | Create", $"Time:{time} Who:{user} Where:{objective} BodyNum:{bodyNum}", userID);

                    // Save changes
                    db.SaveChanges();
                    // Return success message
                    return Json(new { success = true });
                }
            }
            else
            {
                try
                {
                    int userId;
                    if (int.TryParse(Session["UserID"].ToString(), out userId))
                        LogHelper.AddLog(DateTime.Now, "ReIssueTable | Create | Error", $"Time:{time} Who:{user} Where:{objective} BodyNums:{bodyNum}", userId);
                }
                catch { }
            }

            // Return error message
            return Json(this, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Edit - modal window page with inputs
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[AuthorizeAD(Groups = "CCR_Report_Control")]
        [CheckSessionTimeOut]
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReIssueTable reIssueTable = db.ReIssueTable.Find(id);
            if (reIssueTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", reIssueTable.ReportID);
            return PartialView(reIssueTable);
        }

        /// <summary>
        /// POST: Edit - save changes to the database and close modal window
        /// </summary>
        /// <param name="reIssueTable"></param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[AuthorizeAD(Groups = "CCR_Report_Control")]
        [CheckSessionTimeOut]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReIssueID,Time,User,Objective,BodyNum,ReportID")] ReIssueTable reIssueTable)
        {
            Session["ErrorMessage"] = "";
            Guid passID;

            // Check if ReportID is not null from session data
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

            // Check if inputs are not null and valid
            if (ModelState.IsValid)
            {
                reIssueTable.ReportID = passID;
                reIssueTable.Time = reIssueTable.Time;
                reIssueTable.User = reIssueTable.User.ToUpperCaps();
                reIssueTable.Objective = reIssueTable.Objective.ToUpperCaps();
                reIssueTable.BodyNum = reIssueTable.BodyNum.ToUpperCaps();

                // Set entity state to modified
                db.Entry(reIssueTable).State = EntityState.Modified;

                // Save changes
                db.SaveChanges();
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "ReIssueTable | Edit", $"Time:{reIssueTable.Time} Who:{reIssueTable.User} Where:{reIssueTable.Objective} BodyNum:{reIssueTable.BodyNum}", userID);
                }
                catch { }

                // Return success message
                return Json(new { success = true });
            }
            else
            {
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "ReIssueTable | Edit | Error", $"Time:{reIssueTable.Time} Who:{reIssueTable.User} Where:{reIssueTable.Objective} BodyNum:{reIssueTable.BodyNum}", userID);
                }
                catch { }
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", reIssueTable.ReportID);

            // Return error message
            return PartialView("Edit", reIssueTable);
        }

        /// <summary>
        /// GET: Delete - modal window page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReIssueTable reIssueTable = db.ReIssueTable.Find(id);
            if (reIssueTable == null)
            {
                return HttpNotFound();
            }
            //return View(reIssueTable);
            return PartialView("Delete", reIssueTable);
        }

        /// <summary>
        /// POST: Delete - delete record from the database and close modal window
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckSessionTimeOut]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Guid passID = (Guid)Session["ActiveGUID"];
            ReIssueTable reIssueTable = db.ReIssueTable.Find(id);
            db.ReIssueTable.Remove(reIssueTable);
            // Remove comments to enable logging
            try
            {
                int userID;
                if (int.TryParse(Session["UserID"].ToString(), out userID))
                    LogHelper.AddLog(DateTime.Now, "ReIssueTable | Delete", $"Time:{reIssueTable.Time} Who:{reIssueTable.User} Where:{reIssueTable.Objective} BodyNum:{reIssueTable.BodyNum}", userID);
            }
            catch { }
            db.SaveChanges();
            return Json(new { success = true });
        }

        /// <summary>
        /// AJAX: GetWho - get data for autocomplete input from the database column "User"
        /// </summary>
        /// <param name="term">Term from autocomplete input</param>
        /// <param name="cnt">Number of displayed results</param>
        /// <returns>Strings of "cnt" from searchable "term"</returns>
        public JsonResult GetWho(string term, int cnt)
        {
            var data = db.ReIssueTable.Select(q => new
            {
                User = q.User
            }).Where(q=>q.User.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);
            return Json(data, JsonRequestBehavior.AllowGet);      
        }

        /// <summary>
        /// AJAX: GetWhat - get data for autocomplete input from the database column "Objective"
        /// </summary>
        /// <param name="term">Term from autocomplete input</param>
        /// <param name="cnt">Number of displayed results</param>
        /// <returns>Strings of "cnt" from searchable "term"</returns>
        public JsonResult GetWhat(string term, int cnt)
        {
            var data = db.ReIssueTable.Select(q => new
            {
                Objective = q.Objective
            }).Where(q => q.Objective.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);
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
