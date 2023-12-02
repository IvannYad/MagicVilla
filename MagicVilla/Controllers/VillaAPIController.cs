using MagicVilla.Data;
using MagicVilla.Models;
using MagicVilla.Models.DTO;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla.Controllers
{
    // We need to apply [Route] attribute to API class.
    [Route("api/[controller]")]
    // Classes with [ApiController] are configured with features to improve developers experience for buildeing API.
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VillaAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            var villas = _context.Villas.AsNoTracking();
            return Ok(villas);
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        // Following attridutes specifies what status codes method can return.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
                return BadRequest();

            var villa = _context.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null)
                return NotFound();

            return Ok(villa);
        }

        [HttpPost(Name = "CreateVilla")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            // With [ApiController] we dont need to explicitly check ModelState.
            // With [ApiController] validation occurs before entering method.
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            if (_context.Villas
                .AsNoTracking()
                .AsEnumerable()
                .FirstOrDefault(v => v.Name.Equals(villaDTO.Name, StringComparison.OrdinalIgnoreCase)) is not null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists");
                return BadRequest(ModelState);
            }

            if (villaDTO is null)
                return BadRequest();

            if (villaDTO.Id > 0)
                return StatusCode(StatusCodes.Status500InternalServerError);
            
            _context.Villas.Add(villaDTO);
            _context.SaveChanges();
            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
                return BadRequest();

            var villa = _context.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null)
                return NotFound();

            _context.Villas.Remove(villa);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if (id == villaDTO.Id || villaDTO is null)
                return BadRequest();

            var villaFromDb = _context.Villas.FirstOrDefault(v => v.Id == id);
            if (villaFromDb == null)
                return NotFound();

            villaFromDb.Name = villaDTO.Name;
            villaFromDb.Occupancy = villaDTO.Occupancy;
            villaFromDb.SquareMeters = villaDTO.SquareMeters;
            _context.Villas.Update(villaFromDb);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
