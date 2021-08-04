namespace NexxtVoucher.Controllers
{
    using NexxtVoucher.Classes;
    using NexxtVoucher.Models;
    using PagedList;
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class OrderTicketsController : Controller
    {
        private readonly NexxtVouContext db = new NexxtVouContext();


        public async Task<ActionResult> PrintReportDate()
        {
            var user = await db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefaultAsync();

            var printviewcajero = new PrintReportDate
            {
                CompanyId = user.CompanyId,
                DateFin = DateTime.Today,
                DateInicio = DateTime.Today
            };

            return View(printviewcajero);
        }

        // Post: PrintCxCReport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrintReportDate(PrintReportDate printreportdate)
        {

            return RedirectToAction("PrintReport", new { fechaInicio = printreportdate.DateInicio, fechafin = printreportdate.DateFin});
        }

        // GET: PaymentChachiers
        public async Task<ActionResult> PrintReport(DateTime fechaInicio, DateTime fechafin)
        {
            var user = await db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefaultAsync();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var orderticket = db.OrderTicketDetails.Where(c => c.CompanyId == user.CompanyId && c.Date >= fechaInicio && c.Date <= fechafin && c.Vendido == true)
                .Include(s => s.Cachier)
                .Include(s => s.PlanCategory)
                .Include(s => s.PlanTicket)
                .Include(s => s.Server);

            return View(orderticket.OrderByDescending(c => c.Date).ToList());
        }


        [HttpPost]
        public JsonResult Search(string Prefix)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var servidores = (from servidor in db.Servers
                              where servidor.Nombre.StartsWith(Prefix) && servidor.CompanyId == user.CompanyId
                              select new
                              {
                                  label = servidor.Nombre,
                                  val = servidor.ServerId
                              }).ToList();

            return Json(servidores);

        }


        // GET: OrderTickets
        public async Task<ActionResult> AddTicket(int id, string up, string down) //id = OrderTicketId
        {
            var ordenticket = await db.OrderTickets.FindAsync(id);
            int can = ordenticket.Cantidad;
            int planid = ordenticket.PlanTicketId;
            string plan =ordenticket.Plan;
            string tiempo = ordenticket.PlanTicket.TicketTime.TiempoTicket;
            string ip = ordenticket.Server.IpServer;
            int Serv = ordenticket.ServerId;
            int catego = ordenticket.PlanCategoryId;
            decimal prec = ordenticket.Precio;
            int conteo = 0;

            var db2 = new NexxtVouContext();
            var servidordata = await db2.Servers.FindAsync(Serv);
            string us = servidordata.Usuario;
            string pss = servidordata.Clave;

            var puertos = await db2.MikrotikControls.Where(p => p.ServerId == Serv).FirstOrDefaultAsync();
            int port = puertos.PuertoApi;

            var cadenas = await db2.ChainCodes.Where(c => c.CompanyId == ordenticket.CompanyId).FirstOrDefaultAsync();
            int largocadena = cadenas.Largo;
            string caracteres = cadenas.Cadena;
            string codigoTicket;
            db2.Dispose();

            //Se hace con conexion a la Mikroti y se deja abierto
            ////////////////////////////////////////////////////////////
            MK mikrotik = new MK(ip, port);
            if (!mikrotik.Login(us, pss))
            {
                return RedirectToAction("Index");
            }
            ///////////////////////////////////////////////////////////////////////////////////

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

                var register = await db3.Registers.Where(c => c.CompanyId == ordenticket.CompanyId).FirstOrDefaultAsync();
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
                    
                }
                await db4.SaveChangesAsync();
                await db3.SaveChangesAsync();
                conteo += 1;
            }

            mikrotik.Close();

            db3.Dispose();  //conexion para el Registro de Consecutivos
            db4.Dispose();  //Se cierra la crecion del ticket en el OrderTicketDetail
            db11.Dispose();  //Se cierra la crecion del ticket en el OrderTicketDetail

            ordenticket.Creados = conteo;
            ordenticket.Mikrotik = true;

            db.Entry(ordenticket).State = EntityState.Modified;
            await db.SaveChangesAsync();
            //var db10 = new NexxtVouContext();
            //var orderticketup = db10.OrderTickets.Find(id);
            //orderticketup.Creados = conteo;
            //orderticketup.Mikrotik = true;
            //db10.Entry(orderticketup).State = EntityState.Modified;
            //db10.SaveChanges();
            //db10.Dispose();

            return RedirectToAction("Details", new { id = ordenticket.OrderTicketId });
        }

        [HttpPost]
        public JsonResult Search2(string Prefix)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var ticketdetail = (from codigos in db.OrderTicketDetails
                              where codigos.Usuario.StartsWith(Prefix) && codigos.CompanyId == user.CompanyId
                              select new
                              {
                                  label = codigos.Usuario,
                                  val = codigos.OrderTicketDetailId
                              }).ToList();

            return Json(ticketdetail);

        }

        // GET: OrderTickets
        public ActionResult IndexView(int? ticketdetailid, int? page = null)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            page = (page ?? 1);

            if (ticketdetailid != null)
            {
                var orderTicketsdetail = db.OrderTicketDetails.Where(c => c.CompanyId == user.CompanyId && c.OrderTicketDetailId == ticketdetailid)
                    .Include(o => o.PlanCategory)
                    .Include(o => o.PlanTicket)
                    .Include(o => o.OrderTicket)
                    .Include(o => o.Server);
                return View(orderTicketsdetail.OrderByDescending(o => o.OrderTicket.Date).ToList().ToPagedList((int)page, 20));
            }
            else
            {
                var orderTicketsdetail = db.OrderTicketDetails.Where(c => c.CompanyId == user.CompanyId)
                    .Include(o => o.PlanCategory)
                    .Include(o => o.PlanTicket)
                    .Include(o => o.OrderTicket)
                    .Include(o => o.Server);
                return View(orderTicketsdetail.OrderByDescending(o => o.Date).ToList().ToPagedList((int)page, 20));
            }
        }

        // GET: OrderTickets
        public ActionResult Index(int? servidorid, int? page = null)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            page = (page ?? 1);

            if (servidorid != null)
            {
                var orderTickets = db.OrderTickets.Where(c => c.CompanyId == user.CompanyId && c.ServerId == servidorid)
                    .Include(o => o.PlanCategory)
                    .Include(o => o.PlanTicket)
                    .Include(o => o.Server);
                return View(orderTickets.OrderByDescending(o => o.OrdenNumero).ToList().ToPagedList((int)page, 20));
            }
            else
            {
                var orderTickets = db.OrderTickets.Where(c => c.CompanyId == user.CompanyId)
                    .Include(o => o.PlanCategory)
                    .Include(o => o.PlanTicket)
                    .Include(o => o.Server);
                return View(orderTickets.OrderByDescending(o => o.OrdenNumero).ToList().ToPagedList((int)page, 20));
            }
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

        // GET: OrderTickets/Details/5
        public ActionResult DetailView(int? id)
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


        public JsonResult GetCategories(int companyid)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var categoria = db.PlanCategories.Where(c => c.CompanyId == companyid);
           
            return Json(categoria);
        }

        public JsonResult GetPlanes(int categoryid, int serverid)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var planticket = db.PlanTickets.Where(c => c.PlanCategoryId == categoryid  && c.ServerId == serverid);

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
