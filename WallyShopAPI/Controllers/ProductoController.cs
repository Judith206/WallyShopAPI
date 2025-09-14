using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetById(int id)
        {
            var producto = await _productoRepository.GetByIdAsync(id);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> Create(Producto producto)
        {
            var created = await _productoRepository.AddAsync(producto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Producto>> Update(int id, Producto producto)
        {
            if (id != producto.Id) return BadRequest();

            var updated = await _productoRepository.UpdateAsync(producto);
            return Ok(updated);
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
