using System.ComponentModel.DataAnnotations.Schema;

namespace WallyShopAPI.Entidades
{
    [Table("cotizacion")]
    public class Cotizacion
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Contacto { get; set; } = null!; //  Puede ser email o teléfono

        public int Cantidad { get; set; }

        public decimal Total { get; set; }

        //  Clave foránea a Producto
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        //  Clave foránea a Usuario
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
