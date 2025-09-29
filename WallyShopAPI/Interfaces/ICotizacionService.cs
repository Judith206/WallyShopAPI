using WallyShopAPI.DTOs.CotizacionDTOs;
using WallyShopAPI.Entidades;

namespace WallyShopAPI.Interfaces
{
    public interface ICotizacionRepository
    {
        // Obtener una cotización por Id
        Task<Cotizacion?> GetByIdAsync(int id);

        // Agregar nueva cotización
        Task<Cotizacion> AddAsync(Cotizacion cotizacion);

        // Listar todas las cotizaciones (ahora usando CotizacionDto)
        Task<List<CotizacionDTO>> GetAllCotizacionesAsync();

        // Actualizar cotización
        Task<bool> UpdateAsync(Cotizacion cotizacion);

        // Eliminar cotización
        Task<bool> DeleteAsync(int id);
    }
}
