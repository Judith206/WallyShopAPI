namespace WallyShopAPI.DTOs.UsuarioDTOs
{
    public class UsuarioRespuestaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";

        public string Email { get; set; } = "";

        public string Rol { get; set; } = "";

        public string Token { get; set; } = "";
    }
}
