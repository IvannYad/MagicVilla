using MagicVilla.Models;
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
        public IEnumerable<Villa> GetVillas()
        {
            return new List<Villa>
            {
                new Villa { Id = 1, Name = "Pool Villa" },
                new Villa { Id = 2, Name = "Beach Villa" },
            };
        }
    }
}
