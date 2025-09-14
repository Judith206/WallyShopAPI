using Microsoft.EntityFrameworkCore;
using WallyShopAPI.Entidades;

namespace WallyShopAPI.Repositorios
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> productos { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Cotizacion> Cotizaciones { get; set; }
        public object Productos { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Email unico 
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            //Relacion uno a muchos entre Usuario y Rol
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId);

            // Relación uno a muchos entre Usuario y Cotizacion
            modelBuilder.Entity<Cotizacion>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Cotizaciones)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación uno a muchos entre Producto y Cotizacion
            modelBuilder.Entity<Cotizacion>()
                .HasOne(c => c.Producto)
                .WithMany(p => p.Cotizaciones)
                .HasForeignKey(c => c.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración adicional para Cotizacion
            modelBuilder.Entity<Cotizacion>()
                .Property(c => c.Fecha)
                .IsRequired();

            modelBuilder.Entity<Cotizacion>()
                .Property(c => c.Contacto)
                .IsRequired();
        }
    }
}
