using System.ComponentModel.DataAnnotations.Schema;

namespace WallyShopAPI.Entidades
{
    [Table("cotizacion")]
    public class Cotizacion
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int Contacto { get; set; }

        //  Clave foránea a Producto
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        //  Clave foránea a Usuario
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
