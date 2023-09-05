using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Project_REPORT_v7.Controllers.Addon;
using Project_REPORT_v7.Models;

namespace Project_REPORT_v7.Controllers
{
    /// <summary>
    /// HourOvertimeTablesController is a class that used to manage HourOvertimeTable data.
    /// Authomaticly created by MVC Entity Framework.
    /// </summary>
    [CheckSessionTimeOut]
    public class HourOvertimeTablesController : Controller
    {
        // Private variable that used to connect to database.
        private ReportDBEntities1 db = new ReportDBEntities1();

        /// <summary>
        /// Action method Index GET that used to show HourOvertimeTable data.
        /// </summary>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report,CCR_Report_Control,CCR_Report_Admin")]
        public PartialViewResult _index()
        {
            var hourOvertimeTable = db.HourOvertimeTable.Include(p => p.ReportTable);
            return PartialView(hourOvertimeTable.OrderByDescending(o => o.Time).ToList());
        }

        /// <summary>
        /// Action method Create GET to show Create page for Hour Overtime table row.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        public ActionResult Create()
        {
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift");
            return PartialView("Create");
        }

        /// <summary>
        /// Action method Create POST to save new Hour Overtime table row. Action is called by Ajax so there is no need for redirect. Used for modular purpose.
        /// </summary>
        /// <param name="hourOvertimeTable"></param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "OvertimeID,Time,Duration,Shop,Type,Description,Cooperation,ReportID")] HourOvertimeTable hourOvertimeTable)
        {
            Session["ErrorMessage"] = "";
            Guid passID;
            // Check if ReportID is exist or not in Session data and assign it to variable passID.
            if (Session["ActiveGUID"] != null)
                passID = (Guid)Session["ActiveGUID"];
            else
            {
                // Close as 
                Session["ErrorMessage"] = "No ReportID was found. Refresh the page and fill this form again. If it's happen again, contact web administrator/developer.";
                return Json(this, JsonRequestBehavior.AllowGet);
            }

            // Check if the model state is valid or not. Which means that the data is valid or not. Length, format, etc.
            if (ModelState.IsValid)
            {
                hourOvertimeTable.OvertimeID = Guid.NewGuid();
                hourOvertimeTable.ReportID = passID;
                hourOvertimeTable.Type = hourOvertimeTable.Type.ToAutoCapitalize();
                hourOvertimeTable.Description = hourOvertimeTable.Description.ToAutoCapitalize();
                db.HourOvertimeTable.Add(hourOvertimeTable);

                // Log the action
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "HourOvertime | Create", $"Time:{hourOvertimeTable.Time} Duration:{hourOvertimeTable.Duration} Shop:{hourOvertimeTable.Shop} Type:{hourOvertimeTable.Type} Description:{hourOvertimeTable.Description} Cooperation:{hourOvertimeTable.Cooperation}", userID);
                }
                catch { }

                // Save the changes to database.
                db.SaveChanges();

