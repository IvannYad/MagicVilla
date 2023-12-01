using MagicVilla.Data;
using MagicVilla.Models;
using MagicVilla.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla.Controllers
{
    // We need to apply [Route] attribute to API class.
    [Route("api/[controller]")]
    // Classes with [ApiController] are configured with features to improve developers experience for buildeing API.
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<VillaDTO> GetVillas()
        {
            return VillaStore.villaList;
        }

        [HttpGet("{id:int}")]
        public VillaDTO? GetVilla(int id)
        {
            return VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        }
    }
}
