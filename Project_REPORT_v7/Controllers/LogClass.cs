using Project_REPORT_v7.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Project_REPORT_v7.Controllers
{
    public class LogClass
    {
        private static ReportDBEntities1 db = new ReportDBEntities1();
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