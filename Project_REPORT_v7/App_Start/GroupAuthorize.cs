using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_REPORT_v7.App_Start
{
    public class GroupAuthorize : AuthorizeAttribute
    {
        public GroupAuthorize(params string[] roleKeys)
        {
            List<string> roles = new List<string>(roleKeys.Length);
            NameValueCollection allRoles = ConfigurationManager.GetSection("roles") as NameValueCollection;
            foreach (string roleKey in roleKeys)
            {
                roles.Add(allRoles[roleKey]);
            }

            this.Roles = string.Join(",", roles);
        }
    }
}