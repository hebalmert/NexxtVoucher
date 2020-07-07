using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NexxtVoucher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NexxtVoucher.Controllers
{
    public class HomeController : Controller
    {
        private NexxtVouContext db = new NexxtVouContext();

        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (user != null)
            {
                var db2 = new NexxtVouContext();
                var companyUp = db2.Companies.Find(user.CompanyId);
                bool comActivo = companyUp.Activo;
                db2.Dispose();

                DateTime hoy = DateTime.Today;
                DateTime current = companyUp.DateHasta;
                if (comActivo == false)
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    ModelState.AddModelError("Error", "La Cuenta Caduco o esta Bloqueada !!!");

                    return RedirectToAction("Login", "Account");
                }

                if (current <= hoy)
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    ModelState.AddModelError("Error", "La Cuenta Caduco o esta Bloqueada !!!");

                    return RedirectToAction("Login", "Account");
                }
            }

            return View(user);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
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