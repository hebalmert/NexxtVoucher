using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using NexxtVoucher.Classes;
using NexxtVoucher.Models;
using PagedList;

namespace NexxtVoucher.Controllers
{
    [Authorize(Roles = "User")]

    public class SellTicketsController : Controller
    {
        private NexxtVouContext db = new NexxtVouContext();

        public IList<SellTicketDetail> GetSellTicketList(int id)
        {
            //DbAccessContext db = new DbAccessContext();
            //var employeeList = (from e in db.Employees
            //                    join d in db.Departments on e.DepartmentId equals d.DepartmentId
            //                    select new EmployeeViewModel
            //                    {
            //                        Name = e.Name,
            //                        Email = e.Email,
            //                        Age = (int)e.Age,
            //                        Address = e.Address,
            //                        Department = d.DepartmentName
            //                    }).ToList();
            var selltickets = db.SellTicketDetails.Where(o => o.SellTicketId == id).ToList();
            return selltickets;
        }

        public ActionResult Lista(int id)
        {

            return View(this.GetSellTicketList(id));
        }

        public ActionResult ExportToExcel(int seid)
        {
            var gv = new GridView();
            gv.DataSource = this.GetSellTicketList(seid);
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
            gv.RenderControl(objHtmlTextWriter);
            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();
            //return View("Index");
            return RedirectToAction("Details", new { id = seid });
        }

        // GET: SellTickets/Edit/5
        public ActionResult AddTicket(int planid, int serid, int VentaN, int sellid, int com)
        {
            int ticketid = 0;

            var tickets = db.OrderTicketDetails.Where(o => o.ServerId == serid && o.PlanTicketId == planid && o.Vendido == false).ToList();
            if (tickets.Count != 0)
            {
                var db2 = new NexxtVouContext();

                foreach (var item in tickets)
                {
                    item.VentaNumero =Convert.ToString(VentaN);
                    item.Vendido = true;
                    ticketid = item.OrderTicketDetailId;
                    db.SaveChanges();

                    var sellticketdetail = new SellTicketDetail
                    {
                        CompanyId = com,
                        SellTicketId = sellid,
                        OrderTicketDetailId = ticketid,
                        Plan = item.PlanTicket.Plan,
                        Usuario = item.Usuario,
                        TicketNumero = item.TicketNumero,
                        Precio = item.Precio
                    };
                    db2.SellTicketDetails.Add(sellticketdetail);
                    db2.SaveChanges();
                }
                db2.Dispose();
            }

            return RedirectToAction("Details", new { id = sellid });
        }

        // GET: SellTickets
        public ActionResult Index(int? page = null)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            page = (page ?? 1);

            var sellTickets = db.SellTickets.Where(o=> o.CompanyId == user.CompanyId)
                .Include(s => s.PlanCategory)
                .Include(s => s.PlanTicket)
                .Include(s => s.Server);
            return View(sellTickets.OrderBy(o=> o.ServerId).ThenByDescending(o=> o.VentaOne).ToList().ToPagedList((int)page, 10));
        }

        // GET: SellTickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SellTicket sellTicket = db.SellTickets.Find(id);
            if (sellTicket == null)
            {
                return HttpNotFound();
            }
            return View(sellTicket);
        }

        // GET: SellTickets/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var sellticket = new SellTicket
            {
                CompanyId = user.CompanyId,
                Date = DateTime.Today
            };

            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(user.CompanyId), "PlanCategoryId", "Categoria");
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicket(user.CompanyId), "PlanTicketId", "Plan");
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(user.CompanyId), "ServerId", "Nombre");

            return View(sellticket);
        }

        // POST: SellTickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SellTicket sellTicket)
        {
            if (ModelState.IsValid)
            {
                db.SellTickets.Add(sellTicket);
                try
                {
                    db.SaveChanges();

                    var db2 = new NexxtVouContext();
                    var db3 = new NexxtVouContext();
                    int sum = 0;
                    int Recep = 0;

                    var register = db2.Registers.Where(c => c.CompanyId == sellTicket.CompanyId).FirstOrDefault();
                    Recep = register.VentaOne;
                    sum = Recep + 1;
                    register.VentaOne = sum;
                    db2.Entry(register).State = EntityState.Modified;
                    db2.SaveChanges();

                    var sellticket = db3.SellTickets.Find(sellTicket.SellTicketId);
                    sellticket.VentaOne = sum;
                    db3.Entry(sellticket).State = EntityState.Modified;
                    db3.SaveChanges();

                    db2.Dispose();
                    db3.Dispose();

                    return RedirectToAction("Details", new { id = sellTicket.SellTicketId });
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

            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(sellTicket.CompanyId), "PlanCategoryId", "Categoria", sellTicket.PlanCategoryId);
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicket(sellTicket.CompanyId), "PlanTicketId", "Plan", sellTicket.PlanTicketId);
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(sellTicket.CompanyId), "ServerId", "Nombre", sellTicket.ServerId);

            return View(sellTicket);
        }

        // GET: SellTickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SellTicket sellTicket = db.SellTickets.Find(id);
            if (sellTicket == null)
            {
                return HttpNotFound();
            }

            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(sellTicket.CompanyId), "PlanCategoryId", "Categoria", sellTicket.PlanCategoryId);
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicket(sellTicket.CompanyId), "PlanTicketId", "Plan", sellTicket.PlanTicketId);
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(sellTicket.CompanyId), "ServerId", "Nombre", sellTicket.ServerId);

            return View(sellTicket);
        }

        // POST: SellTickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SellTicket sellTicket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sellTicket).State = EntityState.Modified;
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

            ViewBag.PlanCategoryId = new SelectList(ComboHelper.GetPlancategory(sellTicket.CompanyId), "PlanCategoryId", "Categoria", sellTicket.PlanCategoryId);
            ViewBag.PlanTicketId = new SelectList(ComboHelper.GetPlanTicket(sellTicket.CompanyId), "PlanTicketId", "Plan", sellTicket.PlanTicketId);
            ViewBag.ServerId = new SelectList(ComboHelper.GetServer(sellTicket.CompanyId), "ServerId", "Nombre", sellTicket.ServerId);

            return View(sellTicket);
        }

        // GET: SellTickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SellTicket sellTicket = db.SellTickets.Find(id);
            if (sellTicket == null)
            {
                return HttpNotFound();
            }
            return View(sellTicket);
        }

        // POST: SellTickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SellTicket sellTicket = db.SellTickets.Find(id);
            db.SellTickets.Remove(sellTicket);
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
            return View(sellTicket);
        }

        public JsonResult GetPlanes(int categoryid)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var planticket = db.PlanTickets.Where(c => c.PlanCategoryId == categoryid);

            return Json(planticket);
        }

        public JsonResult GetPrecio(int planticketId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var planes = db.PlanTickets.Find(planticketId);
            decimal precio = planes.Precio;

            return Json(precio);
        }

        public JsonResult GetCategory(int ServerId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var categories = db.PlanCategories.Where(c => c.ServerId == ServerId).ToList();

            return Json(categories);
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
