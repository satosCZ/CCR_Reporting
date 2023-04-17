using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;
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
                    Session["UserID"] = ad.MemberID;
                }
                else
                {
                    Session["User"] = "Unknown";
                    Session["UserID"] = 99999999;
                }
            }
            else
            {
                Session["User"] = $"{ad.MemberName} [{ad.MemberID}]";
                Session["UserID"] = ad.MemberID;
            }
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
                FormsAuthentication.SetAuthCookie(IDLogin, true);
                if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length> 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

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