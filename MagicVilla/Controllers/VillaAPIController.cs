﻿using AutoMapper;
using Azure;
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
        private readonly IMapper _mapper;
        public VillaAPIController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            var villas = await _context.Villas.AsNoTracking().ToListAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villas));
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        // Following attridutes specifies what status codes method can return.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }


            var villa = await _context.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDTO>(villa));
        }

        [HttpPost(Name = "CreateVilla")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            // With [ApiController] we dont need to explicitly check ModelState.
            // With [ApiController] validation occurs before entering method.
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            if (_context.Villas
                .AsNoTracking()
                .AsEnumerable()
                .FirstOrDefault(v => v.Name.Equals(createDTO.Name, StringComparison.OrdinalIgnoreCase)) is not null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists");
                return BadRequest(ModelState);
            }

            if (createDTO is null)
                return BadRequest();

            Villa villa = _mapper.Map<Villa>(createDTO);
            await  _context.Villas.AddAsync(villa);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetVilla", new { id = villa.Id }, villa);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id <= 0)
                return BadRequest();

            var villa = await _context.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if (villa == null)
                return NotFound();

            _context.Villas.Remove(villa);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            if (id != updateDTO.Id || updateDTO is null)
                return BadRequest();

            var villaFromDb = await _context.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            if (villaFromDb == null)
                return NotFound();

            villaFromDb = _mapper.Map<Villa>(updateDTO);
            _context.Villas.Update(villaFromDb);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (id <= 0 || patchDTO is null)
                return BadRequest();

            var villaFromDb = await _context.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            if (villaFromDb == null)
                return NotFound();
            
            VillaUpdateDTO updateDTO = _mapper.Map<VillaUpdateDTO>(villaFromDb);
            patchDTO.ApplyTo(updateDTO, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            villaFromDb = _mapper.Map<Villa>(updateDTO);
            _context.Villas.Update(villaFromDb);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
