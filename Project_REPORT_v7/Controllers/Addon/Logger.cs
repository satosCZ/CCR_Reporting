using System;

namespace Project_REPORT_v7.Controllers.Addon
{
    /// <summary>
    /// Logger class
    ///     custom logger class created with no mind over efficiency and elegance
    /// </summary>
    public static class Logger
    {
        // Private fields
        private static string path = @"D:\Logs\";
        private static string infoLog = "info";
        private static string errorLog = "error";
        private static string warningLog = "warning";
        private static string extension = ".log";

        /// <summary>
        /// Method to log information to log file
        /// </summary>
        /// <param name="message">Message to save</param>
        /// <param name="function">Which function in code it was executed</param>
        public static void LogInfo(string message, string function)
        {
            try
            {
                string resutl = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {function} - {message}";
                System.IO.File.AppendAllText(path + infoLog + "_" + DateTime.UtcNow.ToString("ddMMyyyy") + extension, resutl + Environment.NewLine);
            }
            catch { }
        }

        /// <summary>
        /// Method to log error to log file
        /// </summary>
        /// <param name="message">Error message to save</param>
        /// <param name="function">Function that it was caused with</param>
        public static void LogError(string message, string function)
        {
            try
            {
                string resutl = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {function} - {message}";
                System.IO.File.AppendAllText(path + errorLog + "_" + DateTime.UtcNow.ToString( "ddMMyyyy" ) + extension, resutl + Environment.NewLine);
            }
            catch { }
        }

        /// <summary>
        /// Method to log warning to log file
        /// </summary>
        /// <param name="message">Warning message to save</param>
        /// <param name="function">Function that it was executed</param>
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