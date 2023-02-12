using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    public class MembersTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: MembersTables
        public ActionResult _index()
        {
            return View(db.MembersTable.ToList());
        }

        // GET: MembersTables/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MembersTable membersTable = db.MembersTable.Find(id);
            if (membersTable == null)
            {
                return HttpNotFound();
            }
            return View(membersTable);
        }

        // GET: MembersTables/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MembersTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MemberID,FirstName,LastName,Email")] MembersTable membersTable)
        {
            if (ModelState.IsValid)
            {
                db.MembersTable.Add(membersTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(membersTable);
        }

        // GET: MembersTables/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MembersTable membersTable = db.MembersTable.Find(id);
            if (membersTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.data = db.PermisionTable.ToList();
            return View(membersTable);
        }

        // POST: MembersTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MemberID,FirstName,LastName,Email")] MembersTable membersTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(membersTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(membersTable);
        }

        // GET: MembersTables/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MembersTable membersTable = db.MembersTable.Find(id);
            if (membersTable == null)
            {
                return HttpNotFound();
            }
            return View(membersTable);
        }

        // POST: MembersTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MembersTable membersTable = db.MembersTable.Find(id);
            db.MembersTable.Remove(membersTable);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetMembers(string term)
        {
            var members = db.MembersTable.Select(q => new
            {
                Name = q.FirstName + " " + q.LastName,
                Id = q.MemberID
            }).Where(q => q.Name.Contains(term));
            return Json(members, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult GetMembers(int id)
        //{
        //    var members = db.MembersTable.Select(q => new
        //    {
        //        Name = q.FirstName + " " + q.LastName,
        //        Id = q.MemberID
        //    }).Where(q => q.Id == id);
        //    return Json(members, JsonRequestBehavior.AllowGet);
        //}


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
