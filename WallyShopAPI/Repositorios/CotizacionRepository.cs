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
    }
}