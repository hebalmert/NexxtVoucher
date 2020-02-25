using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtVoucher.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("Company_Name_Index", IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Compania")]
        public string Compania { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("Company_Rif_Index", IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Rif")]
        public string Rif { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [DataType(DataType.PhoneNumber)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Phone")]
        public string Phone { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(250, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Address")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Logo")]
        public string Logo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Country")]
        public string Country { get; set; }

        [NotMapped]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Logo")]
        public HttpPostedFileBase LogoFile { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Desde")]
        public DateTime DateDesde { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Hasta")]
        public DateTime DateHasta { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Active")]
        public bool Activo { get; set; }

        public virtual ICollection<Register> Registers { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<Tax> Taxes { get; set; }

        public virtual ICollection<HeadText> HeadTexts { get; set; }

        public virtual ICollection<Identification> Identifications { get; set; }

        public virtual ICollection<City> Cities { get; set; }

        public virtual ICollection<PlanCategory> PlanCategories { get; set; }

        public virtual ICollection<Server> Servers { get; set; }

        public virtual ICollection<PlanTicket> PlanTickets { get; set; }

        public virtual ICollection<OrderTicket> OrderTickets { get; set; }

        public virtual ICollection<ChainCode> ChainCodes { get; set; }

        public virtual ICollection<OrderTicketDetail> OrderTicketDetails { get; set; }

        public virtual ICollection<SellTicketOne> SellTicketOnes { get; set; }

        public virtual ICollection<SellTicket> SellTickets { get; set; }

        public virtual ICollection<SellTicketDetail> SellTicketDetails { get; set; }

        public virtual ICollection<Cachier> Cachiers { get; set; }

        public virtual ICollection<SellTicketOneCachier> SellTicketOneCachiers { get; set; }
    }
}