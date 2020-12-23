using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtVoucher.Models
{
    public class SpeedUp
    {
        [Key]
        public int SpeedUpId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("SpeedUp_VelocidadUp_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Model_SpeedUp_Velocidad1")]
        public string VelocidadUp { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Index("SpeedUp_VelocidadUp_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Model_TicketTime_Orden")]
        public string Orden { get; set; }

        public virtual ICollection<PlanTicket> PlanTickets { get; set; }
    }
}