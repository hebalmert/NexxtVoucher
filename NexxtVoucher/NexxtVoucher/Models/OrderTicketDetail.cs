using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NexxtVoucher.Models
{
    public class OrderTicketDetail
    {
        [Key]
        public int OrderTicketDetailId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Index("OrderTicketDetail_TicketNumero_Company_Index", 1, IsUnique = true)]
        [Display(ResourceType = typeof(Resource), Name = "Company_Model_Compania")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]
        [Display(ResourceType = typeof(Resource), Name = "OrderTicketDetail_Model_OrdenTicket")]
        public int OrderTicketId { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "OrderTicketDetail_Model_TicketNumero")]
        [Index("OrderTicketDetail_TicketNumero_Company_Index", 2, IsUnique = true)]
        public int TicketNumero { get; set; }

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

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Msg_Range")]  //Currency es formato de Moneda del pais IP
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)] //Formato valor con 2 decimales
        [Display(ResourceType = typeof(Resource), Name = "PlanTicket_Model_Precio")]
        public decimal Precio { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "OrderTicketDetail_Model_Velocidad")]
        public string Velocidad { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "OrderTicketDetail_Model_Usuario")]
        public string Usuario { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "OrderTicketDetail_Model_Clave")]
        public string Clave { get; set; }

        [MaxLength(15, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "OrderTicketDetail_Model_MikrotikID")]
        public string MikrotikId { get; set; }

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Se Marca si el ticket esta vendido o no y se coloca la fecha en caso de Vendido = true
        [Display(ResourceType = typeof(Resource), Name = "OrderTicketDetail_Model_Vendido")]
        public bool Vendido { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resource), Name = "OrderTicket_Model_Date")]
        public DateTime? Date { get; set; }
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Este numero es de la venta hecha por un Usuario Administrador

        [MaxLength(15, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(Resource), Name = "Register_Model_VentaOne")]
        public string VentaNumero { get; set; }
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Datos en caso que se haya vendido por un Cajero
        [Display(ResourceType = typeof(Resource), Name = "OrderTicketDetail_Model_VendidoCajero")]
        public bool VendidoCajero { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "Cachier_Model_FullName")]
        public int? CachierId { get; set; }
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        
        public virtual Company Company { get; set; }

        public virtual OrderTicket OrderTicket { get; set; }

        public  virtual Server Server { get; set; }

        public virtual PlanCategory PlanCategory { get; set; }

        public virtual PlanTicket PlanTicket { get; set; }

        public virtual Cachier Cachier { get; set; }

        public virtual ICollection<SellTicketOne> SellTicketOnes { get; set; }

        public virtual ICollection<SellTicketDetail> SellTicketDetails { get; set; }

        public virtual ICollection<SellTicketOneCachier> SellTicketOneCachiers { get; set; }
    }
}