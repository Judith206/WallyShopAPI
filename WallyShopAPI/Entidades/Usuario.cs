namespace WallyShopAPI.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = "";

        public string Email { get; set; } = "";

        public string PasswordHash { get; set; } = "";

        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;

        //  Relación con Cotizaciones
        public ICollection<Cotizacion> Cotizaciones { get; set; } = new List<Cotizacion>();
    }
}
