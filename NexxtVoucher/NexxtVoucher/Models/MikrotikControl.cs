namespace NexxtVoucher.Models
{
    using Resources;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class MikrotikControl
    {
        [Key]
        public int MikrotikControlId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("MikrotikControl_Company_Server_PuertoAPI_Index", 1, IsUnique = true)]
        [Index("MikrotikControl_Company_Server_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Compania")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("MikrotikControl_Company_Server_PuertoAPI_Index", 2, IsUnique = true)]
        [Index("MikrotikControl_Company_Server_Index", 2, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_Server")]
        public int ServerId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("MikrotikControl_Company_Server_PuertoAPI_Index", 3, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "MikrotikControl_Model_PuertoApi")]
        public int PuertoApi { get; set; }

        public virtual Company Company { get; set; }

        public virtual Server Server { get; set; }
    }
}