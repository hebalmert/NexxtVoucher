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

    public class TicketRefreshesController : Controller
    {
        private NexxtVouContext db = new NexxtVouContext();

        // GET: TicketRefreshes
        public ActionResult Index()
        {
            return View(db.TicketRefreshes.ToList());
        }

        // GET: TicketRefreshes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketRefresh ticketRefresh = db.TicketRefreshes.Find(id);
            if (ticketRefresh == null)
            {
                return HttpNotFound();
            }
            return View(ticketRefresh);
        }

        // GET: TicketRefreshes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TicketRefreshes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TicketRefresh ticketRefresh)
        {
            if (ModelState.IsValid)
            {
                db.TicketRefreshes.Add(ticketRefresh);
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("_Index"))
                    {
                        ModelState.AddModelError(string.Empty, (@Resources.Resource.Msg_DoubleData));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }

            return View(ticketRefresh);
        }

        // GET: TicketRefreshes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketRefresh ticketRefresh = db.TicketRefreshes.Find(id);
            if (ticketRefresh == null)
            {
                return HttpNotFound();
            }
            return View(ticketRefresh);
        }

        // POST: TicketRefreshes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TicketRefresh ticketRefresh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketRefresh).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("_Index"))
                    {
                        ModelState.AddModelError(string.Empty, (@Resources.Resource.Msg_DoubleData));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }
            return View(ticketRefresh);
        }

        // GET: TicketRefreshes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketRefresh ticketRefresh = db.TicketRefreshes.Find(id);
            if (ticketRefresh == null)
            {
                return HttpNotFound();
            }
            return View(ticketRefresh);
        }

        // POST: TicketRefreshes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketRefresh ticketRefresh = db.TicketRefreshes.Find(id);
            db.TicketRefreshes.Remove(ticketRefresh);
            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    ModelState.AddModelError(string.Empty, (@Resources.Resource.Msg_Relationship));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(ticketRefresh);
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
