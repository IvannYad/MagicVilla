using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_Web.Models.DTO
{
    public class VillaNumberUpdateDTO
    {
        [Required]
        public int VillaNo { get; set; }
        [ForeignKey("VillaDTO")]
        public int VillaId { get; set; }
        public string SpecialDetails { get; set; }
    }
}
