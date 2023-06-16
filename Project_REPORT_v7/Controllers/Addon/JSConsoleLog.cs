using System.Web;

namespace Project_REPORT_v7.Controllers.Addon
{
    //public static class JSConsoleLog
    //{
    //    static string scriptTag = "<script type=\"\" language=\"\">{0}</script>";

    //    public static void ConsoleLog(string message)
    //    {
    //        string function = "console.log('{0}');";
    //        string log = string.Format(GenerateCodeFromFunction(function), message);
    //        HttpContext.Current.Response.Write(log);
    //    }

    //    public static void Alert(string message)
    //    {
    //        string function = "alert('{0}');";
    //        string alert = string.Format(GenerateCodeFromFunction(function), message);
    //        HttpContext.Current.Response.Write(alert);
    //    }

    //    private static string GenerateCodeFromFunction(string function)
    //    {
    //        return string.Format(scriptTag, function);
    //    }
    //}

    // Usage: JSConsoleLog.ConsoleLog("Hello World!");
    public static class JSConsoleLog
    {
        // default string line
        private static string scriptTag = "<script type=\"\" language=\"\">{0}</script>";

        // function to write to console
        public static void ConsoleLog(string message)
        {
            // function to write to console
            string function = "console.log('{0}');";
            // generate code from function
            string log = string.Format(GenerateCodeFromFunction(function), message);
            // write to response
            HttpContext.Current.Response.Write(log);
        }

        // function GenerateCodeFromFunction
        private static string GenerateCodeFromFunction(string function)
        {
            // generate code from function
            return string.Format(scriptTag, function);
        }

        // function to write to alert
        public static void Alert(string message)
        {
            // function to write to alert
            string function = "alert('{0}');";
            // generate code from function
            string alert = string.Format(GenerateCodeFromFunction(function), message);
            // write to response
            HttpContext.Current.Response.Write(alert);
        }

        public static void Error(string message)
        {
              // function to write to console
            string function = "console.error('{0}');";
            // generate code from function
            string log = string.Format(GenerateCodeFromFunction(function), message);
            // write to response
            HttpContext.Current.Response.Write(log);
        }
    }
}