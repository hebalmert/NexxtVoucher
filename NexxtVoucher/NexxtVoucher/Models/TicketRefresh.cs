using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtVoucher.Models
{
    public class TicketRefresh
    {
        [Key]
        public int TicketRefreshId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("TicketRefresh_TiempoRefrescar_Index", IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Model_TicketRefresh")]
        public string TiempoRefrescar { get; set; }

        public virtual ICollection<PlanTicket> PlanTickets { get; set; }
    }
}