﻿using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Project_REPORT_v7.Controllers
{

    public class HomeController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Index()
        {
            MembersTablesController member = new MembersTablesController();
            //ADHelper ad = new ADHelper(User.Identity.Name);
            //if (LDAPHelper.UserIsMemberOfGroups(User.Identity.Name, new string[] {"CCR_Report_Admin"}))
            //{
            //    Session["isAdmin"] = "Admin";
            //}
            //else
            //{
            //    Session["isAdmin"] = "NonAdmin";
            //}
            Session["isAdmin"] = "Admin";
            Session["User"] = HttpContext.User.Identity.Name.ToString();
            //if (!member.CheckMember(ad.MemberID))
            //{
            //    if (member.AddMember(ad.MemberID, ad.MemberName, ad.MemberEmail))
            //    {
            //        Session["User"] = ad.MemberName;
            //    }
            //    else
            //    {
            //        Session["User"] = "Unknown";
            //    }
            //}
            return View();
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Filtered()
        {
            return View("Filtered");
        }

        [AuthorizeAD(Groups = "CCR_Report_Admin")]
        public ActionResult Information()
        {
            return View("Information");
        }
    }
}