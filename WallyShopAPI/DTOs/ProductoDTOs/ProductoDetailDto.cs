namespace WallyShopAPI.DTOs.ProductoDTOs
{
    public class ProductoDetailDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public decimal Precio { get; set; }
        public string Imagen { get; set; } = null!;
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = "";

        // Solo IDs de cotizaciones para evitar recursividad
        public List<int> CotizacionIds { get; set; } = new List<int>();
    }
}
