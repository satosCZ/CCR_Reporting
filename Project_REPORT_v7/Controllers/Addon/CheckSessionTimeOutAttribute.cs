using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Project_REPORT_v7.Controllers.Addon
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckSessionTimeOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var context = filterContext.HttpContext;
            if (context.Session != null)
            {
                if (context.Session.IsNewSession)
                {
                    string sessionCookie = context.Request.Headers["Cookie"];
                    if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET&#95;SessionId") >= 0))
                    {
                        FormsAuthentication.SignOut();
                        string redirectTo = "~/Home/Index";
                        if (!string.IsNullOrEmpty(context.Request.RawUrl))
                        {
                            redirectTo = string.Format("~/Home/Login?returnUrl=[0]", HttpUtility.UrlEncode(context.Request.RawUrl));
                        }
                        MembersTablesController member = new MembersTablesController();
                        ADHelper ad = new ADHelper(context.User.Identity.Name);
                        if ( LDAPHelper.UserIsMemberOfGroups( context.User.Identity.Name, new string [] { "CCR_Report_Admin" } ) )
                        {
                            context.Session ["isAdmin"] = "Admin";
                            context.Session ["Closed"] = "false";
                            JSConsoleLog.ConsoleLog( $"Loged in as {ad.MemberName} [{ad.MemberID}]" );
                        }
                        else
                        {
                            context.Session ["isAdmin"] = "NonAdmin";
                            JSConsoleLog.ConsoleLog( $"Loged in as {ad.MemberName} [{ad.MemberID}]" );
                        }

                        if ( !member.CheckMember( ad.MemberID ) )
                        {
                            JSConsoleLog.ConsoleLog( $"User ID {ad.MemberID} & User Name {ad.MemberName} is not in local DB" );
                            if ( member.AddMember( ad ) )
                            {
                                context.Session ["User"] = $"{ad.MemberName} [{ad.MemberID}]";
                                context.Session ["UserID"] = ad.MemberID;
                            }
                            else
                            {
                                JSConsoleLog.ConsoleLog( $"User wasn't added to DB." );
                                context.Session ["User"] = "Unknown";
                                context.Session ["UserID"] = 99999999;
                            }
                        }
                        else
                        {
                            context.Session ["User"] = $"{ad.MemberName} [{ad.MemberID}]";
                            context.Session ["UserID"] = ad.MemberID;
                        }
                        JSConsoleLog.ConsoleLog( $"Logged user {ad.MemberName}, ID:{ad.MemberID}" );
                        filterContext.HttpContext.Response.Redirect(redirectTo, true);
                    }
                }
            }
            base.OnActionExecuted(filterContext);
        }
    }
}