                // Return success message to ajax.
                return Json(new { success = true });
            }
            else
            {
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "HourOvertime | Create | Error", $"Time:{hourOvertimeTable.Time} Duration:{hourOvertimeTable.Duration} Shop:{hourOvertimeTable.Shop} Type:{hourOvertimeTable.Type} Description:{hourOvertimeTable.Description} Cooperation:{hourOvertimeTable.Cooperation}", userID);
                }
                catch { }
            }

            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", hourOvertimeTable.ReportID);
            //return Json(new { success = false });
            return Json(this, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GET: Edit Hour Overtime table row. Used for modular purpose.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Edit(Guid? id)
        {
            // Check if the id is null or not.
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Find the Hour Overtime table row by id.
            HourOvertimeTable hourOvertimeTable = db.HourOvertimeTable.Find(id);
            
            // Check if the Hour Overtime table row is exist or not.
            if (hourOvertimeTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", hourOvertimeTable.ReportID);

            // Return the Hour Overtime table row to the view.
            return PartialView(hourOvertimeTable);
        }

        /// <summary>
        /// POST: Edit Hour Overtime table row. Used for modular purpose. Action is called by Ajax so there is no need for redirect.
        /// </summary>
        /// <param name="hourOvertimeTable"></param>
        /// <returns></returns>
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OvertimeID,Time,Duration,Shop,Type,Description,Cooperation,ReportID")] HourOvertimeTable hourOvertimeTable)
        {
            Session["ErrorMessage"] = "";
            Guid passID;

            // Check if ReportID is exist or not in Session data and assign it to variable passID.
            if (Session["ActiveGUID"] != null)
            {
                passID = (Guid)Session["ActiveGUID"];
            }
            // If not exist, return error message.
            else
            {
                // Close as 
                Session["ErrorMessage"] = "No ReportID was found. Refresh the page and fill this form again. If it's happen again, contact web administrator/developer.";
                return Json(this, JsonRequestBehavior.AllowGet);
            }

            // Check if the model state is valid or not. Which means that the data is valid or not. Length, format, etc.
            if (ModelState.IsValid)
            {
                hourOvertimeTable.ReportID = passID;
                hourOvertimeTable.Type = hourOvertimeTable.Type.ToAutoCapitalize();
                hourOvertimeTable.Description = hourOvertimeTable.Description.ToAutoCapitalize();
                
                // Change the state of the Hour Overtime table row to modified.
                db.Entry(hourOvertimeTable).State = EntityState.Modified;
                
                // Save the changes to database.
                db.SaveChanges();
                
                // Log the action
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "HourOvertime | Edit", $"Time:{hourOvertimeTable.Time} Duration:{hourOvertimeTable.Duration} Shop:{hourOvertimeTable.Shop} Type:{hourOvertimeTable.Type} Description:{hourOvertimeTable.Description} Cooperation:{hourOvertimeTable.Cooperation}", userID);
                }
                catch { }

                // Return success message to ajax.
                return Json(new { success = true });
            }
            // If not valid, return error message.
            else
            {
                try
                {
                    int userID;
                    if (int.TryParse(Session["UserID"].ToString(), out userID))
                        LogHelper.AddLog(DateTime.Now, "HourOvertime | Edit | Error", $"Time:{hourOvertimeTable.Time} Duration:{hourOvertimeTable.Duration} Shop:{hourOvertimeTable.Shop} Type:{hourOvertimeTable.Type} Description:{hourOvertimeTable.Description} Cooperation:{hourOvertimeTable.Cooperation}", userID);
                }
                catch { }
            }
            ViewBag.ReportID = new SelectList(db.ReportTable, "ReportID", "Shift", hourOvertimeTable.ReportID);
            // Return failure message to ajax.
            return Json(new { success = false });
        }

        /// <summary>
        /// GET: Delete Hour Overtime table row. Used for modular purpose. Action is called by Ajax so there is no need for redirect.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthorizeAD(Groups = "CCR_Report_Control,CCR_Report_Admin")]
        [HttpGet]
        public ActionResult Delete(Guid? id)
        {
            // Check if the id is null or not.
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Find the Hour Overtime table row by id.
            HourOvertimeTable hourOvertimeTable = db.HourOvertimeTable.Find(id);

            // Check if the Hour Overtime table row is exist or not.
            if (hourOvertimeTable == null)
            {
                return HttpNotFound();
            }

            // Return the Hour Overtime table row to the view.
            return PartialView("Delete", hourOvertimeTable);
        }
        

        /// <summary>
        /// POST: Delete Hour Overtime table row. Used for modular purpose. Action is called by Ajax so there is no need for redirect.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            // Find the Hour Overtime table row by id.
            HourOvertimeTable hourOvertimeTable = db.HourOvertimeTable.Find(id);
            
            // Remove the Hour Overtime table row.
            db.HourOvertimeTable.Remove(hourOvertimeTable);
            
            // Save the changes to database.
            try
            {
                int userID;
                if (int.TryParse(Session["UserID"].ToString(), out userID))
                    LogHelper.AddLog(DateTime.Now, "HourOvertime | Delete", $"Time:{hourOvertimeTable.Time} Duration:{hourOvertimeTable.Duration} Shop:{hourOvertimeTable.Shop} Type:{hourOvertimeTable.Type} Description:{hourOvertimeTable.Description} Cooperation:{hourOvertimeTable.Cooperation}", userID);
                }
            catch { }
            // Return success message to ajax.
            db.SaveChanges();

            // Return success message to ajax.
            return Json(new { success = true });
        }

        /// <summary>
        /// Ajax method that will get unique data from row Shops. Or default HMMC,GLOVIS,TRANSYS
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetShops()
        {
            // Variable for db connection
            ReportDBEntities1 sdb = new ReportDBEntities1();

            // Variable for list of select list item
            var ls = new List<SelectListItem>();

            // Variable for temporary data with all unique shops
            var temp = sdb.MainTaskTable.Select(s => new
            {
                Shop = s.Shop
            }).Distinct().ToList();

            // If the count of the temp data is more than 2, then add all the shops to the list.
            if (temp.Count > 2)
            {
                ls = temp.ConvertAll(a =>
                {
                    return new SelectListItem()
                    {
                        Text = a.Shop.ToString(),
                        Value = a.Shop.ToString(),
                        Selected = false
                    };
                });
            }
            // If the count of the temp data is less than or equal to 2, then add default shops to the list.
            else if (temp.Count <= 2)
            {
                ls.Add(
                    new SelectListItem()
                    {
                        Text = "HMMC",
                        Value = "HMMC",
                        Selected = false
                    });
                ls.Add(
                    new SelectListItem()
                    {
                        Text = "GLOVIS",
                        Value = "GLOVIS",
                        Selected = false
                    });
                ls.Add(
                    new SelectListItem()
                    {
                        Text = "TRANSYS",
                        Value = "TRANSYS",
                        Selected = false
                    });
            }

            // For measure purpose again make the list distinct.
            var result = ls.Distinct().ToList();

            // Set the first item of the list to selected.
            result[1].Selected = true;

            // Return the list.
            return result;
        }

        /// <summary>
        /// Ajax method that will return all the data from row Types by matching term. Used in autocomplete text input.
        /// Not case sensitive but white space sensitive
        /// </summary>
        /// <param name="term">Searchable text</param>
        /// <param name="cnt">Max displayed searched results</param>
        /// <returns>Return array of strings</returns>
        public JsonResult GetTypes(string term, int cnt)
        {
            // Variable for db connection with selecting all the data from row Types then distinct it and take only selected few by variable cnt.
            var check = db.HourOvertimeTable.Select( s => new
            {
                Type = s.Type
            }).Where(w => w.Type.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);

            // Return the data to ajax.
            return Json(check, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Ajax method that will return all the data from row Descriptions by matching term. Used in autocomplete text input.
        /// Not case sensitive but white space sensitive
        /// </summary>
        /// <param name="term">Searchable text</param>
        /// <param name="cnt">Max displayed searched results</param>
        /// <returns></returns>
        public JsonResult GetDescriptions(string term, int cnt)
        {
            // Variable for db connection with selecting all the data from row Descriptions then distinct it and take only selected few by variable cnt.
            var check = db.HourOvertimeTable.Select(s => new
            {
                Description = s.Description
            }).Where(w => w.Description.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);

            // Return the data to ajax.
            return Json(check, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Ajax method that will return all the data from row Cooperations by matching term. Used in autocomplete text input.
        /// Not case sensitive but white space sensitive
        /// </summary>
        /// <param name="term">Searchable text</param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public JsonResult GetCooperations(string term, int cnt)
        {
            // Variable for db connection with selecting all the data from row Cooperations then distinct it and take only selected few by variable cnt.
            var check = db.HourOvertimeTable.Select(s => new
            {
                Cooperation = s.Cooperation
            }).Where(w => w.Cooperation.ToLower().Contains(term.ToLower())).Distinct().Take(cnt);

            // Return the data to ajax.
            return Json(check, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Dispose method for db connection.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
