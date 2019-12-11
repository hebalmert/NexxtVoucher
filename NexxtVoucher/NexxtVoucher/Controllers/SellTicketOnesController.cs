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

    public class SellTicketOnesController : Controller
    {
        private NexxtVouContext db = new NexxtVouContext();

        // GET: SellTicketOnes/Edit/5
        public ActionResult PrintTicket(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sellTicketOne = db.SellTicketOnes.Find(id);
            if (sellTicketOne == null)
            {
                return HttpNotFound();
            }

            return View(sellTicketOne);
        }

        // GET: SellTicketOnes
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var sellTicketOnes = db.SellTicketOnes
                .Include(s => s.OrderTicketDetail)
                .Include(s => s.PlanCategory)
                .Include(s => s.PlanTicket)
                .Include(s => s.Server);

            return View(sellTicketOnes.ToList());
        }

        // GET: SellTicketOnes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SellTicketOne sellTicketOne = db.SellTicketOnes.Find(id);
            if (sellTicketOne == null)
            {
                return HttpNotFound();
            }
            return View(sellTicketOne);
        }

        // GET: SellTicketOnes/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var sellticketone = new SellTicketOne 
            { 
                CompanyId = user.CompanyId,
                Date = DateTime.Today
            };

            ViewBag.OrderTicketDetailId = new SelectList(ComboHelper.GetOrderticketdetail(user.CompanyId), "OrderTicketDetailId", "Usuario");
            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(user.CompanyId), "PlanCategoryId", "Categoria");
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicket(user.CompanyId), "PlanTicketId", "Plan");
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(user.CompanyId), "ServerId", "Nombre");

            return View(sellticketone);
        }

        // POST: SellTicketOnes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SellTicketOne sellTicketOne)
        {
            if (ModelState.IsValid)
            {
                db.SellTicketOnes.Add(sellTicketOne);
                try
                {
                    db.SaveChanges();

                    var db2 = new NexxtVouContext();
                    var db3 = new NexxtVouContext();
                    var db4 = new NexxtVouContext();
                    int sum = 0;
                    int Recep = 0;

                    var register = db2.Registers.Where(c => c.CompanyId == sellTicketOne.CompanyId).FirstOrDefault();
                    Recep = register.VentaOne;
                    sum = Recep + 1;
                    register.VentaOne = sum;
                    db2.Entry(register).State = EntityState.Modified;
                    db2.SaveChanges();
                    
                    var sellticket = db3.SellTicketOnes.Find(sellTicketOne.SellTicketOneId);
                    sellticket.VentaOne = sum;
                    db3.Entry(sellticket).State = EntityState.Modified;
                    db3.SaveChanges();
                    
                    var orderticketdetail = db4.OrderTicketDetails.Find(sellTicketOne.OrderTicketDetailId);
                    orderticketdetail.Vendido = true;
                    orderticketdetail.VentaNumero = Convert.ToString(sum);
                    db4.Entry(orderticketdetail).State = EntityState.Modified;
                    db4.SaveChanges();

                    db4.Dispose();
                    db2.Dispose();
                    db3.Dispose();

                    return RedirectToAction("Details", new { id = sellTicketOne.SellTicketOneId});
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


            ViewBag.OrderTicketDetailId = new SelectList(ComboHelper.GetOrderticketdetail(sellTicketOne.CompanyId), "OrderTicketDetailId", "Usuario", sellTicketOne.OrderTicketDetailId);
            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(sellTicketOne.CompanyId), "PlanCategoryId", "Categoria", sellTicketOne.PlanCategoryId);
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicket(sellTicketOne.CompanyId), "PlanTicketId", "Plan", sellTicketOne.PlanTicketId);
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(sellTicketOne.CompanyId), "ServerId", "Nombre", sellTicketOne.ServerId);
            return View(sellTicketOne);
        }

        // GET: SellTicketOnes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SellTicketOne sellTicketOne = db.SellTicketOnes.Find(id);
            if (sellTicketOne == null)
            {
                return HttpNotFound();
            }

            ViewBag.OrderTicketDetailId = new SelectList(ComboHelper.GetOrderticketdetail(sellTicketOne.CompanyId), "OrderTicketDetailId", "Usuario", sellTicketOne.OrderTicketDetailId);
            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(sellTicketOne.CompanyId), "PlanCategoryId", "Categoria", sellTicketOne.PlanCategoryId);
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicket(sellTicketOne.CompanyId), "PlanTicketId", "Plan", sellTicketOne.PlanTicketId);
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(sellTicketOne.CompanyId), "ServerId", "Nombre", sellTicketOne.ServerId);
            return View(sellTicketOne);
        }

        // POST: SellTicketOnes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SellTicketOne sellTicketOne)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sellTicketOne).State = EntityState.Modified;
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

            ViewBag.OrderTicketDetailId = new SelectList(ComboHelper.GetOrderticketdetail(sellTicketOne.CompanyId), "OrderTicketDetailId", "Usuario", sellTicketOne.OrderTicketDetailId);
            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(sellTicketOne.CompanyId), "PlanCategoryId", "Categoria", sellTicketOne.PlanCategoryId);
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicket(sellTicketOne.CompanyId), "PlanTicketId", "Plan", sellTicketOne.PlanTicketId);
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(sellTicketOne.CompanyId), "ServerId", "Nombre", sellTicketOne.ServerId);
            return View(sellTicketOne);
        }

        // GET: SellTicketOnes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SellTicketOne sellTicketOne = db.SellTicketOnes.Find(id);
            if (sellTicketOne == null)
            {
                return HttpNotFound();
            }
            return View(sellTicketOne);
        }

        // POST: SellTicketOnes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SellTicketOne sellTicketOne = db.SellTicketOnes.Find(id);
            db.SellTicketOnes.Remove(sellTicketOne);
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
            return View(sellTicketOne);
        }

        public JsonResult GetPlanes(int categoryid)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var planticket = db.PlanTickets.Where(c => c.PlanCategoryId == categoryid);

            return Json(planticket);
        }

        public JsonResult GetTickets(int categoriaId, int servidorId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var orderticketdeatil = db.OrderTicketDetails.Where(o => o.ServerId == servidorId && o.PlanCategoryId == categoriaId && o.Vendido == false);

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
