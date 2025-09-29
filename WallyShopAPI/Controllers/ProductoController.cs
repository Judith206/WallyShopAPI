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

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductoReadDTO>> Update(int id, ProductoUpdateDTO updateDto)
        {
            if (id != updateDto.Id) return BadRequest();

            var productoExistente = await _productoRepository.GetByIdAsync(id);
            if (productoExistente == null) return NotFound();

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
    }
}
