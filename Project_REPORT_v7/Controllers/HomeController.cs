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
                                // Temperaly non active code for future use
            //if (Session["MemberID"] != null)
            //    return View();
            //else
            //    return RedirectToAction("Login");
            //string[] separator = new string[] { "\\" };
            //var sep = User.Identity.Name.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            //Session["User"] = sep[1];
            return View();
        }

                                // Temperaly non active code for future use

        //public ActionResult Login()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login(string memberID, string password)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var pass = GetMD5(password);
        //        int id = int.Parse(memberID.ToString());
        //        var data = db.MembersTable.Where(s => s.MemberID.Equals(id) && s.Password.Equals(pass)).ToList();
        //        if (data.Count() > 0)
        //        {
        //            Session["FullName"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
        //            Session["Email"] = data.FirstOrDefault().Email;
        //            Session["MemberID"] = data.FirstOrDefault().MemberID;
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            ViewBag.Error = "Login failed!";
        //            return RedirectToAction("Login");
        //        }
        //    }
        //    return View();
        //}

        //public ActionResult Logout()
        //{
        //    Session.Clear();
        //    return RedirectToAction("Login");
        //}

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