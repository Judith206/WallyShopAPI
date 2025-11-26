using Microsoft.EntityFrameworkCore;
using WallyShopAPI.DTOs.CotizacionDTOs;
using WallyShopAPI.Entidades;
using WallyShopAPI.Interfaces;
using WallyShopAPI.Repositorios;

namespace WallyShopAPI.Repositories
{
    public class CotizacionRepository : ICotizacionRepository
    {
        private readonly AppDbContext _context;

        public CotizacionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cotizacion?> GetByIdAsync(int id)
        {
            return await _context.Cotizaciones
                .Include(c => c.Usuario)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cotizacion> AddAsync(Cotizacion cotizacion)
        {
            _context.Cotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();
            return cotizacion;
        }

        public async Task<List<CotizacionDTO>> GetAllCotizacionesAsync()
        {
            return await _context.Cotizaciones
                .Include(c => c.Usuario)
                .Include(c => c.Producto)
                .Select(c => new CotizacionDTO
                {
                    Id = c.Id,
                    Fecha = c.Fecha,
                    Contacto = c.Contacto,
                    Cantidad = c.Cantidad,
                    Total = c.Total,
                    UsuarioId = c.UsuarioId,
                    UsuarioNombre = c.Usuario.Nombre,
                    ProductoId = c.ProductoId,
                    ProductoNombre = c.Producto.Nombre,
                    ProductoPrecio = c.Producto.Precio
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Cotizacion cotizacion)
        {
            _context.Cotizaciones.Update(cotizacion);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cotizacion = await GetByIdAsync(id);
            if (cotizacion == null) return false;

            _context.Cotizaciones.Remove(cotizacion);
            return await _context.SaveChangesAsync() > 0;
        }

        // NUEVOS MÉTODOS IMPLEMENTADOS
        public async Task<List<CotizacionDTO>> GetByFechaRangeAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Cotizaciones
                .Include(c => c.Usuario)
                .Include(c => c.Producto)
                .Where(c => c.Fecha.Date >= fechaInicio.Date && c.Fecha.Date <= fechaFin.Date)
                .Select(c => new CotizacionDTO
                {
                    Id = c.Id,
                    Fecha = c.Fecha,
                    Contacto = c.Contacto,
                    Cantidad = c.Cantidad,
                    Total = c.Total,
                    UsuarioId = c.UsuarioId,
                    UsuarioNombre = c.Usuario.Nombre,
                    ProductoId = c.ProductoId,
                    ProductoNombre = c.Producto.Nombre,
                    ProductoPrecio = c.Producto.Precio
                })
                .ToListAsync();
        }

        public async Task<List<CotizacionDTO>> GetByContactoAsync(string contacto)
        {
            return await _context.Cotizaciones
                .Include(c => c.Usuario)
                .Include(c => c.Producto)
                .Where(c => c.Contacto.ToLower().Contains(contacto.ToLower()))
                .Select(c => new CotizacionDTO
                {
                    Id = c.Id,
                    Fecha = c.Fecha,
                    Contacto = c.Contacto,
                    Cantidad = c.Cantidad,
                    Total = c.Total,
                    UsuarioId = c.UsuarioId,
                    UsuarioNombre = c.Usuario.Nombre,
                    ProductoId = c.ProductoId,
                    ProductoNombre = c.Producto.Nombre,
                    ProductoPrecio = c.Producto.Precio
                })
                .ToListAsync();
        }

        public async Task<List<CotizacionDTO>> GetCotizacionesPersonalesAsync(int usuarioId)
        {
            // Carga las cotizaciones donde el usuario es el creador (Comprador)
            // O donde el producto cotizado pertenece al usuario (Vendedor)
            return await _context.Cotizaciones
                .Include(c => c.Usuario)
                .Include(c => c.Producto)
                    .ThenInclude(p => p.Usuario) // Incluir el dueño del producto
                .Where(c => c.UsuarioId == usuarioId || c.Producto.UsuarioId == usuarioId) // <-- DOBLE FILTRO CLAVE
                .Select(c => new CotizacionDTO
                {
                    Id = c.Id,
                    Fecha = c.Fecha,
                    Contacto = c.Contacto,
                    Cantidad = c.Cantidad,
                    Total = c.Total,
                    UsuarioId = c.UsuarioId,
                    UsuarioNombre = c.Usuario.Nombre,
                    ProductoId = c.ProductoId,
                    ProductoNombre = c.Producto.Nombre,
                    ProductoPrecio = c.Producto.Precio
                })
                .ToListAsync();
        }

    }
}