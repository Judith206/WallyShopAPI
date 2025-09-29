using System.ComponentModel.DataAnnotations.Schema;

namespace WallyShopAPI.Entidades
{
    [Table("productos")]
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public bool Estado { get; set; }
        public decimal Precio { get; set; }
        public byte[] Imagen { get; set; } 
        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; } = null!;

        //  Relación con Cotizaciones
        public ICollection<Cotizacion> Cotizaciones { get; set; } = new List<Cotizacion>();
    }
}
