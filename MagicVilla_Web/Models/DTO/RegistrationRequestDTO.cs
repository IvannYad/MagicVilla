using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MagicVilla_Web.Models.DTO
{
    public class RegistrationRequestDTO
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string Role { get; set; } = "regular";
    }
}
