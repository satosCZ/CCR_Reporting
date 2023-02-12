using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Project_REPORT_v7.Controllers.Addon
{
    public static class AppSettings
    {
        private static NameValueCollection _cachedRoles;
        private static NameValueCollection _roles
        {
            get
            {
                if (_cachedRoles == null )
                {
                    _cachedRoles = ConfigurationManager.GetSection("roles") as NameValueCollection;
                }
                return _cachedRoles;
            }
        }

        public static string IT_HAECZ_MES_SECTION
        {
            get
            {
                return _roles.GetValues("ITHaeczMesSection").First();
            }
        }

        public static string IT_MES_ADMIN
        {
            get
            {
                return _roles.GetValues("ITMesAdmin").First();
            }
        }

        public static string IT_MES_TECHNICIAN
        {
            get
            {
                return _roles.GetValues("ITMesTechnician").First();
            }
        }

        public static IEnumerable<string> APPLICATION_GROUPS
        {
            get
            {
                return _roles.Cast<string>().Select(e => _roles[e]);
            }
        }
    }
}