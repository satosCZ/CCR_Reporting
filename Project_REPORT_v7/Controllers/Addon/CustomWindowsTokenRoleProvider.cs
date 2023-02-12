using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Security;

namespace Project_REPORT_v7.Controllers.Addon
{
    public class CustomWindowsTokenRoleProvider : WindowsTokenRoleProvider
    {
        public override string[] GetRolesForUser(string username)
        {
            // Will contain the list of roles that the user is a member of
            List<string> roles = null;

            // Create unique cache key for the user
            string key = string.Concat(username, ":", base.ApplicationName);

            // Get cache for current session
            Cache cache = HttpContext.Current.Cache;

            // Obtain cached roles for the user
            if (cache[key] != null)
            {
                roles = new List<string>(cache[key] as string[]);
            }

            // Was the list of roles for the user in cache?
            if (roles == null)
            {
                roles = new List<string>();

                // For each system role, determine if the user is a member of that role
                foreach (string role in AppSettings.APPLICATION_GROUPS)
                {
                    if (base.IsUserInRole(username, role))
                    {
                        roles.Add(role);
                    }
                }

                // Cache the roles for 1 month
                cache.Insert(key, roles.ToArray(), null, DateTime.Now.AddMonths(1), Cache.NoSlidingExpiration);
            }

            return roles.ToArray();
        }
    }
}