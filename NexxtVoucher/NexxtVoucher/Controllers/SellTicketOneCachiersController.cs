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
    [Authorize(Roles = "Cobros")]
    public class SellTicketOneCachiersController : Controller
    {
        private NexxtVouContext db = new NexxtVouContext();

        // GET: SellTicketOnes/Edit/5
        public ActionResult PrintTicket(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sellticketonecahiers = db.SellTicketOneCachiers.Find(id);
            if (sellticketonecahiers == null)
            {
                return HttpNotFound();
            }

            return View(sellticketonecahiers);
        }

        // GET: SellTicketOneCachiers
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var cachiers = db.Cachiers.Where(c => c.CompanyId == user.CompanyId && c.UserName == User.Identity.Name).FirstOrDefault();

            var sellTicketOneCachiers = db.SellTicketOneCachiers.Where(o=> o.CompanyId == user.CompanyId && o.CachierId == cachiers.CachierId)
                .Include(s => s.Cachier)
                .Include(s => s.OrderTicketDetail)
                .Include(s => s.PlanCategory)
                .Include(s => s.PlanTicket)
                .Include(s => s.Server);
            return View(sellTicketOneCachiers.OrderByDescending(c=> c.VentaCachier).ToList());
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

        // GET: SellTicketOneCachiers/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //var db2 = new NexxtVouContext();
            var cachiers = db.Cachiers.Where(c => c.CompanyId == user.CompanyId && c.UserName == User.Identity.Name).FirstOrDefault();
            var categoria = db.PlanCategories.Where(t => t.CompanyId == user.CompanyId && t.ServerId == cachiers.ServerId).FirstOrDefault();

            var centaXCajero = new SellTicketOneCachier
            {
                CompanyId = user.CompanyId,
                CachierId = cachiers.CachierId,
                PlanCategoryId = categoria.PlanCategoryId,
                ServerId = cachiers.ServerId,
                Date = DateTime.Today
            };

            //ViewBag.CachierId = new SelectList(ComboHelper.GetCachier(user.CompanyId), "CachierId", "FirstName");
            ViewBag.OrderTicketDetailId = new SelectList(ComboHelper.GetOrderticketdetail(user.CompanyId), "OrderTicketDetailId", "Velocidad");
            //ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(user.CompanyId), "PlanCategoryId", "Categoria");
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicketCajero(user.CompanyId, cachiers.ServerId), "PlanTicketId", "Plan");
            //ViewBag.ServerId = new SelectList(db.Servers.Where(c => c.CompanyId == user.CompanyId && c.ServerId == cachiers.ServerId), "ServerId", "Nombre");

            return View(centaXCajero);
        }

        // POST: SellTicketOneCachiers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SellTicketOneCachier sellTicketOneCachier)
        {
            if (ModelState.IsValid)
            {
                db.SellTicketOneCachiers.Add(sellTicketOneCachier);
                try
                {
                   
                    var db2 = new NexxtVouContext();
                    var db4 = new NexxtVouContext();
                    int sum = 0;
                    int Recep = 0;

                    var register = db2.Registers.Where(c => c.CompanyId == sellTicketOneCachier.CompanyId).FirstOrDefault();
                    Recep = register.VentaCachier;
                    sum = Recep + 1;
                    register.VentaCachier = sum;
                    db2.Entry(register).State = EntityState.Modified;
                    db2.SaveChanges();

                    sellTicketOneCachier.VentaCachier = sum;
                    db.SaveChanges();

                    var orderticketdetail = db4.OrderTicketDetails.Find(sellTicketOneCachier.OrderTicketDetailId);
                    orderticketdetail.Vendido = true;
                    orderticketdetail.Date = DateTime.Today;
                    orderticketdetail.VentaNumero = "C" + Convert.ToString(sum);
                    orderticketdetail.VendidoCajero = true;
                    orderticketdetail.CachierId = sellTicketOneCachier.CachierId;
                    db4.Entry(orderticketdetail).State = EntityState.Modified;
                    db4.SaveChanges();

                    db2.Dispose();
                    db4.Dispose();

                    return RedirectToAction("Details", new { id = sellTicketOneCachier.SellTicketOneCachierId });
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

            ViewBag.OrderTicketDetailId = new SelectList(ComboHelper.GetOrderticketdetail(sellTicketOneCachier.CompanyId), "OrderTicketDetailId", "Velocidad", sellTicketOneCachier.OrderTicketDetailId);
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicketCajero(sellTicketOneCachier.CompanyId, sellTicketOneCachier.ServerId), "PlanTicketId", "Plan", sellTicketOneCachier.PlanTicketId);
            return View(sellTicketOneCachier);
        }

        // GET: SellTicketOneCachiers/Edit/5
        public ActionResult Edit(int? id)
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

            ViewBag.OrderTicketDetailId = new SelectList(ComboHelper.GetOrderticketdetail(sellTicketOneCachier.CompanyId), "OrderTicketDetailId", "Velocidad", sellTicketOneCachier.OrderTicketDetailId);
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicketCajero(sellTicketOneCachier.CompanyId, sellTicketOneCachier.ServerId), "PlanTicketId", "Plan", sellTicketOneCachier.PlanTicketId);
            return View(sellTicketOneCachier);
        }

        // POST: SellTicketOneCachiers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SellTicketOneCachier sellTicketOneCachier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sellTicketOneCachier).State = EntityState.Modified;
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

            ViewBag.OrderTicketDetailId = new SelectList(ComboHelper.GetOrderticketdetail(sellTicketOneCachier.CompanyId), "OrderTicketDetailId", "Velocidad", sellTicketOneCachier.OrderTicketDetailId);
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicketCajero(sellTicketOneCachier.CompanyId, sellTicketOneCachier.ServerId), "PlanTicketId", "Plan", sellTicketOneCachier.PlanTicketId);
            return View(sellTicketOneCachier);
        }

        // GET: SellTicketOneCachiers/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: SellTicketOneCachiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var sellTicketOneCachier = db.SellTicketOneCachiers.Find(id);
            db.SellTicketOneCachiers.Remove(sellTicketOneCachier);
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
            return View(sellTicketOneCachier);
        }

        public JsonResult GetTickets(int PlanTicketId)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var cajero = db.Cachiers.Where(f => f.UserName == User.Identity.Name).FirstOrDefault();
            var orderticketdeatil = db.OrderTicketDetails.Where(o => o.PlanTicketId == PlanTicketId && o.ServerId == cajero.ServerId && o.Vendido == false);

            return Json(orderticketdeatil);
        }

        public JsonResult GetPrecio(int orderTicketdetailId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var orderdetail = db.OrderTicketDetails.Find(orderTicketdetailId);
            decimal precio = orderdetail.Precio;

            return Json(precio);
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
