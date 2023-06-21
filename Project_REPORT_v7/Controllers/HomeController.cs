using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;
using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace Project_REPORT_v7.Controllers
{

    public class HomeController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Index()
        {
            MembersTablesController member = new MembersTablesController();
            ADHelper ad = new ADHelper(User.Identity.Name);
            if (LDAPHelper.UserIsMemberOfGroups(User.Identity.Name, new string[] { "CCR_Report_Admin" }))
            {
                Session["isAdmin"] = "Admin";
                Session["Closed"] = "false";
                JSConsoleLog.ConsoleLog( $"Loged in as {ad.MemberName} [{ad.MemberID}]" );
            }
            else
            {
                Session["isAdmin"] = "NonAdmin";
                JSConsoleLog.ConsoleLog( $"Loged in as {ad.MemberName} [{ad.MemberID}]" );
            }

            

            if (!member.CheckMember(ad.MemberID))
            {
                JSConsoleLog.ConsoleLog($"User ID {ad.MemberID} & User Name {ad.MemberName} is not in local DB");
                if (member.AddMember(ad))
                {
                    Session["User"] = $"{ad.MemberName} [{ad.MemberID}]";
                    Session["UserID"] = ad.MemberID;
                }
                else
                {
                    JSConsoleLog.ConsoleLog($"User wasn't added to DB.");
                    Session["User"] = "Unknown";
                    Session["UserID"] = 99999999;
                }
            }
            else
            {
                Session["User"] = $"{ad.MemberName} [{ad.MemberID}]";
                Session["UserID"] = ad.MemberID;
            }

            JSConsoleLog.ConsoleLog( ViewBag.ReturnURL );
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string IDLogin, string Password, string returnUrl) 
        {
            if (Membership.ValidateUser(IDLogin, Password))
            {
                ViewBag.LoggedUser = $"User {IDLogin} logged from {returnUrl}";
                FormsAuthentication.SetAuthCookie(IDLogin, true);
                if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length> 1 && (returnUrl.StartsWith("/") || (returnUrl.StartsWith("%2f"))) && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    ViewBag.ReturnURL = "Return URL: " + returnUrl;
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult Logout()
        {
            HttpCookie cookie = new HttpCookie("ASP.NET_SessionId", "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            HttpCookie httpCookie = new HttpCookie(sessionStateSection.CookieName, "");
            httpCookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(httpCookie);

            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Home");
        }

        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Filtered()
        {
            return View("Filtered");
        }

        [AuthorizeAD(Groups = "CCR_Report_Admin")]
        public ActionResult Information()
        {
            return PartialView("Information");
        }
    }
}