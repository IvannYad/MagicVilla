using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Models.DTO
{
    public class VillaNumberCreateDTO
    {
        [Required]
        [Range(100, 1000)]
        public int VillaNo { get; set; }
        [Required]
        [DisplayName("Villa")]
        public int VillaId { get; set; }
        public string SpecialDetails { get; set; }
    }
}
