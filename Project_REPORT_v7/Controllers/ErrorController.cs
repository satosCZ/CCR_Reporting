using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_REPORT_v7.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        [HandleError]
        public ActionResult Error404(int statusCode, Exception exception)
        {
            //Response.StatusCode = 404;
            //Exception ex = new Exception();
            //ex = Server.GetLastError().GetBaseException();
            //TempData["Message"] = ex.Message;
            //return View("Error_404");
            Response.StatusCode = statusCode;
            TempData["Status"] = statusCode + " Error";
            return View();
        }

        [HandleError]
        public ActionResult Error500()
        {
            return View();
        }


    }
}