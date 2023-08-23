using Project_REPORT_v7.Models;
using System;
using System.Diagnostics;

namespace Project_REPORT_v7.Controllers.Addon
{
    /// <summary>
    /// LogHelper class
    ///     custom little class that it could be viewed as controller
    /// </summary>
    public class LogHelper
    {
        // Private variable for database connection
        private static ReportDBEntities1 db = new ReportDBEntities1();

        /// <summary>
        /// Method to add log to database
        /// </summary>
        /// <param name="l_date">DateTime: Usualy DateTime.Now</param>
        /// <param name="l_type">String: Used for Create|Edit|Delete</param>
        /// <param name="l_message">String: Used as summary of message</param>
        /// <param name="l_user">Integer: ID of the user</param>
        public static void AddLog(DateTime l_date, string l_type, string l_message, int l_user)
        {
            LogTable lt = new LogTable();

            try
            {
                lt.L_DATE = l_date;
                lt.L_TYPE = l_type;
                lt.L_MESSAGE = l_message;
                lt.L_USER_ID = l_user;
                db.LogTable.Add(lt);
                db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine("AddLog error: " + e.Message);
            }
        }
    }
}