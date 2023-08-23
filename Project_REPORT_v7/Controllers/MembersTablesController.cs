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
    /// <summary>
    /// MembersTablesController is controller for MembersTable model.
    /// </summary>
    [CheckSessionTimeOut]
    public class MembersTablesController : Controller
    {
        // Private variable for database connection
        private readonly ReportDBEntities1 db = new ReportDBEntities1();

        /// <summary>
        /// Function AddMember is used for adding new member to database.
        /// </summary>
        /// <param name="ad"></param>
        /// <returns></returns>
        public bool AddMember(ADHelper ad)
        {
            Logger.LogInfo($"Function AddMember(int {ad.MemberID}, string {ad.MemberName}, string {ad.MemberEmail}", "Project_REPORT_v7.Controllers.MembersTablesController.AddMember()");

            // check if MemberID is not 0 and MemberName and MemberEmail are not empty
            if (ad.MemberID > 0 && !string.IsNullOrEmpty(ad.MemberName) && !string.IsNullOrEmpty(ad.MemberEmail)) 
            {
                // create new member and add it to database
                MembersTable member = new MembersTable
                {
                    MemberID = ad.MemberID,
                    Name = ad.MemberName,
                    Email = ad.MemberEmail
                };
                db.MembersTable.Add(member);
                db.SaveChanges();
                Logger.LogInfo($"AddMember - User was successfuly", "Project_REPORT_v7.Controllers.MembersTablesController.AddMember()");
                return true;
            }
            else
            {
                Logger.LogInfo($"AddMember - Error in adding user", "Project_REPORT_v7.Controllers.MembersTablesController.AddMember()");
                return false;
            }
        }

        /// <summary>
        /// GET: Index page for MembersTable
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var memberTable = db.MembersTable;
            return View(memberTable);
        }

        /// <summary>
        /// GET: Details page for MembersTable
        /// </summary>
        /// <param name="MemberID">Integer: Member ID same as in LDAP</param>
        /// <returns>Show details page with data saved in DB about said member</returns>
        public ActionResult Details(int MemberID) 
        {
            // check if MemberID is not 0
            if (MemberID ==  0)
            {
                // if MemberID is 0, return bad request
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            // find member by MemberID
            MembersTable member = db.MembersTable.Find(MemberID);

            // set ViewData for current member
            ViewData["CurrentMember"] = MemberID;

            // check if member is not null
            if (member == null)
            {
                // if member is null, return HttpNotFound
                return HttpNotFound();
            }

            // return view with member
            return View(member);
        }

        /// <summary>
        /// GET: Edit page for MembersTable
        /// </summary>
        /// <param name="id">Integer: Member ID same as in LDAP</param>
        /// <returns>Show new page with editing possible</returns>
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

        /// <summary>
        /// POST: Edit page for MembersTable
        /// </summary>
        /// <param name="membersTable">MembersTable: parameters from inputs assigned by their HTML ID</param>
        /// <returns>Index page for MembersTable</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MemberID, Name, Email, ShiftID")] MembersTable membersTable)
        {
            // check if model is valid
            if (ModelState.IsValid)
            {
                // set state of membersTable to modified
                db.Entry(membersTable).State = System.Data.Entity.EntityState.Modified;

                // save changes to database
                db.SaveChanges();
            }
            // return index page
            return View("Index");
        }

        /// <summary>
        /// GET: Delete page for MembersTable
        /// </summary>
        /// <param name="id">Integer: id for row id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            // check if id is null
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

        /// <summary>
        /// Check if member exists in database
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool CheckMember(int ID)
        {
            JSConsoleLog.ConsoleLog($"Checking user: {ID}");
            return db.MembersTable.Any(i => i.MemberID == ID);
        }

        /// <summary>
        /// POST: GetMembers for autocomplete various pages. Accessible by AJAX from any pages
        /// </summary>
        /// <param name="term">String: Name of member - not casesensitive</param>
        /// <returns>Return objects with Name and ID</returns>
        [HttpPost]
        public JsonResult GetMembers(string term)
        {
            var members = db.MembersTable.Select(q => new
            {
                Name = q.Name,
                Id = q.MemberID
            }).Where(q => q.Name.ToLower().Contains(term.ToLower()));
            return Json(members, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all shifts from database
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Get members by their shift, if shift is not specified, show members from shift 1 = A
        /// </summary>
        /// <param name="term">string of 1,2,3 identifiers for shift</param>
        /// <returns></returns>
        public JsonResult GetMembersByShiftID(string term)
        {
            int id = 0;

            // int.TryParse is fastest converter from string to int. If convert was succesful, get members else show only members from shift 1 = A
            if ( int.TryParse( term, out id ) )
            {
                var members = db.MembersTable.Select(s => new
                {
                    Name = s.Name,
                    ID = s.MemberID,
                    s.ShiftID
                }).Where(w => w.ShiftID == id);
                return Json( members, JsonRequestBehavior.AllowGet );
            }
            else
            {
                var members = db.MembersTable.Select(s => new
                {
                    Name = s.Name,
                    ID = s.MemberID,
                    s.ShiftID
                }).Where(w => w.ShiftID == 1);
                return Json( members, JsonRequestBehavior.AllowGet );
            }
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
