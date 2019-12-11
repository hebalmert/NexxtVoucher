using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NexxtVoucher.Classes;
using NexxtVoucher.Models;

namespace NexxtVoucher.Controllers
{
    public class OrderTicketsController : Controller
    {
        private NexxtVouContext db = new NexxtVouContext();


        // GET: OrderTickets
        public ActionResult AddTicket(int id, string up, string down) //id = OrderTicketId
        {
            var ordenticket = db.OrderTickets.Find(id);
            int can = ordenticket.Cantidad;
            int planid = ordenticket.PlanTicketId;
            string plan =ordenticket.Plan;
            string tiempo = ordenticket.PlanTicket.TicketTime.TiempoTicket;
            string ip = ordenticket.Server.IpServer;
            int Serv = ordenticket.ServerId;
            int catego = ordenticket.PlanCategoryId;
            decimal prec = ordenticket.Precio;
            int conteo = 0;

            var db12 = new NexxtVouContext();
            var servidordata = db12.Servers.Find(Serv);
            string us = servidordata.Usuario;
            string pss = servidordata.Clave;
            db12.Dispose();

            //Se hace con conexion a la Mikroti y se deja abierto
            ////////////////////////////////////////////////////////////
            MK mikrotik = new MK(ip);
            if (!mikrotik.Login(us, pss))
            {
                return RedirectToAction("Index");
            }
            ///////////////////////////////////////////////////////////////////////////////////
            
            var db2 = new NexxtVouContext();
            var cadenas = db2.ChainCodes.Where(c => c.CompanyId == ordenticket.CompanyId).FirstOrDefault();
            int largocadena = cadenas.Largo;
            string caracteres = cadenas.Cadena;
            string codigoTicket;
            db2.Dispose();

            var db3 = new NexxtVouContext();
            var db4 = new NexxtVouContext();
            var db11 = new NexxtVouContext();

            for (int i = 0; i < can; i++)
            {
                codigoTicket = ComboHelper.GetCrearPassword(largocadena, caracteres);
                var ticketsearch = db11.OrderTicketDetails.Where(s => s.Usuario == codigoTicket).FirstOrDefault();
                if (ticketsearch != null)
                {
                    while (codigoTicket != ticketsearch.Usuario)
                    {
                        codigoTicket = ComboHelper.GetCrearPassword(largocadena, caracteres);
                    } 
                }

                int sum = 0;
                int Recep = 0;

                var register = db3.Registers.Where(c => c.CompanyId == ordenticket.CompanyId).FirstOrDefault();
                Recep = register.Tickets;
                sum = Recep + 1;
                register.Tickets = sum;
                db3.Entry(register).State = EntityState.Modified;

                //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
                //Sistema de Nuevo en Mikrotik

                mikrotik.Send("/ip/hotspot/user/add");
                mikrotik.Send("=name=" + codigoTicket);
                mikrotik.Send("=password=" + "1234");
                mikrotik.Send("=limit-uptime=" + tiempo);
                mikrotik.Send("=profile=" + plan);
                mikrotik.Send("/ip/hotspot/user/print", true);

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

                    var orderticketdetails = new OrderTicketDetail
                    {
                        CompanyId = ordenticket.CompanyId,
                        OrderTicketId = id,
                        TicketNumero = sum,
                        ServerId = Serv,
                        PlanTicketId = planid,
                        Velocidad = up + "/" + down,
                        Usuario = codigoTicket,
                        Clave = "1234",
                        PlanCategoryId = catego,
                        Precio = prec,
                        MikrotikId = mikrotiIndex
                    };
                    db4.OrderTicketDetails.Add(orderticketdetails);
                    db4.SaveChanges();
                }

                db3.SaveChanges();
                conteo += 1;
            }
            mikrotik.Close();

            db3.Dispose();  //conexion para el Registro de Consecutivos
            db4.Dispose();  //Se cierra la crecion del ticket en el OrderTicketDetail
            db11.Dispose();  //Se cierra la crecion del ticket en el OrderTicketDetail

            var db10 = new NexxtVouContext();
            var orderticketup = db10.OrderTickets.Find(id);
            orderticketup.Creados = conteo;
            orderticketup.Mikrotik = true;
            db10.Entry(orderticketup).State = EntityState.Modified;
            db10.SaveChanges();
            db10.Dispose();

            return RedirectToAction("Details", new { id = ordenticket.OrderTicketId });
        }

        // GET: OrderTickets
        public ActionResult Index()
        {
            var orderTickets = db.OrderTickets
                .Include(o => o.PlanCategory)
                .Include(o => o.PlanTicket)
                .Include(o => o.Server);
            return View(orderTickets.ToList());
        }

