using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    public class MembersTablesController : Controller
    {
        private readonly ReportDBEntities1 db = new ReportDBEntities1();

        // Add Member from AD to DB        
        public bool AddMember(ADHelper ad)
        {
            JSConsoleLog.ConsoleLog($"Function AddMember(int {ad.MemberID}, string {ad.MemberName}, string {ad.MemberEmail}");
            Logger.LogInfo($"Function AddMember(int {ad.MemberID}, string {ad.MemberName}, string {ad.MemberEmail}", "Project_REPORT_v7.Controllers.MembersTablesController.AddMember()");
            if (ad.MemberID > 0 && !string.IsNullOrEmpty(ad.MemberName) && !string.IsNullOrEmpty(ad.MemberEmail)) 
            {
                MembersTable member = new MembersTable
                {
                    MemberID = ad.MemberID,
                    Name = ad.MemberName,
                    Email = ad.MemberEmail
                };
                db.MembersTable.Add(member);
                db.SaveChanges();
                JSConsoleLog.ConsoleLog($"AddMember - User was successfuly");
                return true;
            }
            else
            {
                JSConsoleLog.ConsoleLog($"AddMember - Error in adding user");
                return false;
            }
        }

        public ActionResult Index()
        {
            var memberTable = db.MembersTable;
            return View(memberTable);
        }

        public ActionResult Details(int MemberID) 
        {
            if (MemberID ==  0)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            MembersTable member = db.MembersTable.Find(MemberID);

            ViewData["CurrentMember"] = MemberID;

            if (member == null)
            {
                return HttpNotFound();
            }

            return View(member);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            MembersTable member = db.MembersTable.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MemberID, Name, Email, ShiftID")] MembersTable membersTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(membersTable).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return View("Index");
        }

        // GET: /MembersTable/Delete/5
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            MembersTable membersTable = db.MembersTable.Find(id);
            if (membersTable == null)
                return HttpNotFound();
            return View("Delete", membersTable);
        }

        // POST: /MembersTable/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MembersTable membersTable = db.MembersTable.Find(id);
            db.MembersTable.Remove(membersTable);
            db.SaveChanges();
            return View("Index");
        }

        public bool CheckMember(int ID)
        {
            JSConsoleLog.ConsoleLog($"Checking user: {ID}");
            return db.MembersTable.Any(i => i.MemberID == ID);
        }

        [HttpPost]
        public JsonResult GetMembers(string term)
        {
            var members = db.MembersTable.Select(q => new
            {
                Name = q.Name,
                Id = q.MemberID
            }).Where(q => q.Name.Contains(term));
            return Json(members, JsonRequestBehavior.AllowGet);
        }

        public static List<SelectListItem> GetShifts()
        {
            ReportDBEntities1 sdb = new ReportDBEntities1();
            var ls = new List<SelectListItem>();

            var temp = sdb.ShiftTable.Select(s => new
            {
                Shift = s.ShiftName,
                ID = s.ShiftID
            }).ToList();

            ls = temp.ConvertAll(a =>
            {
                return new SelectListItem()
                {
                    Text = a.Shift,
                    Value = a.ID.ToString(),
                    Selected = false
                };
            });

            ls[0].Selected = true;

            return ls;
        }

        public JsonResult GetMembersByShiftID(string term)
        {
            //JSConsoleLog.ConsoleLog($"GetMembersByShiftID - Got term = {term}.");
            int id = 0;
            if (int.TryParse(term, out id))
            {
                Debug.WriteLine("Int.TryParse in GetMembersByShiftID success");
                //JSConsoleLog.ConsoleLog($"GetMembersByShiftID - Int.TryParse got int {id} from {term}.");
            }

            var members = db.MembersTable.Select(s => new
            {
                Name = s.Name,
                ID = s.MemberID,
                s.ShiftID
            }).Where(w => w.ShiftID == id);
            return Json(members, JsonRequestBehavior.AllowGet);
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
