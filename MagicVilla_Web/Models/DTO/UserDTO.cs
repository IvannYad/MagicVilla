﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Models
{
    public class UserDTO
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
