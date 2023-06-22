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
                        Logger.LogInfo("SessionCookie: " + sessionCookie, "Project_REPORT_v7.Controllers.Addon.CheckSessionTimeOutAttribute.OnActionExecuted()");
                        FormsAuthentication.SignOut();
                        string redirectTo = "~/Home/Index";
                        if (!string.IsNullOrEmpty(context.Request.RawUrl))
                        {
                            redirectTo = string.Format("~/Home/Login?returnUrl=[0]", HttpUtility.UrlEncode(context.Request.RawUrl));
                            Logger.LogInfo("redirectTo: " + redirectTo, "Project_REPORT_v7.Controllers.Addon.CheckSessionTimeOutAttribute.OnActionExecuted()");
                        }
                        MembersTablesController member = new MembersTablesController();
                        ADHelper ad = new ADHelper(context.User.Identity.Name);
                        if ( LDAPHelper.UserIsMemberOfGroups( context.User.Identity.Name, new string [] { "CCR_Report_Admin" } ) )
                        {
                            context.Session ["isAdmin"] = "Admin";
                            context.Session ["Closed"] = "false";
                            JSConsoleLog.ConsoleLog( $"Loged in as admin {ad.MemberName} [{ad.MemberID}]" );
                            Logger.LogInfo("Loged in as " + ad.MemberName + " [" + ad.MemberID + "]", "Project_REPORT_v7.Controllers.Addon.CheckSessionTimeOutAttribute.OnActionExecuted()");
                        }
                        else
                        {
                            context.Session ["isAdmin"] = "NonAdmin";
                            JSConsoleLog.ConsoleLog( $"Loged in as nonadmin {ad.MemberName} [{ad.MemberID}]" );
                            Logger.LogInfo("Loged in as " + ad.MemberName + " [" + ad.MemberID + "]", "Project_REPORT_v7.Controllers.Addon.CheckSessionTimeOutAttribute.OnActionExecuted()");
                        }

                        if ( !member.CheckMember( ad.MemberID ) )
                        {
                            JSConsoleLog.ConsoleLog( $"User ID {ad.MemberID} & User Name {ad.MemberName} is not in local DB" );
                            Logger.LogInfo("User ID " + ad.MemberID + " & User Name " + ad.MemberName + " is not in local DB", "Project_REPORT_v7.Controllers.Addon.CheckSessionTimeOutAttribute.OnActionExecuted()");
                            if ( member.AddMember( ad ) )
                            {
                                context.Session ["User"] = $"{ad.MemberName} [{ad.MemberID}]";
                                context.Session ["UserID"] = ad.MemberID;
                            }
                            else
                            {
                                JSConsoleLog.ConsoleLog( $"User wasn't added to DB." );
                                Logger.LogInfo("User wasn't added to DB.", "Project_REPORT_v7.Controllers.Addon.CheckSessionTimeOutAttribute.OnActionExecuted()");
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
                        Logger.LogInfo("Logged user " + ad.MemberName + ", ID:" + ad.MemberID, "Project_REPORT_v7.Controllers.Addon.CheckSessionTimeOutAttribute.OnActionExecuted()");
                        filterContext.HttpContext.Response.Redirect(redirectTo, true);
                    }
                }
            }
            base.OnActionExecuted(filterContext);
        }
    }
}