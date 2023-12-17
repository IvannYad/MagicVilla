using AutoMapper;
using MagicVilla.Data;
using MagicVilla.Models;
using MagicVilla.Models.DTO;
using MagicVilla.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private string _secretKey;

        public UserRepository(ApplicationDbContext context
            , IConfiguration configuration
            , UserManager<ApplicationUser> userManager
            , IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret")!;
        }
        public bool IsUserUnique(string username)
        {
            var userFromDb = _context.ApplicationUsers.FirstOrDefault(user => user.Name == username);
            if (userFromDb is null)
                return true;

            return false;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            var user = _context.ApplicationUsers
                .FirstOrDefault(user => user.UserName == loginRequestDTO.UserName);

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            if (user is null || !isValid)
            {
                return new LoginResponseDTO
                {
                    Token = string.Empty,
                    User = null
                };
            }

            // If user is found generate JWT token.
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDTO>(user),
                Role = roles.FirstOrDefault()
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
