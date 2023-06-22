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
            #region Check Session
            CheckSession();
            #endregion
            return View();
        }

        private void CheckSession(string username = "")
        {
            MembersTablesController member = new MembersTablesController();
            JSConsoleLog.ConsoleLog("User.Identity.Name: " + username != "" ? username : User.Identity.Name);
            Logger.LogInfo("User.Identity.Name: " + username != "" ? username : User.Identity.Name, "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
            ADHelper ad = new ADHelper(username != "" ? username : User.Identity.Name);
            JSConsoleLog.ConsoleLog( $"ADHelper initialed in CheckSession(): {ad.MemberName}, {ad.MemberID}, {ad.MemberEmail}" );
            Logger.LogInfo( $"ADHelper initialed in CheckSession(): {ad.MemberName}, {ad.MemberID}, {ad.MemberEmail}", "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
            if ( LDAPHelper.UserIsMemberOfGroups( username == "" ? User.Identity.Name : username, new string [] { "CCR_Report_Admin" } ) )
            {
                Session ["isAdmin"] = "Admin";
                Session ["Closed"] = "false";
                JSConsoleLog.ConsoleLog( $"Loged in as {ad.MemberName} [{ad.MemberID}] - Admin" );
                Logger.LogInfo( $"Loged in as {ad.MemberName} [{ad.MemberID}] - Admin", "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
            }
            else
            {
                Session ["isAdmin"] = "NonAdmin";
                JSConsoleLog.ConsoleLog( $"Loged in as {ad.MemberName} [{ad.MemberID}] - User" );
                Logger.LogInfo( $"Loged in as {ad.MemberName} [{ad.MemberID}] - User", "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
            }



            if ( !member.CheckMember( ad.MemberID ) )
            {
                JSConsoleLog.ConsoleLog( $"User ID {ad.MemberID} & User Name {ad.MemberName} is not in local DB" );
                Logger.LogInfo( $"User ID {ad.MemberID} & User Name {ad.MemberName} is not in local DB", "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
                if ( member.AddMember( ad ) )
                {
                    JSConsoleLog.ConsoleLog( $"User ID {ad.MemberID} & User Name {ad.MemberName} was added to local DB" );
                    Logger.LogInfo( $"User ID {ad.MemberID} & User Name {ad.MemberName} was added to local DB", "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
                    Session ["User"] = $"{ad.MemberName} [{ad.MemberID}]";
                    Session ["UserID"] = ad.MemberID;
                }
                else
                {
                    JSConsoleLog.ConsoleLog( $"User wasn't added to DB." );
                    Logger.LogInfo( $"User wasn't added to DB.", "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
                    Session ["User"] = "Unknown";
                    Session ["UserID"] = 99999999;
                }
            }
            else
            {
                Session ["User"] = $"{ad.MemberName} [{ad.MemberID}]";
                Session ["UserID"] = ad.MemberID;
            }
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
                Session ["LoggedUser"] = $"User {IDLogin} logged from {returnUrl}";

                Logger.LogInfo( $"IDLogin - {IDLogin}", "Project_REPORT_v7.Controllers.HomeController.[POST]Login()" );
                //FormsAuthentication.SetAuthCookie(IDLogin, true);
                if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length> 1 && (returnUrl.StartsWith("/") || (returnUrl.StartsWith("%2f"))) && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    Session ["ReturnURL"] = "Return URL: " + returnUrl;
                    Logger.LogInfo( $"returnUrl - {returnUrl}", "Project_REPORT_v7.Controllers.HomeController.[POST]Login()" );
                    CheckSession(IDLogin);
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