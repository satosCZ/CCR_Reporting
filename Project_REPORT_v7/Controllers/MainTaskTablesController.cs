using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PagedList;
using Project_REPORT_v7.App_Start;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    public class MainTaskTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: MainTaskTables
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician", "ITHaeczMesSection")]
        public PartialViewResult _index()
        {
            var mainTaskTable = db.MainTaskTable.Include(p => p.ReportTable);
            return PartialView(mainTaskTable.OrderBy(s => s.Time).ToList());
        }

        // GET: MainTaskTables/FilterIndex
        //[ChildActionOnly]
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician", "ITHaeczMesSection")]
        public PartialViewResult FilterIndex(string filterMT, DateTime? mtFromDT, DateTime? mtToDT, string mtFultex, int? mtPage)
        {
            var mainTaskTable = db.MainTaskTable.Include(p => p.ReportTable);

            IOrderedQueryable<MainTaskTable> filtered;

            int pageSize = 20;
            int pageNumber = (mtPage ?? 1);

            DateTime from = mtFromDT.GetValueOrDefault();
            DateTime to = mtToDT.GetValueOrDefault().AddDays(1);

            // Filter by company
            if (filterMT == "GLOVIS" || filterMT == "TRANSYS")
            {
                filtered = mainTaskTable.Where(w => w.Shop == filterMT).OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time);
            }
            else
            {
                filtered = mainTaskTable.OrderByDescending(s => s.ReportTable.Date).ThenBy(s => s.Time);
            }

            if (mtFromDT != null && mtToDT != null)
            {
                filtered = filtered.Where(w => w.ReportTable.Date >= from && w.ReportTable.Date <= to).OrderByDescending(o => o.ReportTable.Date).ThenBy(t => t.Time);
            }
            else if (mtFromDT != null && mtToDT == null)
            {
                filtered = filtered.Where(w => w.ReportTable.Date >= from).OrderByDescending(o => o.ReportTable.Date).ThenBy(t => t.Time);
            }
            else if (mtFromDT == null && mtToDT != null)
            {
                filtered = filtered.Where(w => w.ReportTable.Date <= to).OrderByDescending(o => o.ReportTable.Date).ThenBy(t => t.Time);
            }

            if (!string.IsNullOrEmpty(mtFultex))
            {
                filtered = filtered.Where(w => w.System.Contains(mtFultex) || w.Problem.Contains(mtFultex) || w.Solution.Contains(mtFultex)).OrderByDescending(o => o.ReportTable.Date).ThenBy(t => t.Time);
            }

            return PartialView("FilterIndex", filtered.ToPagedList(pageNumber, pageSize));
        }

        // GET: MainTaskTables/Create
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("Create");
        }

        // POST: MainTaskTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "MainTaskID,Time,Duration,Shop,System,Problem,Solution,Cooperation,ReportID")] MainTaskTable mainTaskTable)
        {
            try
            {
                Guid passID = (Guid)TempData["ActiveGUID"];
                if (ModelState.IsValid)
                {
                    mainTaskTable.MainTaskID = Guid.NewGuid();
                    mainTaskTable.ReportID = passID;
                    db.MainTaskTable.Add(mainTaskTable);
                    //int userID;
                    //if (int.TryParse(Session["User"].ToString(), out userID))
                    //    LogClass.AddLog(DateTime.Now, "MainTaskTable|Create", $"Created new Main task, Time:{mainTaskTable.Time} Duration:{mainTaskTable.Duration} Shop:{mainTaskTable.Shop} System:{mainTaskTable.System}, Problem:{mainTaskTable.Problem} Solution:{mainTaskTable.Solution} Cooperation:{mainTaskTable.Cooperation}", userID);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }

            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", mainTaskTable.ReportID);
            return Json(new { success = false });
        }

        // GET: MainTaskTables/Edit/5
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
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
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MainTaskID,Time,Duration,Shop,System,Problem,Solution,Cooperation,ReportID")] MainTaskTable mainTaskTable)
        {
            Guid passID = (Guid)TempData["ActiveGUID"];
            if (ModelState.IsValid)
            {
                mainTaskTable.ReportID = passID;
                db.Entry(mainTaskTable).State = EntityState.Modified;
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
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
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
        //[GroupAuthorize("ITMesAdmin", "ITMesTechnician")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            MainTaskTable mainTaskTable = db.MainTaskTable.Find(id);
            db.MainTaskTable.Remove(mainTaskTable);
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

            ls = temp.ConvertAll(a =>
            {
                return new SelectListItem()
                {
                    Text = a.Shop.ToString(),
                    Value = a.Shop.ToString(),
                    Selected = false
                };
            });

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
