using Project_REPORT_v7.App_Start;
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
    //[GroupAuthorize("ITMesAdmin", "ITMesTechnician", "ITHaeczMesSection")]
    public class HomeController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        public ActionResult Index()
        {
            MembersTablesController member = new MembersTablesController();
                                // Temperaly non active code for future use
            //if (Session["MemberID"] != null)
            //    return View();
            //else
            //    return RedirectToAction("Login");
            //string[] separator = new string[] { "\\" };
            //var sep = User.Identity.Name.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            //Session["User"] = sep[1];
            if (member.AddMember())
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