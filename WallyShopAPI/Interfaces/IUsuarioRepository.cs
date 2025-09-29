using WallyShopAPI.DTOs.UsuarioDTOs;
using WallyShopAPI.Entidades;

namespace WallyShopAPI.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByEmailAsync(string email);

        Task<Usuario> AddAsync(Usuario usuario);

        Task<List<UsuarioListadoDTO>> GetAllUsuariosAsync();
    }
}
