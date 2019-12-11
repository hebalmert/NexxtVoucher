using NexxtVoucher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexxtVoucher.Classes
{
    public class ComboHelper : IDisposable
    {
        private static NexxtVouContext db = new NexxtVouContext();

        //Combos de Compañias directo
        public static List<Company> GetCompanies()
        {
            var companies = db.Companies.ToList();
            companies.Add(new Company
            {
                CompanyId = 0,
                Compania = @Resources.Resource.ComboSelect,
            });
            return companies.OrderBy(d => d.Compania).ToList();
        }
        
        //Combos de Ciudades
        public static List<City> GetCities(int companyid)
        {
            var cities = db.Cities.Where(c => c.CompanyId == companyid).ToList();
            cities.Add(new City
            {
                CityId = 0,
                Ciudad = @Resources.Resource.ComboSelect,
            });
            return cities.OrderBy(d => d.Ciudad).ToList();
        }


        //Combos de Zonas
        public static List<Zone> GetZone(int companyid)
        {
            var zones = db.Zones.Where(c => c.CompanyId == companyid).ToList();
            zones.Add(new Zone
            {
                ZoneId = 0,
                Zona = @Resources.Resource.ComboSelect,
            });
            return zones.OrderBy(d => d.Zona).ToList();
        }

        //Combos de SpeedDown
        public static List<SpeedDown> GetSpeedown()
        {
            var speeddowns = db.SpeedDowns.ToList();
            speeddowns.Add(new SpeedDown
            {
                SpeedDownId = 0,
                VelocidadDown = @Resources.Resource.ComboSelect,
            });
            return speeddowns.OrderBy(o=> o.SpeedDownId).ToList();
        }

        //Combos de SpeedUp
        public static List<SpeedUp> GetSpeeUp()
        {
            var speedups = db.SpeedUps.ToList();
            speedups.Add(new SpeedUp
            {
                SpeedUpId = 0,
                VelocidadUp = @Resources.Resource.ComboSelect,
            });
            return speedups.OrderBy(o=> o.SpeedUpId).ToList();
        }

        //Combos de Tiempo Inactivo
        public static List<TicketInactive> GetTicketinactive()
        {
            var ticketinactives = db.TicketInactives.ToList();
            ticketinactives.Add(new TicketInactive
            {
                TicketInactiveId = 0,
                TiempoInactivo = @Resources.Resource.ComboSelect,
            });
            return ticketinactives.OrderBy(o=> o.TicketInactiveId).ToList();
        }

        //Combos de Tiempo Refrescar
        public static List<TicketRefresh> GetTicketrefresh()
        {
            var ticketrefreshs = db.TicketRefreshes.ToList();
            ticketrefreshs.Add(new TicketRefresh
            {
                TicketRefreshId = 0,
                TiempoRefrescar = @Resources.Resource.ComboSelect,
            });
            return ticketrefreshs.OrderBy(o=> o.TicketRefreshId).ToList();
        }

        //Combos de Tiempo del Ticket
        public static List<TicketTime> GetTicketime()
        {
            var tickettimes = db.TicketTimes.ToList();
            tickettimes.Add(new TicketTime
            {
                TicketTimeId = 0,
                TiempoTicket = @Resources.Resource.ComboSelect,
            });
            return tickettimes.OrderBy(o=> o.TicketTimeId).ToList();
        }

        //Combos de Zonas
        public static List<Tax> GetTax(int companyid)
        {
            var tax = db.Taxes.Where(c => c.CompanyId == companyid).ToList();
            tax.Add(new Tax
            {
                TaxId = 0,
                Impuesto = @Resources.Resource.ComboSelect,
            });
            return tax.OrderBy(d => d.Impuesto).ToList();
        }

        //Combos de Zonas
        public static List<PlanCategory> GetPlancategory(int companyid)
        {
            var plancategory = db.PlanCategories.Where(c => c.CompanyId == companyid).ToList();
            plancategory.Add(new PlanCategory
            {
                PlanCategoryId = 0,
                Categoria = @Resources.Resource.ComboSelect,
            });
            return plancategory.OrderBy(d => d.Categoria).ToList();
        }

        //Combos de Servidores
        public static List<Server> GetServer(int companyid)
        {
            var server = db.Servers.Where(c => c.CompanyId == companyid).ToList();
            server.Add(new Server
            {
                ServerId = 0,
                Nombre = @Resources.Resource.ComboSelect,
            });
            return server.OrderBy(d => d.IpServer).ToList();
        }

        //Combos de Zonas
        public static List<PlanTicket> GetPlanTicket(int companyid)
        {
            var plantikes = db.PlanTickets.Where(c => c.CompanyId == companyid).ToList();
            plantikes.Add(new PlanTicket
            {
                PlanTicketId = 0,
                Plan = @Resources.Resource.ComboSelect,
            });
            return plantikes.OrderBy(d => d.Plan).ToList();
        }

        //Combos de Orderticketdeatil
        public static List<OrderTicketDetail> GetOrderticketdetail(int companyid)
        {
            var orderticketdetail = db.OrderTicketDetails.Where(c => c.CompanyId == companyid && c.Vendido == false).ToList();
            orderticketdetail.Add(new OrderTicketDetail
            {
                OrderTicketDetailId = 0,
                Usuario = @Resources.Resource.ComboSelect,
            });
            return orderticketdetail.OrderBy(d => d.Usuario).ToList();
        }


        public static string GetCrearPassword(int longitud, string caracteres)
        {
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < longitud--)
            {
                res.Append(caracteres[rnd.Next(caracteres.Length)]);
            }
            return res.ToString();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}