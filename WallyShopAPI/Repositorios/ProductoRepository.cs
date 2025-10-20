using Microsoft.EntityFrameworkCore;
using WallyShopAPI.Entidades;
using WallyShopAPI.Interfaces;

namespace WallyShopAPI.Repositorios
{
    public class ProductoRepository : IProductoRepository
    {
        public readonly AppDbContext _context;

        public ProductoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _context.productos
                .Include(p => p.Usuario) // Incluir el usuario relacionado
                .ToListAsync();
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.productos
                .Include(p => p.Usuario) // Incluir el usuario relacionado
                .ThenInclude(u => u.Rol)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Producto> AddAsync(Producto producto)
        {
            _context.productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<Producto> UpdateAsync(Producto producto)
        {
            _context.productos.Update(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _context.productos.FindAsync(id);
            if (producto == null) return false;

            _context.productos.Remove(producto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
