namespace NexxtVoucher.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using NexxtVoucher.Classes;
    using NexxtVoucher.Models;

    [Authorize(Roles = "User")]
    public class MikrotikControlsController : Controller
    {
        private readonly NexxtVouContext db = new NexxtVouContext();

        // GET: MikrotikControls
        public async Task<ActionResult> Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var mikrotikControls = db.MikrotikControls.Where(c => c.CompanyId == user.CompanyId)
                .Include(m => m.Server);

            return View(await mikrotikControls.ToListAsync());
        }

        // GET: MikrotikControls/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mikrotikControl = await db.MikrotikControls.FindAsync(id);
            if (mikrotikControl == null)
            {
                return HttpNotFound();
            }
            return View(mikrotikControl);
        }

        // GET: MikrotikControls/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var mikrotikcontrol = new MikrotikControl
            {
                CompanyId = user.CompanyId
            };

            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(user.CompanyId), "ServerId", "Nombre");

            return View(mikrotikcontrol);
        }

        // POST: MikrotikControls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(MikrotikControl mikrotikControl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.MikrotikControls.Add(mikrotikControl);
                    await db.SaveChangesAsync();
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

            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(mikrotikControl.CompanyId), "ServerId", "Nombre", mikrotikControl.ServerId);
            return View(mikrotikControl);
        }

        // GET: MikrotikControls/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mikrotikControl = await db.MikrotikControls.FindAsync(id);
            if (mikrotikControl == null)
            {
                return HttpNotFound();
            }
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(mikrotikControl.CompanyId), "ServerId", "Nombre", mikrotikControl.ServerId);
            return View(mikrotikControl);
        }

        // POST: MikrotikControls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(MikrotikControl mikrotikControl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(mikrotikControl).State = EntityState.Modified;
                    await db.SaveChangesAsync();
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
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(mikrotikControl.CompanyId), "ServerId", "Nombre", mikrotikControl.ServerId);
            return View(mikrotikControl);
        }

        // GET: MikrotikControls/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mikrotikControl = await db.MikrotikControls.FindAsync(id);
            if (mikrotikControl == null)
            {
                return HttpNotFound();
            }
            return View(mikrotikControl);
        }

        // POST: MikrotikControls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var mikrotikControl = await db.MikrotikControls.FindAsync(id);

            try
            {
                db.MikrotikControls.Remove(mikrotikControl);
                await db.SaveChangesAsync();
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
            return View(mikrotikControl);
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
