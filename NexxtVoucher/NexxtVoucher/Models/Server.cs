using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtVoucher.Models
{
    public class Server
    {
        [Key]
        public int ServerId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("Server_IpServer_Company_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Compania")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Server_Model_Modelo")]
        public string Modelo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Server_Model_Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("Server_IpServer_Company_Index", 2, IsUnique = true)]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$")]
        [Display(ResourceType = typeof(Resource), Name = "Server_Model_IpServidor")]
        public string IpServer { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Server_Model_Usuario")]
        public string Usuario { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(25, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        //[DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resource), Name = "Server_Model_Clave")]
        public string Clave { get; set; }

        [MaxLength(250, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resource), Name = "Server_Model_Detalle")]
        public string Description { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "City_Model_City")]
        public int CityId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "Zone_Model_Zone")]
        public int ZoneId { get; set; }

        public virtual Company Company { get; set; }

        public virtual City City { get; set; }

        public virtual Zone Zone { get; set; }

        public virtual ICollection<PlanTicket> PlanTickets { get; set; }

        public virtual ICollection<OrderTicket> OrderTickets { get; set; }

        public virtual ICollection<SellTicketOne> SellTicketOnes { get; set; }

        public virtual ICollection<OrderTicketDetail> OrderTicketDetails { get; set; }

        public virtual ICollection<SellTicket> SellTickets { get; set; }

        //public virtual ICollection<PlanCategory> PlanCategories { get; set; }

        public virtual ICollection<SellTicketOneCachier> SellTicketOneCachiers { get; set; }

        public virtual ICollection<Cachier> Cachiers { get; set; }

        public virtual ICollection<MikrotikControl> MikrotikControls { get; set; }
    }
}