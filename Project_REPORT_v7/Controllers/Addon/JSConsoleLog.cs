using System.Web;

namespace Project_REPORT_v7.Controllers.Addon
{
    public static class JSConsoleLog
    {
        static string scriptTag = "<script type=\"\" language=\"\">{0}</script>";

        public static void ConsoleLog(string message)
        {
            string function = "console.log('{0}');";
            string log = string.Format(GenerateCodeFromFunction(function), message);
            HttpContext.Current.Response.Write(log);
        }

        public static void Alert(string message)
        {
            string function = "alert('{0}');";
            string alert = string.Format(GenerateCodeFromFunction(function), message);
            HttpContext.Current.Response.Write(alert);
        }

        private static string GenerateCodeFromFunction(string function)
        {
            return string.Format(scriptTag, function);
        }
    }
}