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
    [CheckSessionTimeOut]
    public class ReIssueTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: ReIssueTables
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public PartialViewResult _index()
        {
            var reIssueTable = db.ReIssueTable.Include(p => p.ReportTable).OrderBy(s => s.Time);
            return PartialView(reIssueTable.ToList());
        }

        // GET: ReIssueTables/CreateMultiple
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult CreateMultiple()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("CreateMultiple");
        }

        // POST: ReIssueTables/CreateMultiple
        // To protect from overposting attacks, enable the specific properties you want to bind to, for more details se https://go.microsoft.com/fwlink/?LinkId=317598 .
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateMultiple(TimeSpan time, string user, string objective, string bodyNum)
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
                user = user.ToUpperCaps();
                objective = objective.ToUpperCaps();
                bodyNum = bodyNum.ToUpperCaps();
                if (bodyNum.Contains("\r\n"))
                {
                    string[] separators = new string[] { "\r\n" };
                    List<ReIssueTable> multiple = new List<ReIssueTable>();
                    List<string> bodys = bodyNum.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();
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
                    db.ReIssueTable.AddRange(multiple);

                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    ReIssueTable reIssueTable = new ReIssueTable();
                    reIssueTable.ReIssueID = Guid.NewGuid();
                    reIssueTable.ReportID = passID;
                    reIssueTable.Time = time;
                    reIssueTable.User= user.ToUpperCaps();
                    reIssueTable.Objective = objective.ToUpperCaps();
                    reIssueTable.BodyNum = bodyNum.ToUpperCaps();
                    db.ReIssueTable.Add(reIssueTable);
                    // Remove comments to enable logging

                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "ReIssueTable | Create", $"Time:{time} Who:{user} Where:{objective} BodyNum:{bodyNum}", userID);

                    db.SaveChanges();
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
            return Json(this, JsonRequestBehavior.AllowGet);
        }

        // GET: ReIssueTables/Edit/5
        //[AuthorizeAD(Groups = "CCR_Report_Control")]
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

        // POST: ReIssueTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReIssueID,Time,User,Objective,BodyNum,ReportID")] ReIssueTable reIssueTable)
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
                reIssueTable.ReportID = passID;
                reIssueTable.Time = reIssueTable.Time;
                reIssueTable.User = reIssueTable.User.ToUpperCaps();
                reIssueTable.Objective = reIssueTable.Objective.ToUpperCaps();
                reIssueTable.BodyNum = reIssueTable.BodyNum.ToUpperCaps();
                db.Entry(reIssueTable).State = EntityState.Modified;
                db.SaveChanges();
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "ReIssueTable | Edit", $"Time:{reIssueTable.Time} Who:{reIssueTable.User} Where:{reIssueTable.Objective} BodyNum:{reIssueTable.BodyNum}", userID);
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
                        LogHelper.AddLog(DateTime.Now, "ReIssueTable | Edit | Error", $"Time:{reIssueTable.Time} Who:{reIssueTable.User} Where:{reIssueTable.Objective} BodyNum:{reIssueTable.BodyNum}", userID);
                }
                catch { }
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", reIssueTable.ReportID);
            return PartialView("Edit", reIssueTable);
        }

        // GET: ReIssueTables/Delete/5
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

        // POST: ReIssueTables/Delete/5
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

        public JsonResult GetWho(string term, int cnt)
        {
            var data = db.ReIssueTable.Select(q => new
            {
                User = q.User
            }).Where(q=>q.User.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);
            return Json(data, JsonRequestBehavior.AllowGet);      
        }

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
