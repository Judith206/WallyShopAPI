using WallyShopAPI.DTOs.UsuarioDTOs;

namespace WallyShopAPI.Interfaces
{
    public interface IAuthService
    {
        Task<UsuarioRespuestaDto> RegistrarAsync(UsuarioRegistroDTO dto);

        Task<UsuarioRespuestaDto> LoginAsync(UsuarioLoginDto dto);
    }
}
