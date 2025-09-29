using Microsoft.AspNetCore.Mvc;
using WallyShopAPI.DTOs.UsuarioDTOs;
using WallyShopAPI.Interfaces;

namespace WallyShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] UsuarioRegistroDTO dto)
        {
            var result = await _authService.RegistrarAsync(dto);
            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDTO dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null) return Unauthorized("Credenciales invalidas");
            return Ok(result);
        }
    }
}
