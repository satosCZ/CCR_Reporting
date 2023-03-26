using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Web;

namespace Project_REPORT_v7.Controllers.Addon
{
    public static class LDAPHelper
    {
        public static string GetLDAPContainer()
        {
            Uri ldapUri;
            ParseLDAPConnectionString(out ldapUri);

            return HttpUtility.UrlDecode(ldapUri.PathAndQuery.TrimStart('/'));
        }

        public static string GetLDAPHost()
        {
            Uri ldapUri;
            ParseLDAPConnectionString(out ldapUri);

            return ldapUri.Host;
        }

        public static bool ParseLDAPConnectionString(out Uri ldapUri)
        {
            string connString = ConfigurationManager.ConnectionStrings["ADConnectionString"].ConnectionString;
            return Uri.TryCreate(connString, UriKind.Absolute, out ldapUri);
        }

        public static bool UserIsMemberOfGroups(string username, string[] groups)
        {
            // Return true immediately if the authorization is not locked down to any particular AD group
            if (groups == null || groups.Length == 0)
            {
                return true;
            }

            // Verify that the user is in the given AD group (if any)
            using (var context = BuildPrincipalContext())
            {
                var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);

                foreach (var group in groups)
                {
                    if (userPrincipal.IsMemberOf(context, IdentityType.Name, group))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static PrincipalContext BuildPrincipalContext()
        {
            string container = LDAPHelper.GetLDAPContainer();
            return new PrincipalContext(ContextType.Domain, null, container);
        }
    }
}