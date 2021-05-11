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
        [Index("TicketTime_TiempoTicket_Orden_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Model_TicketTime_Tiempo")]
        public string TiempoTicket { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("TicketTime_TiempoTicket_Orden_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Model_TicketTime_Orden")]
        public string Orden { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resource), Name = "Model_ScriptContinue_ScriptTciketConsumo")]
        public string ScriptTicketConsumo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resource), Name = "Model_ScriptContinue_ScriptTciket")]
        public string ScriptTicket { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_IsActive")]
        public bool IsActive { get; set; }

        public virtual ICollection<PlanTicket> PlanTickets { get; set; }
    }
}