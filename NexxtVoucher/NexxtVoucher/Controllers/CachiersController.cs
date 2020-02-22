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
    public class CachiersController : Controller
    {
        private NexxtVouContext db = new NexxtVouContext();

        // GET: Cachiers
        public ActionResult Index(string searchString, int? page = null)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            page = (page ?? 1);

            if (!String.IsNullOrEmpty(searchString))
            {
                var cajero = db.Cachiers.Where(c => c.CompanyId == user.CompanyId && c.FullName.Contains(searchString))
                .Include(t => t.City)
                .Include(t => t.Identification)
                .Include(t => t.Zone);
                return View(cajero.ToList().ToPagedList((int)page, 10));
            }
            else
            {
                var cajero = db.Cachiers.Where(c => c.CompanyId == user.CompanyId)
                .Include(t => t.City)
                .Include(t => t.Identification)
                .Include(t => t.Zone);

                return View(cajero.ToList().ToPagedList((int)page, 10));
            }
        }

        // GET: Cachiers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cachier cachier = db.Cachiers.Find(id);
            if (cachier == null)
            {
                return HttpNotFound();
            }
            return View(cachier);
        }

        // GET: Cachiers/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var cajeros = new Cachier
            {
                CompanyId = user.CompanyId,
                Activo = true
            };

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(user.CompanyId), "CityId", "Ciudad");
            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(user.CompanyId), "IdentificationId", "TipoDocumento");
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(user.CompanyId), "ZoneId", "Zona");

            return View(cajeros);
        }

        // POST: Cachiers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cachier cachier)
        {
            if (ModelState.IsValid)
            {
                db.Cachiers.Add(cachier);
                try
                {
                    if (cachier.Activo == true)
                    {
                        cachier.FullName = cachier.FirstName + " " + cachier.LastName;

                        db.Cachiers.Add(cachier);
                        db.SaveChanges();

                        UsersHelper.CreateUserASP(cachier.UserName, "Cobros");

                        var db2 = new NexxtVouContext();
                        var usuario = new User
                        {
                            UserName = cachier.UserName,
                            FirstName = cachier.FirstName,
                            LastName = cachier.LastName,
                            Phone = cachier.Phone,
                            Address = cachier.Address,
                            Puesto = "Cobros",
                            CompanyId = cachier.CompanyId
                        };
                        db2.Users.Add(usuario);
                        db2.SaveChanges();
                        db2.Dispose();

                        return RedirectToAction("Index");
                    }
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

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(cachier.CompanyId), "CityId", "Ciudad", cachier.CityId);
            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(cachier.CompanyId), "IdentificationId", "TipoDocumento", cachier.IdentificationId);
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(cachier.CompanyId), "ZoneId", "Zona", cachier.ZoneId);

            return View(cachier);
        }

        // GET: Cachiers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cachier = db.Cachiers.Find(id);
            if (cachier == null)
            {
                return HttpNotFound();
            }

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(cachier.CompanyId), "CityId", "Ciudad", cachier.CityId);
            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(cachier.CompanyId), "IdentificationId", "TipoDocumento", cachier.IdentificationId);
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(cachier.CompanyId), "ZoneId", "Zona", cachier.ZoneId);

            return View(cachier);
        }

        // POST: Cachiers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cachier cachier)
        {
            if (ModelState.IsValid)
            {
                cachier.FullName = cachier.FirstName + " " + cachier.LastName;
                db.Entry(cachier).State = EntityState.Modified;
                try
                {
                    if (cachier.Activo == true)
                    {
                        var db2 = new NexxtVouContext();
                        var currentTech = db2.Cachiers.Find(cachier.CachierId);
                        if (currentTech.UserName != cachier.UserName)
                        {
                            var db3 = new NexxtVouContext();
                            var usuarios = db3.Users.Where(c => c.UserName == currentTech.UserName && c.FirstName == currentTech.FirstName && c.LastName == currentTech.LastName).FirstOrDefault();
                            if (usuarios != null)
                            {
                                usuarios.UserName = cachier.UserName;
                                usuarios.FirstName = cachier.FirstName;
                                usuarios.LastName = cachier.LastName;
                                usuarios.Phone = cachier.Phone;
                                usuarios.Address = cachier.Address;
                                usuarios.Puesto = "Cobros";
                                usuarios.CompanyId = cachier.CompanyId;

                                db3.Entry(usuarios).State = EntityState.Modified;
                                db3.SaveChanges();
                                db3.Dispose();

                                UsersHelper.UpdateUserName(currentTech.UserName, cachier.UserName);
                            }
                            else
                            {
                                var db4 = new NexxtVouContext();
                                var usuario = new User
                                {
                                    UserName = cachier.UserName,
                                    FirstName = cachier.FirstName,
                                    LastName = cachier.LastName,
                                    Phone = cachier.Phone,
                                    Address = cachier.Address,
                                    Puesto = "Cobros",
                                    CompanyId = cachier.CompanyId
                                };
                                db4.Users.Add(usuario);
                                db4.SaveChanges();
                                db4.Dispose();
                                UsersHelper.CreateUserASP(cachier.UserName, "Cobros");
                            }
                            
                        }
                        else
                        {
                            var db3 = new NexxtVouContext();
                            var usuarios = db3.Users.Where(c => c.UserName == currentTech.UserName && c.FirstName == currentTech.FirstName && c.LastName == currentTech.LastName).FirstOrDefault();
                            if (usuarios != null)
                            {
                                usuarios.UserName = cachier.UserName;
                                usuarios.FirstName = cachier.FirstName;
                                usuarios.LastName = cachier.LastName;
                                usuarios.Phone = cachier.Phone;
                                usuarios.Address = cachier.Address;
                                usuarios.Puesto = "Cobros";
                                usuarios.CompanyId = cachier.CompanyId;
                                usuarios.UserName = cachier.UserName;
                                db3.Entry(usuarios).State = EntityState.Modified;
                                db3.SaveChanges();
                                db3.Dispose();
                            }
                            else
                            {
                                var db4 = new NexxtVouContext();
                                var usuario = new User
                                {
                                    UserName = cachier.UserName,
                                    FirstName = cachier.FirstName,
                                    LastName = cachier.LastName,
                                    Phone = cachier.Phone,
                                    Address = cachier.Address,
                                    Puesto = "Cobros",
                                    CompanyId = cachier.CompanyId
                                };
                                db4.Users.Add(usuario);
                                db4.SaveChanges();
                                db4.Dispose();
                                UsersHelper.CreateUserASP(cachier.UserName, "Cobros");
                            }
                            
                        }
                    }

                    if (cachier.Activo == false)
                    {
                        var db5 = new NexxtVouContext();
                        var currentTech2 = db5.Cachiers.Find(cachier.CachierId);

                        var db6 = new NexxtVouContext();
                        var usuarios = db6.Users.Where(c => c.UserName == currentTech2.UserName && c.FirstName == currentTech2.FirstName && c.LastName == currentTech2.LastName).FirstOrDefault();
                        if (usuarios != null)
                        {
                            db6.Users.Remove(usuarios);
                            db6.SaveChanges();
                            db6.Dispose();
                        }
                        UsersHelper.DeleteUser(currentTech2.UserName);

                        db5.Dispose();
                    }
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

            ViewBag.CityId = new SelectList(ComboHelper.GetCities(cachier.CompanyId), "CityId", "Ciudad", cachier.CityId);
            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(cachier.CompanyId), "IdentificationId", "TipoDocumento", cachier.IdentificationId);
            ViewBag.ZoneId = new SelectList(ComboHelper.GetZone(cachier.CompanyId), "ZoneId", "Zona", cachier.ZoneId);

            return View(cachier);
        }

        // GET: Cachiers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cachier = db.Cachiers.Find(id);
            if (cachier == null)
            {
                return HttpNotFound();
            }
            return View(cachier);
        }

        // POST: Cachiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var cachier = db.Cachiers.Find(id);
            db.Cachiers.Remove(cachier);
            try
            {
                db.SaveChanges();

                UsersHelper.DeleteUser(cachier.UserName);

                var db6 = new NexxtVouContext();
                var usuarios = db6.Users.Where(c => c.UserName == cachier.UserName && c.FirstName == cachier.FirstName && c.LastName == cachier.LastName).FirstOrDefault();
                if (usuarios != null)
                {
                    db6.Users.Remove(usuarios);
                    db6.SaveChanges();
                    db6.Dispose();
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
            return View(cachier);
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
