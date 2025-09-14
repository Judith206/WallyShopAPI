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
        public async Task<ActionResult<IEnumerable<CotizacionDto>>> GetCotizaciones()
        {
            var cotizaciones = await _cotizacionRepository.GetAllCotizacionesAsync();
            return Ok(cotizaciones);
        }

        // GET: api/cotizaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CotizacionDto>> GetCotizacion(int id)
        {
            var cotizacion = await _cotizacionRepository.GetByIdAsync(id);

            if (cotizacion == null)
            {
                return NotFound();
            }

            var cotizacionDto = new CotizacionDto
            {
                Id = cotizacion.Id,
                Fecha = cotizacion.Fecha,
                Contacto = cotizacion.Contacto,
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
        public async Task<ActionResult<CotizacionDto>> PostCotizacion(CotizacionCreateDto createDto)
        {
            var cotizacion = new Cotizacion
            {
                Fecha = createDto.Fecha,
                Contacto = createDto.Contacto,
                UsuarioId = createDto.UsuarioId,
                ProductoId = createDto.ProductoId
            };

            var nuevaCotizacion = await _cotizacionRepository.AddAsync(cotizacion);

            // Recargar la entidad con las relaciones
            var cotizacionCompleta = await _cotizacionRepository.GetByIdAsync(nuevaCotizacion.Id);

            var responseDto = new CotizacionDto
            {
                Id = cotizacionCompleta.Id,
                Fecha = cotizacionCompleta.Fecha,
                Contacto = cotizacionCompleta.Contacto,
                UsuarioId = cotizacionCompleta.UsuarioId,
                UsuarioNombre = cotizacionCompleta.Usuario.Nombre,
                ProductoId = cotizacionCompleta.ProductoId,
                ProductoNombre = cotizacionCompleta.Producto.Nombre,
                ProductoPrecio = cotizacionCompleta.Producto.Precio
            };

            return CreatedAtAction(nameof(GetCotizacion), new { id = responseDto.Id }, responseDto);
        }

        // PUT: api/cotizaciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCotizacion(int id, CotizacionCreateDto updateDto)
        {
            var cotizacionExistente = await _cotizacionRepository.GetByIdAsync(id);
            if (cotizacionExistente == null)
            {
                return NotFound();
            }

            cotizacionExistente.Fecha = updateDto.Fecha;
            cotizacionExistente.Contacto = updateDto.Contacto;
            cotizacionExistente.UsuarioId = updateDto.UsuarioId;
            cotizacionExistente.ProductoId = updateDto.ProductoId;

            var actualizado = await _cotizacionRepository.UpdateAsync(cotizacionExistente);

            if (!actualizado)
            {
                return BadRequest("No se pudo actualizar la cotización");
            }

            return NoContent();
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