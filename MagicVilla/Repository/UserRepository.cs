using MagicVilla.Data;
using MagicVilla.Models;
using MagicVilla.Models.DTO;
using MagicVilla.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private string _secretKey;

        public UserRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret")!;
        }
        public bool IsUserUnique(string username)
        {
            var userFromDb = _context.LocalUsers.FirstOrDefault(user => user.Name == username);
            if (userFromDb is null)
                return true;

            return false;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            var user = _context.LocalUsers
                .FirstOrDefault(user => user.UserName == loginRequestDTO.UserName && user.Password == loginRequestDTO.Password);
            if (user is null)
                return new LoginResponseDTO
                {
                    Token = string.Empty,
                    User = null,
                };

            // If user is found generate JWT token.
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = user,
            };
            return loginResponseDTO;
        }

        public async Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            LocalUser user = new LocalUser()
            {
                UserName = registrationRequestDTO.UserName,
                Password = registrationRequestDTO.Password,
                Name = registrationRequestDTO.Name,
                Role = registrationRequestDTO.Role
            };

            await _context.LocalUsers.AddAsync(user);
            await _context.SaveChangesAsync();
            user.Password = string.Empty;
            return user;
        }
    }
}
