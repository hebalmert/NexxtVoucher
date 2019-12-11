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
    public class PlanTicketsController : Controller
    {
        private NexxtVouContext db = new NexxtVouContext();

        // GET: PlanTickets
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var planTickets = db.PlanTickets.Where(c=> c.CompanyId == user.CompanyId)
                .Include(p => p.SpeedDown)
                .Include(p => p.SpeedUp)
                .Include(p => p.Tax)
                .Include(p => p.TicketInactive)
                .Include(p => p.TicketRefresh)
                .Include(p => p.TicketTime)
                .Include(p => p.PlanCategory);

            return View(planTickets.OrderByDescending(o=> o.PlanCategory.Categoria).ThenByDescending(o2=> o2.Plan).ToList());
        }

        // GET: PlanTickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanTicket planTicket = db.PlanTickets.Find(id);
            if (planTicket == null)
            {
                return HttpNotFound();
            }
            return View(planTicket);
        }

        // GET: PlanTickets/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var planetickets = new PlanTicket { CompanyId = user.CompanyId };

            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(user.CompanyId), "PlanCategoryId", "Categoria");
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(user.CompanyId), "ServerId", "Nombre");
            ViewBag.SpeedDownId = new SelectList(ComboHelper.GetSpeedown(), "SpeedDownId", "VelocidadDown");
            ViewBag.SpeedUpId = new SelectList(ComboHelper.GetSpeeUp(), "SpeedUpId", "VelocidadUp");
            ViewBag.TaxId = new SelectList(ComboHelper.GetTax(user.CompanyId), "TaxId", "Impuesto");
            ViewBag.TicketInactiveId = new SelectList(ComboHelper.GetTicketinactive(), "TicketInactiveId", "TiempoInactivo");
            ViewBag.TicketRefreshId = new SelectList(ComboHelper.GetTicketrefresh(), "TicketRefreshId", "TiempoRefrescar");
            ViewBag.TicketTimeId = new SelectList(ComboHelper.GetTicketime(), "TicketTimeId", "TiempoTicket");

            return View(planetickets);
        }

        // POST: PlanTickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PlanTicket planTicket)
        {
            if (ModelState.IsValid)
            {
                db.PlanTickets.Add(planTicket);
                try
                {
                    
                    //se busca informacion del servidor
                    var db2 = new NexxtVouContext();
                    string ip;
                    string us;
                    string pss;
                    var servidor = db2.Servers.Find(planTicket.ServerId);
                    ip = servidor.IpServer;
                    us = servidor.Usuario;
                    pss = servidor.Clave;
                    db2.Dispose();
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    var db3 = new NexxtVouContext();
                    string tiempo;
                    var tickettiempo = db3.TicketTimes.Find(planTicket.TicketTimeId);
                    tiempo = tickettiempo.TiempoTicket;
                    db3.Dispose();
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    var db4 = new NexxtVouContext();
                    string inactivo;
                    var ticketinactivo = db4.TicketInactives.Find(planTicket.TicketInactiveId);
                    inactivo = ticketinactivo.TiempoInactivo;
                    db4.Dispose();
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    var db6 = new NexxtVouContext();
                    string refrescar;
                    var ticketrefrescar = db6.TicketRefreshes.Find(planTicket.TicketRefreshId);
                    refrescar = ticketrefrescar.TiempoRefrescar;
                    db6.Dispose();
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    var db7 = new NexxtVouContext();
                    string Vup;
                    var velocidadup = db7.SpeedUps.Find(planTicket.SpeedUpId);
                    Vup = velocidadup.VelocidadUp;
                    db7.Dispose();
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    var db8 = new NexxtVouContext();
                    string Vdown;
                    var velocidaddown = db8.SpeedDowns.Find(planTicket.SpeedDownId);
                    Vdown = velocidaddown.VelocidadDown;
                    db8.Dispose();
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
                    //Sistema de Nuevo en Mikrotik
                    MK mikrotik = new MK(ip);
                    if (!mikrotik.Login(us, pss))
                    {                      
                        //ModelState.AddModelError(string.Empty, @Resources.Resource.MikrotikFailed);
                    }
                    else
                    {
                        db.SaveChanges();

                        mikrotik.Send("/ip/hotspot/user/profile/add");
                        mikrotik.Send("=name=" + planTicket.Plan);
                        mikrotik.Send("=session-timeout=" + tiempo);
                        mikrotik.Send("=keepalive-timeout=" + tiempo);
                        mikrotik.Send("=rate-limit=" + Vup + "/" + Vdown);
                        mikrotik.Send("=shared-users=" + "unlimited");
                        mikrotik.Send("=idle-timeout=" + inactivo);
                        mikrotik.Send("=status-autorefresh=" + refrescar);
                        mikrotik.Send("=add-mac-cookie=no");
                        mikrotik.Send("=shared-users=" + 1);
                        mikrotik.Send("/ip/hotspot/user/profile/print", true);

                        int total = 0;
                        int rest = 0;
                        string idmk;
                        string mikrotiIndex;

                        foreach (var item in mikrotik.Read())
                        {
                            idmk = item;
                            total = idmk.Length;
                            rest = total - 10;
                            mikrotiIndex = idmk.Substring(10, rest);

                            var db5 = new NexxtVouContext();
                            var planticketUp = db5.PlanTickets.Find(planTicket.PlanTicketId);
                            planticketUp.MikrotikId = mikrotiIndex;
                            db5.Entry(planticketUp).State = EntityState.Modified;
                            db5.SaveChanges();
                            db5.Dispose();
                        }
                        mikrotik.Close();
                        //Cierre de la conexion a la Mikrotik
                        //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
                    }
                    
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

            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(planTicket.CompanyId), "PlanCategoryId", "Categoria", planTicket.PlanCategoryId);
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(planTicket.CompanyId), "ServerId", "Nombre", planTicket.ServerId);
            ViewBag.SpeedDownId = new SelectList(ComboHelper.GetSpeedown(), "SpeedDownId", "VelocidadDown", planTicket.SpeedDownId);
            ViewBag.SpeedUpId = new SelectList(ComboHelper.GetSpeeUp(), "SpeedUpId", "VelocidadUp", planTicket.SpeedUpId);
            ViewBag.TaxId = new SelectList(ComboHelper.GetTax(planTicket.CompanyId), "TaxId", "Impuesto", planTicket.TaxId);
            ViewBag.TicketInactiveId = new SelectList(ComboHelper.GetTicketinactive(), "TicketInactiveId", "TiempoInactivo", planTicket.TicketInactiveId);
            ViewBag.TicketRefreshId = new SelectList(ComboHelper.GetTicketrefresh(), "TicketRefreshId", "TiempoRefrescar", planTicket.TicketRefreshId);
            ViewBag.TicketTimeId = new SelectList(ComboHelper.GetTicketime(), "TicketTimeId", "TiempoTicket", planTicket.TicketTimeId);
            return View(planTicket);
        }

        // GET: PlanTickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanTicket planTicket = db.PlanTickets.Find(id);
            if (planTicket == null)
            {
                return HttpNotFound();
            }

            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(planTicket.CompanyId), "PlanCategoryId", "Categoria", planTicket.PlanCategoryId);
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(planTicket.CompanyId), "ServerId", "Nombre", planTicket.ServerId);
            ViewBag.SpeedDownId = new SelectList(ComboHelper.GetSpeedown(), "SpeedDownId", "VelocidadDown", planTicket.SpeedDownId);
            ViewBag.SpeedUpId = new SelectList(ComboHelper.GetSpeeUp(), "SpeedUpId", "VelocidadUp", planTicket.SpeedUpId);
            ViewBag.TaxId = new SelectList(ComboHelper.GetTax(planTicket.CompanyId), "TaxId", "Impuesto", planTicket.TaxId);
            ViewBag.TicketInactiveId = new SelectList(ComboHelper.GetTicketinactive(), "TicketInactiveId", "TiempoInactivo", planTicket.TicketInactiveId);
            ViewBag.TicketRefreshId = new SelectList(ComboHelper.GetTicketrefresh(), "TicketRefreshId", "TiempoRefrescar", planTicket.TicketRefreshId);
            ViewBag.TicketTimeId = new SelectList(ComboHelper.GetTicketime(), "TicketTimeId", "TiempoTicket", planTicket.TicketTimeId);

            return View(planTicket);
        }

        // POST: PlanTickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PlanTicket planTicket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(planTicket).State = EntityState.Modified;
                try
                {
                    //se busca informacion del servidor
                    var db2 = new NexxtVouContext();
                    string ip;
                    string us;
                    string pss;
                    var servidor = db2.Servers.Find(planTicket.ServerId);
                    ip = servidor.IpServer;
                    us = servidor.Usuario;
                    pss = servidor.Clave;
                    db2.Dispose();
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    var db3 = new NexxtVouContext();
                    string tiempo;
                    var tickettiempo = db3.TicketTimes.Find(planTicket.TicketTimeId);
                    tiempo = tickettiempo.TiempoTicket;
                    db3.Dispose();
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    var db4 = new NexxtVouContext();
                    string inactivo;
                    var ticketinactivo = db4.TicketInactives.Find(planTicket.TicketInactiveId);
                    inactivo = ticketinactivo.TiempoInactivo;
                    db4.Dispose();
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    var db6 = new NexxtVouContext();
                    string refrescar;
                    var ticketrefrescar = db6.TicketRefreshes.Find(planTicket.TicketRefreshId);
                    refrescar = ticketrefrescar.TiempoRefrescar;
                    db6.Dispose();
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    var db7 = new NexxtVouContext();
                    string Vup;
                    var velocidadup = db7.SpeedUps.Find(planTicket.SpeedUpId);
                    Vup = velocidadup.VelocidadUp;
                    db7.Dispose();
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    var db8 = new NexxtVouContext();
                    string Vdown;
                    var velocidaddown = db8.SpeedDowns.Find(planTicket.SpeedDownId);
                    Vdown = velocidaddown.VelocidadDown;
                    db8.Dispose();
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    MK mikrotik = new MK(ip);
                    if (!mikrotik.Login(us, pss))
                    {
                        //ModelState.AddModelError(string.Empty, @Resources.Resource.MikrotikFailed);
                    }
                    else
                    {
                        db.SaveChanges();

                        mikrotik.Send("/ip/hotspot/user/profile/set");
                        mikrotik.Send("=.id=" + planTicket.MikrotikId);
                        mikrotik.Send("=name=" + planTicket.Plan);
                        mikrotik.Send("=session-timeout=" + tiempo);
                        mikrotik.Send("=keepalive-timeout=" + tiempo);
                        mikrotik.Send("=rate-limit=" + Vup + "/" + Vdown);
                        mikrotik.Send("=shared-users=" + "unlimited");
                        mikrotik.Send("=idle-timeout=" + inactivo);
                        mikrotik.Send("=status-autorefresh=" + refrescar);
                        mikrotik.Send("=shared-users=" + 1);
                        mikrotik.Send("/ip/hotspot/user/profile/print", true);

                        int total = 0;
                        int rest = 0;
                        string idmk;

                        foreach (var item in mikrotik.Read())
                        {
                            idmk = item;
                            total = idmk.Length;
                            rest = total - 10;

                        }
                        mikrotik.Close();

                    }

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

            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(planTicket.CompanyId), "PlanCategoryId", "Categoria", planTicket.PlanCategoryId);
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(planTicket.CompanyId), "ServerId", "Nombre", planTicket.ServerId);
            ViewBag.SpeedDownId = new SelectList(ComboHelper.GetSpeedown(), "SpeedDownId", "VelocidadDown", planTicket.SpeedDownId);
            ViewBag.SpeedUpId = new SelectList(ComboHelper.GetSpeeUp(), "SpeedUpId", "VelocidadUp", planTicket.SpeedUpId);
            ViewBag.TaxId = new SelectList(ComboHelper.GetTax(planTicket.CompanyId), "TaxId", "Impuesto", planTicket.TaxId);
            ViewBag.TicketInactiveId = new SelectList(ComboHelper.GetTicketinactive(), "TicketInactiveId", "TiempoInactivo", planTicket.TicketInactiveId);
            ViewBag.TicketRefreshId = new SelectList(ComboHelper.GetTicketrefresh(), "TicketRefreshId", "TiempoRefrescar", planTicket.TicketRefreshId);
            ViewBag.TicketTimeId = new SelectList(ComboHelper.GetTicketime(), "TicketTimeId", "TiempoTicket", planTicket.TicketTimeId);
            return View(planTicket);
        }

        // GET: PlanTickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlanTicket planTicket = db.PlanTickets.Find(id);
            if (planTicket == null)
            {
                return HttpNotFound();
            }
            return View(planTicket);
        }

        // POST: PlanTickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlanTicket planTicket = db.PlanTickets.Find(id);
            db.PlanTickets.Remove(planTicket);
            try
            {
                //se busca informacion del servidor
                var db2 = new NexxtVouContext();
                string ip;
                string us;
                string pss;
                var servidor = db2.Servers.Find(planTicket.ServerId);
                ip = servidor.IpServer;
                us = servidor.Usuario;
                pss = servidor.Clave;
                db2.Dispose();
                //:::::::::::::::::::::::::::::::::::::::::::::

                MK mikrotik = new MK(ip);
                if (!mikrotik.Login(us, pss))
                {
                    //ModelState.AddModelError(string.Empty, @Resources.Resource.MikrotikFailed);
                }
                else
                {
                    db.SaveChanges();

                    mikrotik.Send("/ip/hotspot/user/profile/remove");
                    mikrotik.Send("=.id=" + planTicket.MikrotikId, true);

                    int total = 0;
                    int rest = 0;
                    string idmk;

                    foreach (var item in mikrotik.Read())
                    {
                        idmk = item;
                        total = idmk.Length;
                        rest = total - 10;

                    }
                    mikrotik.Close();

                }

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
            return View(planTicket);
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
