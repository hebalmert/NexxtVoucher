using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace NexxtVoucher.Models
{
    public class NexxtVouContext : DbContext
    {
        public NexxtVouContext() : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.SpeedDown> SpeedDowns { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.SpeedUp> SpeedUps { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.TicketTime> TicketTimes { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.Company> Companies { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.Register> Registers { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.Tax> Taxes { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.HeadText> HeadTexts { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.Identification> Identifications { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.City> Cities { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.Zone> Zones { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.PlanCategory> PlanCategories { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.TicketInactive> TicketInactives { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.TicketRefresh> TicketRefreshes { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.Server> Servers { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.PlanTicket> PlanTickets { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.OrderTicket> OrderTickets { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.ChainCode> ChainCodes { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.OrderTicketDetail> OrderTicketDetails { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.SellTicketOne> SellTicketOnes { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.SellTicket> SellTickets { get; set; }

        public System.Data.Entity.DbSet<NexxtVoucher.Models.SellTicketDetail> SellTicketDetails { get; set; }

    }
}