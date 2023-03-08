using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_REPORT_v7.Controllers.Addon
{
    public class AuthorizeADAttribute : AuthorizeAttribute
    {
        private bool _autenticated;
        private bool _authorized;

        public string Groups { get; set; }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            if (!_authorized && _autenticated)
            {
                filterContext.Result = new RedirectResult("/Home/NotAuthorized");
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            _autenticated =  base.AuthorizeCore(httpContext);

            if (_autenticated)
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
                    if (_authorized && groups.Contains("CCR_Report_Admin")) 
                    {

                    }
                    return _authorized;
                }
                catch
                {
                    _authorized = false;
                    return _authorized;
                }
            }

            _authorized = false;
            return _authorized;
        }
    }
}