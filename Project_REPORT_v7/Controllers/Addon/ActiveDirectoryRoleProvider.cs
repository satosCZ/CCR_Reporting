using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.DirectoryServices;
using System.Globalization;
using System.Web.Configuration;
using System.Web.Security;

namespace Project_REPORT_v7.Controllers.Addon
{
    public class ActiveDirectoryRoleProvider : RoleProvider
    {
        private string ConnectionStringName { get; set; }
        private string ConnectionUsername { get; set; }
        private string ConnectionPassword { get; set; }
        private string AttributeMapUsername { get; set; }

        public override void Initialize(string name, NameValueCollection config)
        {
            ConnectionStringName = config["connectionStringName"];
            ConnectionUsername = config["connectionUsername"];
            ConnectionPassword = config["connectionPassword"];
            AttributeMapUsername = config["attributeMapUsername"];

            base.Initialize(name, config);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            string[] roles = GetRolesForUser(username);

            foreach (string role in roles)
            {
                if (role.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public override string[] GetRolesForUser(string username)
        {
            var allRoles = new List<string>();
            
            var root = new DirectoryEntry(WebConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString, ConnectionUsername, ConnectionPassword);

            var searcher = new DirectorySearcher(root, string.Format(CultureInfo.InvariantCulture, "(&(objectClass=user)({0}={1}))", AttributeMapUsername, username));

            searcher.PropertiesToLoad.Add("memberOf");

            SearchResult result = searcher.FindOne();

            if (result != null && !string.IsNullOrEmpty(result.Path))
            {
                DirectoryEntry user = result.GetDirectoryEntry();

                PropertyValueCollection groups = user.Properties["memborOf"];

                foreach (string path in groups)
                {
                    string[] parts = path.Split(',');

                    if (parts.Length > 0)
                    {
                        foreach (string part in parts)
                        {
                            string[] p = part.Split('=');

                            if (p.Length > 0 && p[0].Equals("cn",StringComparison.OrdinalIgnoreCase))
                            {
                                allRoles.Add(p[1]);
                            }
                        }
                    }
                }
            }

            return allRoles.ToArray();
        }
    }
}