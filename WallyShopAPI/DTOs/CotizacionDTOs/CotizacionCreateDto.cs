namespace WallyShopAPI.DTOs.CotizacionDTOs
{
    public class CotizacionCreateDto
    {
        public DateTime Fecha { get; set; }
        public int Contacto { get; set; }
        public int UsuarioId { get; set; }
        public int ProductoId { get; set; }
    }

}
