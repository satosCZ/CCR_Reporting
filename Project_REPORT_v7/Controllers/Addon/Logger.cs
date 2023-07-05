﻿using System;

namespace Project_REPORT_v7.Controllers.Addon
{
    public static class Logger
    {
        private static string path = @"D:\Logs\";
        private static string infoLog = "info";
        private static string errorLog = "error";
        private static string warningLog = "warning";
        private static string extension = ".log";

        public static void LogInfo(string message, string function)
        {
            try
            {
                string resutl = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {function} - {message}";
                System.IO.File.AppendAllText(path + infoLog + "_" + DateTime.UtcNow.ToString("ddMMyyyy") + extension, resutl + Environment.NewLine);
            }
            catch { }
        }

        public static void LogError(string message, string function)
        {
            try
            {
                string resutl = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {function} - {message}";
                System.IO.File.AppendAllText(path + errorLog + "_" + DateTime.UtcNow.ToString( "ddMMyyyy" ) + extension, resutl + Environment.NewLine);
            }
            catch { }
        }

        public static void LogWarning(string message, string function)
        {
            try
            {
                string resutl = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {function} - {message}";
                System.IO.File.AppendAllText(path + warningLog + "_" + DateTime.UtcNow.ToString( "ddMMyyyy" ) + extension, resutl + Environment.NewLine);
            }
            catch { }
        }


    }
}