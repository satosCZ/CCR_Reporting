using System;
using System.Web;
using System.Web.Mvc;

namespace Project_REPORT_v7.Controllers.Addon
{
    public class AuthorizeADAttribute : AuthorizeAttribute
    {
        private bool _authenticated;
        private bool _authorized;

        public string Groups { get; set; }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            if (_authenticated && !_authorized)
            {
                filterContext.Result = new RedirectResult("/Error/Error302");
            }
        }

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
                string username = httpContext.User.Identity.Name;
                try
                {
                    _authorized = LDAPHelper.UserIsMemberOfGroups(username, groups);
                    return _authorized;
                }
                catch (Exception ex)
                {
                    //this.Log().Error(() => "Error attempting to authorize user", ex);
                    _authorized = false;
                    return _authorized;
                }
            }

            _authorized = false;
            return _authorized;
        }
    }
}