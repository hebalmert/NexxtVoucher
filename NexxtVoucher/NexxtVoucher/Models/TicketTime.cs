using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtVoucher.Models
{
    public class TicketTime
    {
        [Key]
        public int TicketTimeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("TicketTime_TiempoTicket_Index", IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Model_TicketTime_Tiempo")]
        public string TiempoTicket { get; set; }

        public virtual ICollection<PlanTicket> PlanTickets { get; set; }
    }
}