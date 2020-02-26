using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NexxtVoucher.Classes;
using NexxtVoucher.Models;

namespace NexxtVoucher.Controllers
{
    [Authorize(Roles = "User")]
    public class SellTicketOneCachiersRepController : Controller
    {
        private NexxtVouContext db = new NexxtVouContext();

        public ActionResult PrintReportCachier()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var printviewcajero = new PrintViewCachier
            {
                CompanyId = user.CompanyId,
                DateFin = DateTime.Today,
                DateInicio = DateTime.Today
            };

            ViewBag.CachierId = new SelectList(ComboHelper.GetCachier(user.CompanyId), "CachierId", "FullName", (0));

            return View(printviewcajero);
        }

        // Post: PrintCxCReport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrintReportCachier(PrintViewCachier printViewcachier)
        {

            return RedirectToAction("PrintReport", new { fechaInicio = printViewcachier.DateInicio, fechafin = printViewcachier.DateFin, id = printViewcachier.CachierId });
        }

        // GET: PaymentChachiers
        public ActionResult PrintReport(DateTime fechaInicio, DateTime fechafin, int id)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var sellTicketOneCachiers = db.SellTicketOneCachiers.Where(c => c.CompanyId == user.CompanyId && c.Date >= fechaInicio && c.Date <= fechafin && c.CachierId == id)
                .Include(s => s.Cachier)
                .Include(s => s.OrderTicketDetail)
                .Include(s => s.PlanCategory)
                .Include(s => s.PlanTicket)
                .Include(s => s.Server);

            return View(sellTicketOneCachiers.OrderByDescending(c => c.VentaCachier).ToList());
        }

        // GET: SellTicketOneCachiers
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var sellTicketOneCachiers = db.SellTicketOneCachiers.Where(o => o.CompanyId == user.CompanyId)
                .Include(s => s.Cachier)
                .Include(s => s.OrderTicketDetail)
                .Include(s => s.PlanCategory)
                .Include(s => s.PlanTicket)
                .Include(s => s.Server);
            return View(sellTicketOneCachiers.OrderByDescending(c => c.VentaCachier).ToList());
        }

        // GET: SellTicketOneCachiers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sellTicketOneCachier = db.SellTicketOneCachiers.Find(id);
            if (sellTicketOneCachier == null)
            {
                return HttpNotFound();
            }
            return View(sellTicketOneCachier);
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