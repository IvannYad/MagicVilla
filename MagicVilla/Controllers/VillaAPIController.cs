﻿using Azure;
using MagicVilla.Data;
using MagicVilla.Logging;
using MagicVilla.Models;
using MagicVilla.Models.DTO;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.JsonPatch;
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
            var villas = _context.Villas.AsNoTracking().ToList();
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
            {
                return BadRequest();
            }


            var villa = _context.Villas.FirstOrDefault(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost(Name = "CreateVilla")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaCreateDTO villaDTO)
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

            Villa villa = new()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                SquareMeters = villaDTO.SquareMeters,
            };
            _context.Villas.Add(villa);
            _context.SaveChanges();
            return CreatedAtRoute("GetVilla", new { id = villa.Id }, villa);
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
        public IActionResult UpdateVilla(int id, [FromBody] VillaUpdateDTO villaDTO)
        {
            if (id != villaDTO.Id || villaDTO is null)
                return BadRequest();

            var villaFromDb = _context.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id);
            if (villaFromDb == null)
                return NotFound();

            villaFromDb = new()
            {
                Id = villaDTO.Id,
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                SquareMeters = villaDTO.SquareMeters,
            };
            _context.Villas.Update(villaFromDb);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (id == 0 || patchDTO is null)
                return BadRequest();

            var villaFromDb = _context.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id);
            if (villaFromDb == null)
                return NotFound();
            
            VillaUpdateDTO villaDTO = new()
            {
                Id = villaFromDb.Id,
                Amenity = villaFromDb.Amenity,
                Details = villaFromDb.Details,
                ImageUrl = villaFromDb.ImageUrl,
                Name = villaFromDb.Name,
                Occupancy = villaFromDb.Occupancy,
                Rate = villaFromDb.Rate,
                SquareMeters = villaFromDb.SquareMeters,
            };
            patchDTO.ApplyTo(villaDTO, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            villaFromDb = new()
            {
                Id = villaDTO.Id,
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                SquareMeters = villaDTO.SquareMeters,
            };
            _context.Villas.Update(villaFromDb);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
