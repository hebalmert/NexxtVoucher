﻿using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtVoucher.Models
{
    public class SpeedDown
    {
        [Key]
        public int SpeedDownId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("SpeedDown_VelocidadDown_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Model_SpeedDown_Velocidad")]
        public string VelocidadDown { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("SpeedDown_VelocidadDown_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Model_TicketTime_Orden")]
        public string Orden { get; set; }

        public virtual ICollection<PlanTicket> PlanTickets { get; set; }
    }
}