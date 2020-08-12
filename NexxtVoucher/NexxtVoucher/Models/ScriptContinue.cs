using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NexxtVoucher.Models
{
    public class ScriptContinue
    {
        [Key]
        public int ScriptContinueId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        //[MaxLength(10, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        //[Index("SpeedDown_VelocidadDown_Index", IsUnique = true)]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Resource), Name = "Model_ScriptContinue_ScriptTciket")]
        public string ScriptTicket { get; set; }
    }
}