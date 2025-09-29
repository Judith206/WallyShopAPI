namespace WallyShopAPI.DTOs.ProductoDTOs
{
    public class ProductoUpdateDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public decimal Precio { get; set; }
        public byte[] Imagen { get; set; } = null!;
        public int UsuarioId { get; set; }
    }
}
