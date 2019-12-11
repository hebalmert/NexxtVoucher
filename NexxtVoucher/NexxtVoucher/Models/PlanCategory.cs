using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtVoucher.Models
{
    public class PlanCategory
    {
        [Key]
        public int PlanCategoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("PlanCategory_Company_Categoria_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Compania")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("PlanCategory_Company_Categoria_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "PlanCategory_Model_Categoria")]
        public string Categoria { get; set; }

        [MaxLength(250, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "PlanCateogry_Model_Detail")]
        [DataType(DataType.MultilineText)]
        public string Detalle { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<PlanTicket> PlanTickets { get; set; }

        public virtual ICollection<OrderTicket> OrderTickets { get; set; }

        public virtual ICollection<SellTicketOne> SellTicketOnes { get; set; }

        public virtual ICollection<OrderTicketDetail> OrderTicketDetails { get; set; }

        public virtual ICollection<SellTicket> SellTickets { get; set; }
    }
}