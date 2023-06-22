using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Web;
using System.Web.Hosting;

namespace Project_REPORT_v7.Controllers.Addon
{
    public static class LDAPHelper
    {
        public static string GetLDAPContainer()
        {
            Uri ldapUri;
            ParseLDAPConnectionString(out ldapUri);
            Logger.LogInfo( $"ldapUri.PathAndQuery - {ldapUri.PathAndQuery}", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.GetLDAPContainer()" );

            return HttpUtility.UrlDecode(ldapUri.PathAndQuery.TrimStart('/'));
        }

        public static string GetLDAPHost()
        {
            Uri ldapUri;
            ParseLDAPConnectionString(out ldapUri);

            Logger.LogInfo( $"ldapUri.Host - {ldapUri.Host}", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.GetLDAPHost()" );
            return ldapUri.Host;
        }

        public static bool ParseLDAPConnectionString(out Uri ldapUri)
        {
            string connString = ConfigurationManager.ConnectionStrings["ADConnectionString"].ConnectionString;

            Logger.LogInfo( $"connString - {connString}", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.ParseLDAPConnectionString()" );
            return Uri.TryCreate(connString, UriKind.Absolute, out ldapUri);
        }

        public static bool UserIsMemberOfGroups(string username, string[] groups)
        {
            // Return true immediately if the authorization is not locked down to any particular AD group
            if (groups == null || groups.Length == 0)
            {
                Logger.LogInfo( $"groups == null", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.UserIsMemberOfGroups()" );
                return true;
            }

            // Verify that the user is in the given AD group (if any)
            using (var context = BuildPrincipalContext())
            {
                //JSConsoleLog.ConsoleLog($"UserIsMemberOfGroups - {username}");
                Logger.LogInfo($"username - {username}", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.UserIsMemberOfGroups()");
                var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);

                Logger.LogInfo( $"userPrincipal.UserPrincipalName - {userPrincipal.UserPrincipalName}", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.UserIsMemberOfGroups()" );
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

            Logger.LogInfo( $"container - {container}", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.BuildPrincipalContext()" );
            return new PrincipalContext(ContextType.Domain, null, container);
        }
    }
}