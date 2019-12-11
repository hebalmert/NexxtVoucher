using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtVoucher.Models
{
    public class PlanTicket
    {
        [Key]
        public int PlanTicketId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("PlanTicket_Company_Plan_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Compania")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "PlanCategory_Model_Categoria")]
        public int PlanCategoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_Server")]
        public int ServerId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("PlanTicket_Company_Plan_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_Plan")]
        public string Plan { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_SpeedUp")]
        public int SpeedUpId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_SpeedDown")]
        public int SpeedDownId { get; set; }

        //[Display(ResourceType = typeof(Resource), Name = "PlanTicket_Modelo_upDown")]
        //public virtual string Velocidad { get { return string.Format("{0}/{1}", SpeedUp.VelocidadUp, SpeedDown.VelocidadDown); } }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_TicketTime")]
        public int TicketTimeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_TicketInactive")]
        public int TicketInactiveId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_TicketRefresh")]
        public int TicketRefreshId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, 10, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_ShareUser")]
        public int ShareUser { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_Impuesto")]
        public int TaxId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]  //Currency es formato de Moneda del pais IP
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] //Formato valor con 2 decimales
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_Precio")]
        public decimal Precio { get; set; }

        [MaxLength(250, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_Detalle")]
        public string Detalle { get; set; }

        [MaxLength(15, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Modelo_Mikrotik")]
        public string MikrotikId { get; set; }

        public virtual Company Company { get; set; }

        public virtual PlanCategory PlanCategory { get; set; }

        public virtual SpeedUp SpeedUp { get; set; }

        public virtual SpeedDown SpeedDown { get; set; }

        public virtual TicketTime TicketTime { get; set; }

        public virtual TicketInactive TicketInactive { get; set; }

        public virtual TicketRefresh TicketRefresh { get; set; }

        public virtual Tax Tax { get; set; }

        public virtual Server Server { get; set; }

        public virtual ICollection<OrderTicket> OrderTickets { get; set; }

        public virtual ICollection<SellTicketOne> SellTicketOnes { get; set; }

        public virtual ICollection<SellTicket> SellTickets { get; set; }

        public virtual ICollection<OrderTicketDetail> OrderTicketDetails { get; set; }
    }
}