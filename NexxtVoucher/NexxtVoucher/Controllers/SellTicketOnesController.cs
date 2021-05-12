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

    public class SellTicketOnesController : Controller
    {
        private readonly NexxtVouContext db = new NexxtVouContext();

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
        public async Task<ActionResult> Index(int? page = null)
        {
            var user =await db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefaultAsync();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            page = (page ?? 1);

            var sellTicketOnes = db.SellTicketOnes.Where(c=> c.CompanyId == user.CompanyId)
                .Include(s => s.OrderTicketDetail)
                .Include(s => s.PlanCategory)
                .Include(s => s.PlanTicket)
                .Include(s => s.Server);

            return View(sellTicketOnes.OrderBy(o=> o.ServerId).ThenByDescending(o=> o.VentaOne).ToList().ToPagedList((int)page, 10));
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
        public async Task<ActionResult> Create(SellTicketOne sellTicketOne)
        {
            if (ModelState.IsValid)
            {                
                try
                {
                    db.SellTicketOnes.Add(sellTicketOne);
                    await db.SaveChangesAsync();

                    var db2 = new NexxtVouContext();
                    int sum = 0;
                    int Recep = 0;

                    var register = await db2.Registers.Where(c => c.CompanyId == sellTicketOne.CompanyId).FirstOrDefaultAsync();
                    Recep = register.VentaOne;
                    sum = Recep + 1;
                    register.VentaOne = sum;
                    db2.Entry(register).State = EntityState.Modified;
                    await db2.SaveChangesAsync();
                    
                    var sellticket = await db2.SellTicketOnes.FindAsync(sellTicketOne.SellTicketOneId);
                    sellticket.VentaOne = sum;
                    db2.Entry(sellticket).State = EntityState.Modified;
                    await db2.SaveChangesAsync();
                    
                    var orderticketdetail = await db2.OrderTicketDetails.FindAsync(sellTicketOne.OrderTicketDetailId);
                    orderticketdetail.Vendido = true;
                    orderticketdetail.Date = DateTime.Today;
                    orderticketdetail.VentaNumero = Convert.ToString(sum);
                    db2.Entry(orderticketdetail).State = EntityState.Modified;
                    await db2.SaveChangesAsync();

                    db2.Dispose();

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


        public JsonResult GetTickets(int PlanTicketId, int servidorId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var orderticketdeatil = db.OrderTicketDetails.Where(o => o.ServerId == servidorId && o.PlanTicketId == PlanTicketId && o.Vendido == false).FirstOrDefault();

            return Json(orderticketdeatil);
        }


        public JsonResult GetPrecio(int orderTicketdetailId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var orderdetail = db.OrderTicketDetails.Find(orderTicketdetailId);
            decimal precio = orderdetail.Precio;

            return Json(precio);
        }

        //TODO:Categoria sin Servidor
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
