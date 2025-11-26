using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WallyShopAPI.DTOs.ProductoDTOs;
using WallyShopAPI.Entidades;
using WallyShopAPI.Interfaces;
using WallyShopAPI.Repositorios;

namespace WallyShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : Controller
    {
        private readonly IProductoRepository _productoRepository;

        // Método de ayuda para obtener el ID del usuario autenticado
        private int GetUserId()
        {
            // Verifica si el usuario está autenticado y si tiene la Claim de ID
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Asumiendo que el NameIdentifier contiene el UsuarioId como string
            if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            // Devolver 0 o lanzar una excepción si el usuario no tiene un ID válido
            // (Esto solo debería ocurrir si [Authorize] falla o se omite)
            return 0;
        }

        public ProductoController(IProductoRepository pProductoRepository)
        {
            _productoRepository = pProductoRepository;
        }

        [HttpGet("productos")]
        [Authorize]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _productoRepository.GetAllAsync();

            // Convertir a DTOs para evitar referencias circulares
            var productoDtos = productos.Select(p => new ProductoReadDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Estado = p.Estado,
                Precio = p.Precio,
                Imagen = p.Imagen,
                UsuarioId = p.UsuarioId
            }).ToList();

            return Ok(productoDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetById(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            if (producto == null) return NotFound();

            var productoDto = new ProductoReadDTO
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Estado = producto.Estado,
                Precio = producto.Precio,
                Imagen = producto.Imagen,
                UsuarioId = producto.UsuarioId
            };

            return Ok(productoDto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductoReadDTO>> Create(ProductoCreateDTO createDto)
        {
            try
            {
                Console.WriteLine($"Recibiendo producto: {createDto.Nombre}");
                Console.WriteLine($"UsuarioId: {createDto.UsuarioId}");
                Console.WriteLine($"Imagen tamaño: {createDto.Imagen?.Length ?? 0} bytes");

                var producto = new Producto
                {
                    Nombre = createDto.Nombre,
                    Descripcion = createDto.Descripcion,
                    Estado = createDto.Estado,
                    Precio = createDto.Precio,
                    Imagen = createDto.Imagen,
                    UsuarioId = createDto.UsuarioId
                };

                var created = await _productoRepository.AddAsync(producto);
                Console.WriteLine($"Producto creado con ID: {created.Id}");

                var responseDto = new ProductoReadDTO
                {
                    Id = created.Id,
                    Nombre = created.Nombre,
                    Descripcion = created.Descripcion,
                    Estado = created.Estado,
                    Precio = created.Precio,
                    Imagen = created.Imagen,
                    UsuarioId = created.UsuarioId
                };

                return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear producto: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductoReadDTO>> Update(int id, ProductoUpdateDTO updateDto)
        {
            int usuarioId = GetUserId();
            if (usuarioId == 0) return Unauthorized("Usuario no autenticado.");
            if (id != updateDto.Id) return BadRequest();

            var productoExistente = await _productoRepository.GetByIdAsync(id);
            if (productoExistente == null) return NotFound();

            // 🔑 VERIFICACIÓN CLAVE: Solo el dueño puede editar/eliminar
            if (productoExistente.UsuarioId != usuarioId)
            {
                return Forbid("No tiene permisos para editar este producto."); // HTTP 403
            }

            productoExistente.Nombre = updateDto.Nombre;
            productoExistente.Descripcion = updateDto.Descripcion;
            productoExistente.Estado = updateDto.Estado;
            productoExistente.Precio = updateDto.Precio;
            productoExistente.Imagen = updateDto.Imagen;
            productoExistente.UsuarioId = updateDto.UsuarioId;

            var updated = await _productoRepository.UpdateAsync(productoExistente);

            var responseDto = new ProductoReadDTO
            {
                Id = updated.Id,
                Nombre = updated.Nombre,
                Descripcion = updated.Descripcion,
                Estado = updated.Estado,
                Precio = updated.Precio,
                Imagen = updated.Imagen,
                UsuarioId = updated.UsuarioId
            };

            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productoRepository.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }


        // [HttpGet("productos")] // DEJA ESTE ENDPOINT para el Dashboard
        // [Authorize] // Esto es correcto para protegerlo

        // El nombre "GetProductos" es ambiguo. Podrías renombrarlo a "GetAllForDashboard" si quieres usarlo globalmente.
        // Si lo quieres personal, modifícalo así:

        [HttpGet("mis-productos")] // Nuevo endpoint para claridad, o usa el existente si se llama "MisProductos"
        [Authorize]
        public async Task<IActionResult> GetMisProductos() // Cambiar nombre
        {
            int usuarioId = GetUserId(); // Obtener el ID de la sesión

            if (usuarioId == 0) return Unauthorized("Usuario no autenticado.");

            // USAR EL NUEVO MÉTODO DEL REPOSITORIO
            var productos = await _productoRepository.GetProductosByUsuarioIdAsync(usuarioId);

            var productoDtos = productos.Select(p => new ProductoReadDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Estado = p.Estado,
                Precio = p.Precio,
                Imagen = p.Imagen,
                UsuarioId = p.UsuarioId
            }).ToList();
            return Ok(productoDtos);
        }

        // **OPCIONAL:** Mantén el [HttpGet("productos")] original si lo usas para el Dashboard
        // Asegúrate de que tu frontend de "Mis Productos" llama a "mis-productos" (o al nombre que elijas).
    }
}
