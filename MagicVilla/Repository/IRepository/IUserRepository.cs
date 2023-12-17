using MagicVilla.Models;
using MagicVilla.Models.DTO;

namespace MagicVilla.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUserUnique(string username);
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO);
    }
}
