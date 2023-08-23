using System;
using System.Web;
using System.Web.Mvc;

namespace Project_REPORT_v7.Controllers.Addon
{
    /// <summary>
    /// AuthorizeADAttribute class for Active Directory authorization. Used class from StackOverflow
    /// </summary>
    public class AuthorizeADAttribute : AuthorizeAttribute
    {
        // Private variables
        private bool _authenticated;
        private bool _authorized;

        // Public property
        public string Groups { get; set; }

        /// <summary>
        /// Handle unauthorized request method for these that are authenticated but not authorized
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            if (_authenticated && !_authorized)
            {
                filterContext.Result = new RedirectResult("/Error/Error302");
            }
        }

        /// <summary>
        /// AuthorizeCore method for Active Directory authorization
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            _authenticated = base.AuthorizeCore(httpContext);
            if (_authenticated)
            {
                if (string.IsNullOrEmpty(Groups))
                {
                    _authorized = true;
                    return _authorized;
                }

                var groups = Groups.Split(',');

                // get the user name from the requested user (after logging in)
                string username = httpContext.User.Identity.Name;
                try
                {
                    // check if the user is a member of the specified groups
                    _authorized = LDAPHelper.UserIsMemberOfGroups(username, groups);
                    return _authorized;
                }
                catch (Exception ex)
                {
                    //this.Log().Error(() => "Error attempting to authorize user", ex);
                    JSConsoleLog.ConsoleLog("Error attempting to authorize user: " + ex.Message);
                    LogHelper.AddLog(DateTime.Now, "AuthorizeAD | Error","Error attempting to authorize user: " + ex.Message, 0);
                    Logger.LogError("Error attempting to authorize user: " + ex.Message, "Project_REPORT_v7.Controllers.Addon.AuthorizeADAttribute.AuthorizeCore(HttpContextBase httpContext)");
                    _authorized = false;
                    return _authorized;
                }
            }

            _authorized = false;
            return _authorized;
        }
    }
}