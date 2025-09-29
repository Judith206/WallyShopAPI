using WallyShopAPI.DTOs.UsuarioDTOs;

namespace WallyShopAPI.Interfaces
{
    public interface IAuthService
    {
        Task<UsuarioRespuestaDTO> RegistrarAsync(UsuarioRegistroDTO dto);

        Task<UsuarioRespuestaDTO> LoginAsync(UsuarioLoginDTO dto);
    }
}
