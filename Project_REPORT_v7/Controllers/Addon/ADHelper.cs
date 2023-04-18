using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices;
using System.Web.Caching;
using System.Web.ModelBinding;
using System.Web.WebPages;

namespace Project_REPORT_v7.Controllers.Addon
{
    public class ADHelper
    {
        public ADHelper(string name)
        {
            memberID = 0;
            memberName = string.Empty;
            memberEmail = string.Empty;
            temp = name;
            GetInformation(name);
        }

        public void GetInformation(string name)
        {
            try
            {
                var searcher = new DirectorySearcher(new DirectoryEntry(ConfigurationManager.ConnectionStrings["ADConnectionString"].ConnectionString), $"(&(objectClass=user)(sAMAccountName={name}))");
                searcher.PropertiesToLoad.Add("sAMAccountName");
                searcher.PropertiesToLoad.Add("name");
                searcher.PropertiesToLoad.Add("mail");
                var result = searcher.FindOne();
                int temoNum = 0;
                if (result != null)
                {
                    if (int.TryParse(result.Properties["sAMAccountName"][0].ToString(), out temoNum))
                        memberID = temoNum;
                    memberName = result.Properties["name"][0].ToString();
                    memberEmail = result.Properties["mail"][0].ToString();
                }
            }
            catch { }
        }

        private int memberID;
        private string memberName;
        private string memberEmail;
        private string temp;

        public int MemberID
        { get { return memberID; }  }
        public string MemberName
        { get { return memberName; } }
        public string MemberEmail
        { get { return memberEmail; } }
        public string Temp
        { get { return temp; } }
    }
}