using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtVoucher.Models
{
    public class SellTicket
    {
        [Key]
        public int SellTicketId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("SellTicket_Company_VentaOne_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Compania")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resource), Name = "OrderTicket_Model_Date")]
        public DateTime Date { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "OrderTicket_Model_OrdenNumero")]
        [Index("SellTicket_Company_VentaOne_Index", 2, IsUnique = true)]
        public int VentaOne { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "OrderTicket_Model_ServerId")]
        public int ServerId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "PlanCategory_Model_Categoria")]
        public int PlanCategoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_Plan")]
        public int PlanTicketId { get; set; }

        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] //Formato Porcentaje con 2 decimales
        [Display(ResourceType = typeof(Resource), Name = "OderTiket_Model_Precio")]
        public decimal Precio { get; set; }

        public int TotalTicket => SellTicketDetails == null ? 0 : SellTicketDetails.Count;

        public decimal TotalVenta => SellTicketDetails == null ? 0 : SellTicketDetails.Sum(s => s.Precio);

        public virtual Company Company { get; set; }

        public virtual Server Server { get; set; }

        public virtual PlanCategory PlanCategory { get; set; }

        public virtual PlanTicket PlanTicket { get; set; }

        public virtual ICollection<SellTicketDetail> SellTicketDetails { get; set; }
    }
}