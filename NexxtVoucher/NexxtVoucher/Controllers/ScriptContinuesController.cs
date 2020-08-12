using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NexxtVoucher.Models;

namespace NexxtVoucher.Controllers
{
    [Authorize(Roles = "Admin")]

    public class ScriptContinuesController : Controller
    {
        private NexxtVouContext db = new NexxtVouContext();

        // GET: ScriptContinues
        public ActionResult Index()
        {
            return View(db.ScriptContinues.ToList());
        }

        // GET: ScriptContinues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScriptContinue scriptContinue = db.ScriptContinues.Find(id);
            if (scriptContinue == null)
            {
                return HttpNotFound();
            }
            return View(scriptContinue);
        }

        // GET: ScriptContinues/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ScriptContinues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ScriptContinueId,ScriptTicket")] ScriptContinue scriptContinue)
        {
            if (ModelState.IsValid)
            {
                db.ScriptContinues.Add(scriptContinue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(scriptContinue);
        }

        // GET: ScriptContinues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScriptContinue scriptContinue = db.ScriptContinues.Find(id);
            if (scriptContinue == null)
            {
                return HttpNotFound();
            }
            return View(scriptContinue);
        }

        // POST: ScriptContinues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ScriptContinueId,ScriptTicket")] ScriptContinue scriptContinue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scriptContinue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(scriptContinue);
        }

        // GET: ScriptContinues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScriptContinue scriptContinue = db.ScriptContinues.Find(id);
            if (scriptContinue == null)
            {
                return HttpNotFound();
            }
            return View(scriptContinue);
        }

        // POST: ScriptContinues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ScriptContinue scriptContinue = db.ScriptContinues.Find(id);
            db.ScriptContinues.Remove(scriptContinue);
            db.SaveChanges();
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
