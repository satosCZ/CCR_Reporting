using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_REPORT_v7.Models;
using System.Diagnostics;
using Project_REPORT_v7.App_Start;

namespace Project_REPORT_v7.Controllers
{
    public class LogTablesController : Controller
    {
        private ReportDBEntities1 db = new ReportDBEntities1();

        // GET: LogTables
        [GroupAuthorize("ITMesAdmin")]
        public async Task<ActionResult> Index()
        {
            return View(await db.LogTable.ToListAsync());
        }

        // GET: LogTables/Details/5
        [GroupAuthorize("ITMesAdmin")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogTable logTable = await db.LogTable.FindAsync(id);
            if (logTable == null)
            {
                return HttpNotFound();
            }
            return View(logTable);
        }

        // GET: LogTables/Create
        [GroupAuthorize("ITMesAdmin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: LogTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [GroupAuthorize("ITMesAdmin")]
        public async Task<ActionResult> Create([Bind(Include = "Id,L_DATE,L_TYPE,L_MESSAGE,L_USER_ID")] LogTable logTable)
        {
            if (ModelState.IsValid)
            {
                db.LogTable.Add(logTable);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(logTable);
        }

        // GET: LogTables/Edit/5
        [GroupAuthorize("ITMesAdmin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogTable logTable = await db.LogTable.FindAsync(id);
            if (logTable == null)
            {
                return HttpNotFound();
            }
            return View(logTable);
        }

        // POST: LogTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [GroupAuthorize("ITMesAdmin")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,L_DATE,L_TYPE,L_MESSAGE,L_USER_ID")] LogTable logTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(logTable).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(logTable);
        }

        // GET: LogTables/Delete/5
        [GroupAuthorize("ITMesAdmin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogTable logTable = await db.LogTable.FindAsync(id);
            if (logTable == null)
            {
                return HttpNotFound();
            }
            return View(logTable);
        }

        // POST: LogTables/Delete/5
        [GroupAuthorize("ITMesAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            LogTable logTable = await db.LogTable.FindAsync(id);
            db.LogTable.Remove(logTable);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

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
