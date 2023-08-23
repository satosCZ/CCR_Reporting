using System;
using System.Configuration;
using System.DirectoryServices;

namespace Project_REPORT_v7.Controllers.Addon
{
    /// <summary>
    /// Custom class for Active Directory helper to get user information
    /// </summary>
    public class ADHelper
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name for searching in AD</param>
        public ADHelper(string name)
        {
            memberID = 0;
            memberName = string.Empty;
            memberEmail = string.Empty;
            temp = name;
            Logger.LogInfo($"ADHelper - {name}", "Project_REPORT_v7.Controllers.Addon.ADHelper.ADHelper(string name)");
            GetInformation(name);
        }

        /// <summary>
        /// Method to get user information from AD by name
        /// </summary>
        /// <param name="name">Use for getting name for login</param>
        public void GetInformation(string name)
        {
            try
            {
                // DirectorySearcher that connects to AD by sAMAccountName and load sAMAccountName, name, mail
                var searcher = new DirectorySearcher(new DirectoryEntry(ConfigurationManager.ConnectionStrings["ADConnectionString"].ConnectionString), $"(&(objectClass=user)(sAMAccountName={name}))");
                searcher.PropertiesToLoad.Add("sAMAccountName");
                searcher.PropertiesToLoad.Add("name");
                searcher.PropertiesToLoad.Add("mail");

                // Save the result from AD
                var result = searcher.FindOne();
                int tempNum = 0;

                // If result is not null, assign Name and Email and convert sAMAccountName to int for MemberID - Must be int for login ie 90000090
                if (result != null)
                {
                    if (int.TryParse(result.Properties["sAMAccountName"][0].ToString(), out tempNum))
                        memberID = tempNum;
                    memberName = result.Properties["name"][0].ToString();
                    memberEmail = result.Properties["mail"][0].ToString();
                }
            }
            catch (Exception ex)
            { 
                Logger.LogError($"ADHelper - {name}", "Project_REPORT_v7.Controllers.Addon.ADHelper.GetInformation(string name)");
                Logger.LogError( $"Error: {ex.Message}", "Project_REPORT_v7.Controllers.Addon.ADHelper.GetInformation(string name)" );
            }
        }

        // Private variables
        private int memberID;
        private string memberName;
        private string memberEmail;
        private string temp;

        // Public properties
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