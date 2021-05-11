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
using PagedList;

namespace NexxtVoucher.Controllers
{
    [Authorize(Roles = "User")]
    public class ServersController : Controller
    {
        private NexxtVouContext db = new NexxtVouContext();


        // GET: Servers/Connect
        public ActionResult MikroSuccess(string ip, string us, string pss, int idserver)
        {
            var puertos = db.MikrotikControls.Where(p => p.ServerId == idserver).FirstOrDefault();
            int port = puertos.PuertoApi;

            MK mikrotik = new MK(ip, port);
            if (!mikrotik.Login(us, pss))
            {
                mikrotik.Close();
                ModelState.AddModelError(string.Empty, @Resources.Resource.MikrotikFailed);
            }
            else
            {
                mikrotik.Close();
                ModelState.AddModelError(string.Empty, @Resources.Resource.MikrotikSuccess);
            }

            var server = new Server { ServerId = idserver };
            return PartialView(server);
        }


        // GET: Servers
        public ActionResult Index(int? page = null)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            page = (page ?? 1);

            var servers = db.Servers.Where(C=> C.CompanyId == user.CompanyId)
                .Include(s => s.City)
                .Include(s => s.Zone);

            return View(servers.ToList().ToPagedList((int)page, 10));
        }

        // GET: Servers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Server server = db.Servers.Find(id);
            if (server == null)
            {
                return HttpNotFound();
            }
            return View(server);
        }

        // GET: Servers/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var server = new Server { CompanyId = user.CompanyId };

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(user.CompanyId), "CityId", "Ciudad");
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(user.CompanyId), "ZoneId", "Zona");

            return View(server);
        }

        // POST: Servers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Server server)
        {
            if (ModelState.IsValid)
            {
                db.Servers.Add(server);
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

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(server.CompanyId), "CityId", "Ciudad", server.CityId);
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(server.CompanyId), "ZoneId", "Zona", server.ZoneId);

            return View(server);
        }

        // GET: Servers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Server server = db.Servers.Find(id);
            if (server == null)
            {
                return HttpNotFound();
            }

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(server.CompanyId), "CityId", "Ciudad", server.CityId);
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(server.CompanyId), "ZoneId", "Zona", server.ZoneId);

            return View(server);
        }

        // POST: Servers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Server server)
        {
            if (ModelState.IsValid)
            {
                db.Entry(server).State = EntityState.Modified;
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

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(server.CompanyId), "CityId", "Ciudad", server.CityId);
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(server.CompanyId), "ZoneId", "Zona", server.ZoneId);

            return View(server);
        }

        // GET: Servers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Server server = db.Servers.Find(id);
            if (server == null)
            {
                return HttpNotFound();
            }
            return View(server);
        }

        // POST: Servers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Server server = db.Servers.Find(id);
            db.Servers.Remove(server);
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
            return View(server);
        }

        public JsonResult GetZone(int cityId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var zones = db.Zones.Where(c => c.CityId == cityId);
            return Json(zones);
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
