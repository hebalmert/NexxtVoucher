﻿using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtVoucher.Models
{
    public class TicketInactive
    {
        [Key]
        public int TicketInactiveId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("TicketInactive_TiempoInactivo_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Model_TicketInactive")]
        public string TiempoInactivo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("TicketInactive_TiempoInactivo_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Model_TicketTime_Orden")]
        public string Orden { get; set; }

        public virtual ICollection<PlanTicket> PlanTickets { get; set; }
    }
}