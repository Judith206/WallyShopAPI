using Microsoft.AspNetCore.Mvc;
using WallyShopAPI.DTOs.CotizacionDTOs;
using WallyShopAPI.Entidades;
using WallyShopAPI.Interfaces;

namespace WallyShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotizacionesController : ControllerBase
    {
        private readonly ICotizacionRepository _cotizacionRepository;

        public CotizacionesController(ICotizacionRepository cotizacionRepository)
        {
            _cotizacionRepository = cotizacionRepository;
        }

        // GET: api/cotizaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CotizacionDTO>>> GetCotizaciones()
        {
            var cotizaciones = await _cotizacionRepository.GetAllCotizacionesAsync();
            return Ok(cotizaciones);
        }

        // GET: api/cotizaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CotizacionDTO>> GetCotizacion(int id)
        {
            var cotizacion = await _cotizacionRepository.GetByIdAsync(id);

            if (cotizacion == null)
            {
                return NotFound();
            }

            var cotizacionDto = new CotizacionDTO
            {
                Id = cotizacion.Id,
                Fecha = cotizacion.Fecha,
                Contacto = cotizacion.Contacto,
                Cantidad = cotizacion.Cantidad, 
                Total = cotizacion.Total,
                UsuarioId = cotizacion.UsuarioId,
                UsuarioNombre = cotizacion.Usuario.Nombre,
                ProductoId = cotizacion.ProductoId,
                ProductoNombre = cotizacion.Producto.Nombre,
                ProductoPrecio = cotizacion.Producto.Precio
            };

            return Ok(cotizacionDto);
        }

        // POST: api/cotizaciones
        [HttpPost]
        public async Task<ActionResult<CotizacionDTO>> PostCotizacion(CotizacionCreateDTO createDto)
        {
            try
            {
                Console.WriteLine($"📥 Recibiendo cotización:");
                Console.WriteLine($"   - Contacto: {createDto.Contacto}");
                Console.WriteLine($"   - Fecha: {createDto.Fecha}");
                Console.WriteLine($"   - ProductoId: {createDto.ProductoId}");
                Console.WriteLine($"   - Cantidad: {createDto.Cantidad}"); 
                Console.WriteLine($"   - Total: {createDto.Total}");
                Console.WriteLine($"   - UsuarioId: {createDto.UsuarioId}");

                var cotizacion = new Cotizacion
                {
                    Fecha = createDto.Fecha,
                    Contacto = createDto.Contacto,
                    Cantidad = createDto.Cantidad, 
                    Total = createDto.Total,
                    UsuarioId = createDto.UsuarioId,
                    ProductoId = createDto.ProductoId
                };

                var nuevaCotizacion = await _cotizacionRepository.AddAsync(cotizacion);
                Console.WriteLine($" Cotización creada con ID: {nuevaCotizacion.Id}");

                // Recargar la entidad con las relaciones
                var cotizacionCompleta = await _cotizacionRepository.GetByIdAsync(nuevaCotizacion.Id);

                var responseDto = new CotizacionDTO
                {
                    Id = cotizacionCompleta.Id,
                    Fecha = cotizacionCompleta.Fecha,
                    Contacto = cotizacionCompleta.Contacto,
                    Cantidad = cotizacionCompleta.Cantidad, // ✅ AGREGADO
                    Total = cotizacionCompleta.Total,
                    UsuarioId = cotizacionCompleta.UsuarioId,
                    UsuarioNombre = cotizacionCompleta.Usuario.Nombre,
                    ProductoId = cotizacionCompleta.ProductoId,
                    ProductoNombre = cotizacionCompleta.Producto.Nombre,
                    ProductoPrecio = cotizacionCompleta.Producto.Precio
                };

                return CreatedAtAction(nameof(GetCotizacion), new { id = responseDto.Id }, responseDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear cotización: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/cotizaciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCotizacion(int id, CotizacionCreateDTO updateDto)
        {
            try
            {
                Console.WriteLine($"🔄 Actualizando cotización ID: {id}");

                var cotizacionExistente = await _cotizacionRepository.GetByIdAsync(id);
                if (cotizacionExistente == null)
                {
                    return NotFound();
                }

                cotizacionExistente.Fecha = updateDto.Fecha;
                cotizacionExistente.Contacto = updateDto.Contacto;
                cotizacionExistente.Cantidad = updateDto.Cantidad; 
                cotizacionExistente.Total = updateDto.Total;
                cotizacionExistente.UsuarioId = updateDto.UsuarioId;
                cotizacionExistente.ProductoId = updateDto.ProductoId;

                var actualizado = await _cotizacionRepository.UpdateAsync(cotizacionExistente);

                if (!actualizado)
                {
                    return BadRequest("No se pudo actualizar la cotización");
                }

                Console.WriteLine($"✅ Cotización actualizada exitosamente");
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al actualizar cotización: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // DELETE: api/cotizaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCotizacion(int id)
        {
            var eliminado = await _cotizacionRepository.DeleteAsync(id);

            if (!eliminado)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}