        // GET: OrderTickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderTicket orderTicket = db.OrderTickets.Find(id);
            if (orderTicket == null)
            {
                return HttpNotFound();
            }
            return View(orderTicket);
        }

        // GET: OrderTickets/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var orderticket = new OrderTicket
            { 
                CompanyId = user.CompanyId,
                Date = DateTime.Today,
                Cantidad = 0
            };

            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(user.CompanyId), "PlanCategoryId", "Categoria");
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicket(user.CompanyId), "PlanTicketId", "Plan");
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(user.CompanyId), "ServerId", "Nombre");
            return View(orderticket);
        }

        // POST: OrderTickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrderTicket orderTicket)
        {
            if (ModelState.IsValid)
            {
                db.OrderTickets.Add(orderTicket);
                try
                {
                    db.SaveChanges();

                    var db2 = new NexxtVouContext();
                    var db3 = new NexxtVouContext();
                    int sum = 0;
                    int Recep = 0;

                    var register = db2.Registers.Where(c => c.CompanyId == orderTicket.CompanyId).FirstOrDefault();
                    Recep = register.OrderTickets;
                    sum = Recep + 1;
                    register.OrderTickets = sum;
                    db2.Entry(register).State = EntityState.Modified;
                    db2.SaveChanges();
                    db2.Dispose();

                    var db6 = new NexxtVouContext();
                    var planes = db6.PlanTickets.Find(orderTicket.PlanTicketId);
                    string plan = planes.Plan;
                    db6.Dispose();

                    var ordenes = db3.OrderTickets.Find(orderTicket.OrderTicketId);
                    ordenes.OrdenNumero = sum;
                    ordenes.Plan = plan;
                    db3.Entry(ordenes).State = EntityState.Modified;
                    db3.SaveChanges();
                    db3.Dispose();

                    return RedirectToAction("Details", new { id = orderTicket.OrderTicketId});
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

            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(orderTicket.CompanyId), "PlanCategoryId", "Categoria", orderTicket.PlanCategoryId);
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicket(orderTicket.CompanyId), "PlanTicketId", "Plan", orderTicket.PlanTicketId);
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(orderTicket.CompanyId), "ServerId", "Nombre", orderTicket.ServerId);
            return View(orderTicket);
        }

        // GET: OrderTickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderTicket orderTicket = db.OrderTickets.Find(id);
            if (orderTicket == null)
            {
                return HttpNotFound();
            }

            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(orderTicket.CompanyId), "PlanCategoryId", "Categoria", orderTicket.PlanCategoryId);
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicket(orderTicket.CompanyId), "PlanTicketId", "Plan", orderTicket.PlanTicketId);
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(orderTicket.CompanyId), "ServerId", "Nombre", orderTicket.ServerId);
            return View(orderTicket);
        }

        // POST: OrderTickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrderTicket orderTicket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderTicket).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();

                    var db3 = new NexxtVouContext();

                    var db5 = new NexxtVouContext();
                    var servidor = db5.Servers.Find(orderTicket.ServerId);
                    string ipservidor = servidor.IpServer;
                    db5.Dispose();

                    var db6 = new NexxtVouContext();
                    var planes = db6.PlanTickets.Find(orderTicket.PlanTicketId);
                    string plan = planes.Plan;
                    db6.Dispose();

                    var ordenes = db3.OrderTickets.Find(orderTicket.OrderTicketId);
                    ordenes.Plan = plan;
                    db3.Entry(ordenes).State = EntityState.Modified;
                    db3.SaveChanges();
                    db3.Dispose();

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

            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(orderTicket.CompanyId), "PlanCategoryId", "Categoria", orderTicket.PlanCategoryId);
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicket(orderTicket.CompanyId), "PlanTicketId", "Plan", orderTicket.PlanTicketId);
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(orderTicket.CompanyId), "ServerId", "Nombre", orderTicket.ServerId);
            return View(orderTicket);
        }

        // GET: OrderTickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderTicket orderTicket = db.OrderTickets.Find(id);
            if (orderTicket == null)
            {
                return HttpNotFound();
            }
            return View(orderTicket);
        }

        // POST: OrderTickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderTicket orderTicket = db.OrderTickets.Find(id);
            db.OrderTickets.Remove(orderTicket);
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
            return View(orderTicket);
        }


        public JsonResult GetPlanes(int categoryid)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var planticket = db.PlanTickets.Where(c => c.PlanCategoryId == categoryid);

            return Json(planticket);
        }

        public JsonResult GetPrecio(int planId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var planes = db.PlanTickets.Find(planId);
            decimal precio = planes.Precio;

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
