﻿using System.Web.Mvc;

namespace Project_REPORT_v7.Controllers
{
    /// <summary>
    /// Error Controller class - debating if this is needed
    /// </summary>
    public class ErrorController : Controller
    {
        public ActionResult Error(HandleErrorInfo exception)
        {
            return View("ERROR", exception);
        }

        // GET: Error
        public ActionResult Error404(HandleErrorInfo exception)
        {
            ////Response.StatusCode = 404;
            ////Exception ex = new Exception();
            ////ex = Server.GetLastError().GetBaseException();
            ////TempData["Message"] = ex.Message;
            ////return View("Error_404");
            //Response.StatusCode = statusCode;
            //TempData["Status"] = statusCode + " Error";
            return View(exception);
        }


        public ActionResult Error500()
        {
            return View();
        }

        public ActionResult Error302()
        {
            return View();
        }

    }
}