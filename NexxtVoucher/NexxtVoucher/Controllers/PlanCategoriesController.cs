﻿namespace NexxtVoucher.Controllers
{
    using NexxtVoucher.Models;
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [Authorize(Roles = "User")]

    public class PlanCategoriesController : Controller
    {
        private readonly NexxtVouContext db = new NexxtVouContext();

        // GET: PlanCategories
        public async Task<ActionResult> Index()
        {
            var user = await db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefaultAsync();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var planCategories = db.PlanCategories.Where(c=> c.CompanyId == user.CompanyId);

            return View(planCategories.OrderBy(o => o.Categoria).ToList());
        }

        // GET: PlanCategories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var planCategory =await db.PlanCategories.FindAsync(id);
            if (planCategory == null)
            {
                return HttpNotFound();
            }
            return View(planCategory);
        }

        // GET: PlanCategories/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var plancategories = new PlanCategory { CompanyId = user.CompanyId };

            return View(plancategories);
        }

        // POST: PlanCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PlanCategory planCategory)
        {
            if (ModelState.IsValid)
            {
                db.PlanCategories.Add(planCategory);
                try
                {
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

            //ViewBag.ServerId = new SelectList(ComboHelper.GetServer(planCategory.CompanyId), "ServerId", "Nombre", planCategory.ServerId);
            return View(planCategory);
        }

        // GET: PlanCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var planCategory = db.PlanCategories.Find(id);
            if (planCategory == null)
            {
                return HttpNotFound();
            }

            //ViewBag.ServerId = new SelectList(ComboHelper.GetServer(planCategory.CompanyId), "ServerId", "Nombre", planCategory.ServerId);
            return View(planCategory);
        }

        // POST: PlanCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PlanCategory planCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(planCategory).State = EntityState.Modified;
                try
                {
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

            //ViewBag.ServerId = new SelectList(ComboHelper.GetServer(planCategory.CompanyId), "ServerId", "Nombre", planCategory.ServerId);
            return View(planCategory);
        }

        // GET: PlanCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var planCategory = db.PlanCategories.Find(id);
            if (planCategory == null)
            {
                return HttpNotFound();
            }
            return View(planCategory);
        }

        // POST: PlanCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PlanCategory planCategory =await db.PlanCategories.FindAsync(id);
            db.PlanCategories.Remove(planCategory);
            try
            {
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
            return View(planCategory);
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
