
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

    [Authorize(Roles = "User")]
    public class PlanTicketsController : Controller
    {
        private readonly NexxtVouContext db = new NexxtVouContext();


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

        // GET: PlanTickets
        public async Task<ActionResult> Index(int? servidorid, int? page = null)
        {
            var user =await db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefaultAsync();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            page = (page ?? 1);

            if (servidorid != null)
            {
                var planTickets =db.PlanTickets.Where(c => c.CompanyId == user.CompanyId && c.ServerId == servidorid)
                    .Include(p => p.SpeedDown)
                    .Include(p => p.SpeedUp)
                    .Include(p => p.Tax)
                    .Include(p => p.TicketInactive)
                    .Include(p => p.TicketRefresh)
                    .Include(p => p.TicketTime)
                    .Include(p => p.PlanCategory);

                return View(planTickets.OrderByDescending(o => o.PlanCategory.Categoria).ThenByDescending(o2 => o2.Plan).ToList().ToPagedList((int)page, 20));
            }
            else
            {
                var planTickets = db.PlanTickets.Where(c => c.CompanyId == user.CompanyId)
                    .Include(p => p.SpeedDown)
                    .Include(p => p.SpeedUp)
                    .Include(p => p.Tax)
                    .Include(p => p.TicketInactive)
                    .Include(p => p.TicketRefresh)
                    .Include(p => p.TicketTime)
                    .Include(p => p.PlanCategory);

                return View(planTickets.OrderByDescending(o => o.PlanCategory.Categoria).ThenByDescending(o2 => o2.Plan).ToList().ToPagedList((int)page, 20));
            }
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
        public async Task<ActionResult> Create()
        {
            var user =await db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefaultAsync();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var planetickets = new PlanTicket 
            { 
                CompanyId = user.CompanyId,
                ContinueTime = true,
                IsActive = true
            };

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
        public async Task<ActionResult> Create(PlanTicket planTicket)
        {
            if (ModelState.IsValid)
            {
                db.PlanTickets.Add(planTicket);
                try
                {
                    
                    //se busca informacion del servidor
                    string ip;
                    string us;
                    string pss;
                    var servidor =await db.Servers.FindAsync(planTicket.ServerId);
                    ip = servidor.IpServer;
                    us = servidor.Usuario;
                    pss = servidor.Clave;

                    var puertos =await db.MikrotikControls.Where(p => p.ServerId == servidor.ServerId).FirstOrDefaultAsync();
                    int port = puertos.PuertoApi;
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    string tiempo;
                    string Iscript;
                    string IscriptConsumo;
                    var tickettiempo = await db.TicketTimes.FindAsync(planTicket.TicketTimeId);
                    tiempo = tickettiempo.TiempoTicket;
                    Iscript = tickettiempo.ScriptTicket;
                    IscriptConsumo = tickettiempo.ScriptTicketConsumo; 
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    string inactivo;
                    var ticketinactivo = await db.TicketInactives.FindAsync(planTicket.TicketInactiveId);
                    inactivo = ticketinactivo.TiempoInactivo;
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    string refrescar;
                    var ticketrefrescar = await db.TicketRefreshes.FindAsync(planTicket.TicketRefreshId);
                    refrescar = ticketrefrescar.TiempoRefrescar;
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    string Vup;
                    var velocidadup = await db.SpeedUps.FindAsync(planTicket.SpeedUpId);
                    Vup = velocidadup.VelocidadUp;
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    string Vdown;
                    var velocidaddown =await db.SpeedDowns.FindAsync(planTicket.SpeedDownId);
                    Vdown = velocidaddown.VelocidadDown;
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    bool Iproxy = planTicket.proxy;
                    bool Imaccookies = planTicket.macCookies;
                    bool Icontinuetime = planTicket.ContinueTime;

                    string IproxyYesNo = null;
                    string ImacCookiesYesNo = null;


                    if (Iproxy == true)
                    {
                        IproxyYesNo = "yes";
                    }
                    else
                    {
                        IproxyYesNo = "no";
                    }

                    if (Imaccookies == true)
                    {
                        ImacCookiesYesNo = "yes";
                    }
                    else
                    {
                        ImacCookiesYesNo = "no";
                    }

                    //;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
                    //Sistema de Nuevo en Mikrotik
                    MK mikrotik = new MK(ip, port);
                    if (!mikrotik.Login(us, pss))
                    {                      
                        //ModelState.AddModelError(string.Empty, @Resources.Resource.MikrotikFailed);
                    }
                    else
                    {
                        await db.SaveChangesAsync();

                        var db5 = new NexxtVouContext();

                        if (Icontinuetime == false)
                        {
                            Iscript = "";
                            mikrotik.Send("/system/scheduler/add");
                            mikrotik.Send("=name=" + planTicket.Plan);
                            mikrotik.Send("=interval=" + "30s");
                            mikrotik.Send("=policy=" + "ftp,reboot,read,write,policy,test,password,sniff,sensitive,romo");
                            mikrotik.Send("=start-time=" + "startup");
                            mikrotik.Send("=on-event=" + IscriptConsumo);
                            mikrotik.Send("/system/scheduler/print", true);

                            int ITimetotal = 0;
                            int ITimeRest = 0;
                            string ITimeIdmk;
                            string ITimeMikrotiIndex;

                            foreach (var item in mikrotik.Read())
                            {
                                ITimeIdmk = item;
                                ITimetotal = ITimeIdmk.Length;
                                ITimeRest = ITimetotal - 10;
                                ITimeMikrotiIndex = ITimeIdmk.Substring(10, ITimeRest);

                                var planticketUp = await db5.PlanTickets.FindAsync(planTicket.PlanTicketId);
                                planticketUp.MikrotikIdContinuo = ITimeMikrotiIndex;
                                db5.Entry(planticketUp).State = EntityState.Modified;
                                await db5.SaveChangesAsync();
                            }
                        }

                        mikrotik.Send("/ip/hotspot/user/profile/add");
                        mikrotik.Send("=name=" + planTicket.Plan);
                        mikrotik.Send("=session-timeout=" + tiempo);
                        mikrotik.Send("=keepalive-timeout=" + tiempo);
                        mikrotik.Send("=rate-limit=" + Vup + "/" + Vdown);
                        mikrotik.Send("=shared-users=" + planTicket.ShareUser);
                        mikrotik.Send("=idle-timeout=" + inactivo);
                        mikrotik.Send("=status-autorefresh=" + refrescar);
                        mikrotik.Send("=add-mac-cookie=" + ImacCookiesYesNo);
                        if (Imaccookies == false)
                        {
                            mikrotik.Send("=!mac-cookie-timeout=");
                        }
                        else
                        {
                            mikrotik.Send("=mac-cookie-timeout=" + tiempo);
                        }
                        mikrotik.Send("=transparent-proxy=" + IproxyYesNo);
                        mikrotik.Send("=on-login=" + Iscript);
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

                            var planticketUp = await db5.PlanTickets.FindAsync(planTicket.PlanTicketId);
                            planticketUp.MikrotikId = mikrotiIndex;
                            db5.Entry(planticketUp).State = EntityState.Modified;
                            await db5.SaveChangesAsync();
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
                    string ip;
                    string us;
                    string pss;
                    var servidor = db.Servers.Find(planTicket.ServerId);
                    ip = servidor.IpServer;
                    us = servidor.Usuario;
                    pss = servidor.Clave;

                    var puertos = db.MikrotikControls.Where(p => p.ServerId == servidor.ServerId).FirstOrDefault();
                    int port = puertos.PuertoApi;
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    string tiempo;
                    string Iscript;
                    string IscriptConsumo;
                    var tickettiempo = db.TicketTimes.Find(planTicket.TicketTimeId);
                    tiempo = tickettiempo.TiempoTicket;
                    Iscript = tickettiempo.ScriptTicket;
                    IscriptConsumo = tickettiempo.ScriptTicketConsumo;
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    string inactivo;
                    var ticketinactivo = db.TicketInactives.Find(planTicket.TicketInactiveId);
                    inactivo = ticketinactivo.TiempoInactivo;
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    string refrescar;
                    var ticketrefrescar = db.TicketRefreshes.Find(planTicket.TicketRefreshId);
                    refrescar = ticketrefrescar.TiempoRefrescar;
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    string Vup;
                    var velocidadup = db.SpeedUps.Find(planTicket.SpeedUpId);
                    Vup = velocidadup.VelocidadUp;
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    //se busca informacion del servidor
                    string Vdown;
                    var velocidaddown = db.SpeedDowns.Find(planTicket.SpeedDownId);
                    Vdown = velocidaddown.VelocidadDown;
                    //:::::::::::::::::::::::::::::::::::::::::::::

                    bool Iproxy = planTicket.proxy;
                    bool Imaccookies = planTicket.macCookies;
                    bool Icontinuetime = planTicket.ContinueTime;

                    string IproxyYesNo = null;
                    string ImacCookiesYesNo = null;

                    if (Iproxy == true)
                    {
                        IproxyYesNo = "yes";
                    }
                    else
                    {
                        IproxyYesNo = "no";
                    }

                    if (Imaccookies == true)
                    {
                        ImacCookiesYesNo = "yes";
                    }
                    else
                    {
                        ImacCookiesYesNo = "no";
                    }

                    MK mikrotik = new MK(ip, port);
                    if (!mikrotik.Login(us, pss))
                    {
                        //ModelState.AddModelError(string.Empty, @Resources.Resource.MikrotikFailed);
                    }
                    else
                    {
                        db.SaveChanges();
                        if (Icontinuetime == false)
                        {
                            Iscript = "";
                            mikrotik.Send("/system/scheduler/set");
                            mikrotik.Send("=.id=" + planTicket.MikrotikIdContinuo);
                            mikrotik.Send("=name=" + planTicket.Plan);
                            mikrotik.Send("=interval=" + "30s");
                            mikrotik.Send("=policy=" + "ftp,reboot,read,write,policy,test,password,sniff,sensitive,romo");
                            mikrotik.Send("=start-time=" + "startup");
                            mikrotik.Send("=on-event=" + IscriptConsumo);
                            mikrotik.Send("/system/scheduler/print", true);

                            int ITimetotal = 0;
                            int ITimeRest = 0;
                            string ITimeIdmk;

                            foreach (var item in mikrotik.Read())
                            {
                                ITimeIdmk = item;
                                ITimetotal = ITimeIdmk.Length;
                                ITimeRest = ITimetotal - 10;
                            }
                        }

                        mikrotik.Send("/ip/hotspot/user/profile/set");
                        mikrotik.Send("=.id=" + planTicket.MikrotikId);
                        mikrotik.Send("=name=" + planTicket.Plan);
                        mikrotik.Send("=session-timeout=" + tiempo);
                        mikrotik.Send("=keepalive-timeout=" + tiempo);
                        mikrotik.Send("=rate-limit=" + Vup + "/" + Vdown);
                        mikrotik.Send("=shared-users=" + planTicket.ShareUser);
                        mikrotik.Send("=idle-timeout=" + inactivo);
                        mikrotik.Send("=status-autorefresh=" + refrescar);
                        mikrotik.Send("=add-mac-cookie=" + ImacCookiesYesNo);
                        if (Imaccookies == false)
                        {
                            mikrotik.Send("=!mac-cookie-timeout=");
                        }
                        else
                        {
                            mikrotik.Send("=mac-cookie-timeout=" + tiempo);
                        }
                        mikrotik.Send("=transparent-proxy=" + IproxyYesNo);
                        mikrotik.Send("=on-login=" + Iscript);
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

                var puertos = db2.MikrotikControls.Where(p => p.ServerId == servidor.ServerId).FirstOrDefault();
                int port = puertos.PuertoApi;

                db2.Dispose();
                //:::::::::::::::::::::::::::::::::::::::::::::

                MK mikrotik = new MK(ip, port);
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

        //TODO:Arreglar el listado Cateogira sin Servidor
        //public JsonResult GetCategory(int ServerId)
        //{
        //    db.Configuration.ProxyCreationEnabled = false;
        //    var categories = db.PlanCategories.Where(c => c.ServerId == ServerId).ToList();

        //    return Json(categories);
        //}

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
