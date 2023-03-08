using Project_REPORT_v7.App_Start;
using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Project_REPORT_v7.Controllers
{
    [AuthorizeAD(Groups = "CCR_Report")]
    public class HomeController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        public ActionResult Index()
        {
            MembersTablesController member = new MembersTablesController();
            ADHelper ad = new ADHelper(User.Identity.Name);
            if (LDAPHelper.UserIsMemberOfGroups(User.Identity.Name, new string[] {"CCR_Report_Admin"}))
            {
                Session["isAdmin"] = "Admin";
            }
            else
            {
                Session["isAdmin"] = "NonAdmin";
            }
            if (!member.CheckMember(ad.MemberID))
            {
                if (member.AddMember(ad.MemberID, ad.MemberName, ad.MemberEmail))
                {
                    Session["User"] = ad.MemberName;
                }
                else
                {
                    Session["User"] = "Unknown";
                }
            }
            return View();
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string b2s = null;
            for (int i = 0; i < targetData.Length; i++)
            {
                b2s += targetData[i].ToString("x2");
            }
            return b2s;
        }

        public ActionResult Filtered()
        {
            return View("Filtered");
        }
    }
}