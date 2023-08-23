using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;
using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace Project_REPORT_v7.Controllers
{
    /// <summary>
    /// Home Controller class derived from Controller
    /// </summary>
    [CheckSessionTimeOut]
    public class HomeController : Controller
    {
        // Private variable for DB connection
        private ReportDBEntities1 db = new ReportDBEntities1();

        /// <summary>
        /// Index action method GET with check session method call
        /// </summary>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Index()
        {
            #region Check Session
            CheckSession();
            #endregion
            return View();
        }

        /// <summary>
        /// Method to check session and add user to DB if not exist
        /// </summary>
        /// <param name="username"></param>
        private void CheckSession(string username = "")
        {
            // MembersTablesController variable to add user to DB or check if user exist in DB
            MembersTablesController member = new MembersTablesController();
            Logger.LogInfo("User.Identity.Name: " + username != "" ? username : User.Identity.Name, "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
            
            // ADHelper variable to get user info from AD
            ADHelper ad = new ADHelper(username != "" ? username : User.Identity.Name);
            Logger.LogInfo( $"ADHelper initialed in CheckSession(): {ad.MemberName}, {ad.MemberID}, {ad.MemberEmail}", "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
            
            // LDAPHelper to check if user is member of group
            if ( LDAPHelper.UserIsMemberOfGroups( username == "" ? User.Identity.Name : username, new string [] { "CCR_Report_Admin" } ) )
            {
                // Set session data for admin which open all reports
                Session ["isAdmin"] = "Admin";
                Session ["Closed"] = "false";
                Logger.LogInfo( $"Loged in as {ad.MemberName} [{ad.MemberID}] - Admin", "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
            }
            else
            {
                // Set session data for user which open only reports with status open
                Session ["isAdmin"] = "NonAdmin";
                Logger.LogInfo( $"Loged in as {ad.MemberName} [{ad.MemberID}] - User", "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
            }

            // Check if user exist in DB
            if ( !member.CheckMember( ad.MemberID ) )
            {
                Logger.LogInfo( $"User ID {ad.MemberID} & User Name {ad.MemberName} is not in local DB", "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
                
                // Add user to DB
                if ( member.AddMember( ad ) )
                {
                    Logger.LogInfo( $"User ID {ad.MemberID} & User Name {ad.MemberName} was added to local DB", "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
                    Session ["User"] = $"{ad.MemberName} [{ad.MemberID}]";
                    Session ["UserID"] = ad.MemberID;
                }

                // If user wasn't added to DB set session data to Unknown
                else
                {
                    Logger.LogInfo( $"User wasn't added to DB.", "Project_REPORT_v7.Controllers.HomeController.CheckSession()" );
                    Session ["User"] = "Unknown";
                    Session ["UserID"] = 99999999;
                }
            }
            // Set session data for user which exist in DB
            else
            {
                Session ["User"] = $"{ad.MemberName} [{ad.MemberID}]";
                Session ["UserID"] = ad.MemberID;
            }
        }

        /// <summary>
        /// Login action method GET
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Login action method POST with check session method call
        /// </summary>
        /// <param name="IDLogin">ID loggin same as LDAP login</param>
        /// <param name="Password">Password same as LDAP password</param>
        /// <param name="returnUrl">If login was from other page than homepage</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(string IDLogin, string Password, string returnUrl) 
        {
            // Check if user exist in AD and validate password
            if (Membership.ValidateUser(IDLogin, Password))
            {
                Logger.LogInfo( $"IDLogin - {IDLogin}", "Project_REPORT_v7.Controllers.HomeController.[POST]Login()" );
                // Set authentication cookie
                FormsAuthentication.SetAuthCookie(IDLogin, true);
                
                // Check returnUrl and redirect to it
                if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length> 1 && (returnUrl.StartsWith("/") || (returnUrl.StartsWith("%2f"))) && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    Logger.LogInfo( $"returnUrl - {returnUrl}", "Project_REPORT_v7.Controllers.HomeController.[POST]Login()" );
                    CheckSession(IDLogin);
                    return Redirect(returnUrl);
                }
                // Go to homepage
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        
        /// <summary>
        /// Logout action method POST with remove session data and authentication cookie
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Acion method GET to open page with filtered maintask and password tables
        /// </summary>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Filtered()
        {
            return View("Filtered");
        }

        /// <summary>
        /// Action method GET to open information page
        /// </summary>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin" )]
        public ActionResult Information()
        {
            return PartialView("Information");
        }
    }
}