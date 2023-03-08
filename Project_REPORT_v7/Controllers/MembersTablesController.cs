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

        // Add Member from AD to DB        
        public bool AddMember(int ID, string Name, string Email)
        {
            if (ID > 0 && string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Email)) 
            {
                MembersTable member = new MembersTable();
                member.MemberID = ID;
                member.Name = Name;
                member.Email = Email;
                db.MembersTable.Add(member);
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckMember(int ID)
        {
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
