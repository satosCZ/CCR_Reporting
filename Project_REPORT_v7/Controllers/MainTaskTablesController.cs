﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PagedList;
using Project_REPORT_v7.App_Start;
using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    [AuthorizeAD(Groups = "CCR_Report")]
    public class MainTaskTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: MainTaskTables
        public PartialViewResult _index()
        {
            var mainTaskTable = db.MainTaskTable.Include(p => p.ReportTable);
            return PartialView(mainTaskTable.OrderBy(s => s.Time).ToList());
        }

        // GET: MainTaskTables/FilterIndex
        public ViewResult FilterIndex(int? page)
        {
            var mainTaskTable = db.MainTaskTable.Include(p => p.ReportTable);

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(mainTaskTable.OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time).ToPagedList(pageNumber, pageSize));
        }

        // POST : MainTaskTables/FilterIndex > Filter
        public ViewResult FilterIndexFilter(string filter, DateTime? fromDT, DateTime? toDT, int? page)
        {
            var mainTaskTable = db.MainTaskTable.Include(p => p.ReportTable);

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            if (filter == "GLOVIS" || filter == "TRANSYS")
            {
                var filtered = mainTaskTable.OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time).Where(w => w.Shop == filter);
                return View("FilterIndex", filtered.ToPagedList(pageNumber, pageSize));
            }

            if ((fromDT != null && toDT != null))
            {
                var filteredDate = mainTaskTable.Where(w => w.ReportTable.Date >= fromDT && w.ReportTable.Date <= toDT).OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time);
                return View("FilterIndex", filteredDate.ToPagedList(pageNumber, pageSize));
            }
            else if ((fromDT != null && toDT == null))
            {
                var filteredDate = mainTaskTable.Where(w => w.ReportTable.Date >= fromDT).OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time);
                return View("FilterIndex", filteredDate.ToPagedList(pageNumber, pageSize));
            }
            else if ((fromDT != null && toDT == null))
            {
                var filteredDate = mainTaskTable.Where(w => w.ReportTable.Date <= toDT).OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time);
                return View("FilterIndex", filteredDate.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return View("FilterIndex", mainTaskTable.OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time));
            }
        }

        // GET: MainTaskTables/Create
        [AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("Create");
        }

        // POST: MainTaskTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MainTaskID,Time,Duration,Shop,System,Problem,Solution,Cooperation,ReportID")] MainTaskTable mainTaskTable)
        {
            Guid passID = (Guid)TempData["ActiveGUID"];
            if (ModelState.IsValid)
            {
                mainTaskTable.MainTaskID = Guid.NewGuid();
                mainTaskTable.ReportID = passID;
                db.MainTaskTable.Add(mainTaskTable);
                // Remove comments to enable logging

                //int userID;
                //if (int.TryParse(Session["User"].ToString(), out userID))
                //    LogClass.AddLog(DateTime.Now, "MainTaskTable|Create", $"Created new Main task, Time:{mainTaskTable.Time} Duration:{mainTaskTable.Duration} Shop:{mainTaskTable.Shop} System:{mainTaskTable.System}, Problem:{mainTaskTable.Problem} Solution:{mainTaskTable.Solution} Cooperation:{mainTaskTable.Cooperation}", userID);
                db.SaveChanges();
                return Json(new { success = true });
            }

            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", mainTaskTable.ReportID);
            return Json(mainTaskTable, JsonRequestBehavior.AllowGet);
        }

        // GET: MainTaskTables/Edit/5
        [AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MainTaskTable mainTaskTable = db.MainTaskTable.Find(id);
            if (mainTaskTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", mainTaskTable.ReportID);
            return PartialView(mainTaskTable);
        }

        // POST: MainTaskTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MainTaskID,Time,Duration,Shop,System,Problem,Solution,Cooperation,ReportID")] MainTaskTable mainTaskTable)
        {
            Guid passID = (Guid)TempData["ActiveGUID"];
            if (ModelState.IsValid)
            {
                mainTaskTable.ReportID = passID;
                db.Entry(mainTaskTable).State = EntityState.Modified;
                // Remove comments to enable logging

                //int userID;
                //if (int.TryParse(Session["User"].ToString(), out userID))
                //    LogClass.AddLog(DateTime.Now, "MainTaskTable|Edit", $"Edited Main task, Time:{mainTaskTable.Time} Duration:{mainTaskTable.Duration} Shop:{mainTaskTable.Shop} System:{mainTaskTable.System}, Problem:{mainTaskTable.Problem} Solution:{mainTaskTable.Solution} Cooperation:{mainTaskTable.Cooperation}", userID);
                db.SaveChanges();
                return Json(new { success = true });
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", mainTaskTable.ReportID);
            return PartialView(new { success = true });
        }

        // GET: MainTaskTables/Delete/5
        [AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpGet]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MainTaskTable mainTaskTable = db.MainTaskTable.Find(id);
            if (mainTaskTable == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", mainTaskTable);
        }

        // POST: MainTaskTables/Delete/5
        [AuthorizeAD(Groups = "CCR_Report_Control")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            MainTaskTable mainTaskTable = db.MainTaskTable.Find(id);
            db.MainTaskTable.Remove(mainTaskTable);
            // Remove comments to enable logging

            //int userID;
            //if (int.TryParse(Session["User"].ToString(), out userID))
            //    LogClass.AddLog(DateTime.Now, "MainTaskTable|Delete", $"Deleted Main task, Time:{mainTaskTable.Time} Duration:{mainTaskTable.Duration} Shop:{mainTaskTable.Shop} System:{mainTaskTable.System}, Problem:{mainTaskTable.Problem} Solution:{mainTaskTable.Solution} Cooperation:{mainTaskTable.Cooperation}", userID);
            db.SaveChanges();
            return Json(new { success = true });
        }

        public JsonResult GetSystem(string term)
        {
            var check = db.MainTaskTable.Select(q => new
            {
                System = q.System
            }).Where(
                q => q.System.Contains(term)).Distinct();
            return Json(check, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProblem(string term)
        {
            var check = db.MainTaskTable.Select(q => new
            {
                Problem = q.Problem
            }).Where(
                q => q.Problem.Contains(term)).Distinct();
            return Json(check, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSolution(string term)
        {
            var check = db.MainTaskTable.Select(q => new
            {
                Solution = q.Solution
            }).Where(
                q => q.Solution.Contains(term)).Distinct();
            return Json(check, JsonRequestBehavior.AllowGet);
        }

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
            else if(temp.Count == 0)
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
