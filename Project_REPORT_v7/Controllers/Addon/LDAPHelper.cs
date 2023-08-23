using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Web;
using System.Web.Hosting;

namespace Project_REPORT_v7.Controllers.Addon
{
    /// <summary>
    /// LDAPHelper class - custom class that handle LDAP connection
    /// </summary>
    public static class LDAPHelper
    {
        /// <summary>
        /// Getting LDAP connection from web.config and return it as string decoded containing LDAP container
        /// </summary>
        /// <returns></returns>
        public static string GetLDAPContainer()
        {
            Uri ldapUri;
            ParseLDAPConnectionString(out ldapUri);
            Logger.LogInfo( $"ldapUri.PathAndQuery - {ldapUri.PathAndQuery}", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.GetLDAPContainer()" );

            return HttpUtility.UrlDecode(ldapUri.PathAndQuery.TrimStart('/'));
        }

        /// <summary>
        /// Getting LDAP connection from web.config and return it as string containing LDAP host
        /// </summary>
        /// <returns></returns>
        public static string GetLDAPHost()
        {
            Uri ldapUri;
            ParseLDAPConnectionString(out ldapUri);

            Logger.LogInfo( $"ldapUri.Host - {ldapUri.Host}", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.GetLDAPHost()" );
            return ldapUri.Host;
        }

        /// <summary>
        /// Get LDAP connection from web.config and return it as string containing LDAP connection and return true if Uri was created successfully
        /// </summary>
        /// <param name="ldapUri"></param>
        /// <returns></returns>
        public static bool ParseLDAPConnectionString(out Uri ldapUri)
        {
            string connString = ConfigurationManager.ConnectionStrings["ADConnectionString"].ConnectionString;

            Logger.LogInfo( $"connString - {connString}", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.ParseLDAPConnectionString()" );
            return Uri.TryCreate(connString, UriKind.Absolute, out ldapUri);
        }

        /// <summary>
        /// Check if user is member of group by going through all groups and return true if user is member of any group
        /// </summary>
        /// <param name="username">sAMAccount name</param>
        /// <param name="groups">Array of groups for check</param>
        /// <returns></returns>
        public static bool UserIsMemberOfGroups(string username, string[] groups)
        {
            // Return true immediately if the authorization is not locked down to any particular AD group
            if (groups == null || groups.Length == 0)
            {
                Logger.LogInfo( $"groups == null", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.UserIsMemberOfGroups()" );
                return true;
            }

            // Using LDAP connection to check if user is member of any group
            using (var context = BuildPrincipalContext())
            {
                Logger.LogInfo($"username - {username}", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.UserIsMemberOfGroups()");
                // Find user by sAMAccount name
                var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);

                Logger.LogInfo( $"userPrincipal.UserPrincipalName - {userPrincipal.UserPrincipalName}", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.UserIsMemberOfGroups()" );
                
                // Go through all groups and check if user is member of any group
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

        /// <summary>
        /// Method that will create PrincipalContext object with LDAP connection
        /// </summary>
        /// <returns></returns>
        public static PrincipalContext BuildPrincipalContext()
        {
            string container = LDAPHelper.GetLDAPContainer();

            Logger.LogInfo( $"container - {container}", "Project_REPORT_v7.Controllers.Addon.LDAPHelper.BuildPrincipalContext()" );
            return new PrincipalContext(ContextType.Domain, null, container);
        }
    }
}