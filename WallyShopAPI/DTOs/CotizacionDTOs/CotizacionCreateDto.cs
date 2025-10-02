using System.ComponentModel.DataAnnotations;

namespace WallyShopAPI.DTOs.CotizacionDTOs
{
    public class CotizacionCreateDTO
    {
        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El contacto es requerido")]
        [StringLength(100, ErrorMessage = "El contacto no puede exceder 100 caracteres")]
        public string Contacto { get; set; } = null!; //  Puede ser email o teléfono

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, 1000, ErrorMessage = "La cantidad debe ser entre 1 y 1000")]
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El producto es requerido")]
        public int ProductoId { get; set; }
    }

}
