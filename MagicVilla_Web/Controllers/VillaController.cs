using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public VillaController(IVillaService villaService, IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;
        }
        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDTO> villaDTOs = new List<VillaDTO>();
            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response is not null && response.IsSuccess)
                villaDTOs = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result)!);

            return View(villaDTOs);
        }

        [HttpGet]
        public async Task<IActionResult> CreateVilla()
        {
            return View();
        }

        [HttpPost]
        [ActionName("CreateVilla")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillaPOST(VillaCreateDTO villaCreateDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.CreateAsync<APIResponse>(villaCreateDTO);
                if (response is not null && response.IsSuccess)
                    return RedirectToAction(nameof(IndexVilla));
            }

            return View("CreateVilla", villaCreateDTO);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateVilla([FromQuery]int villaId)
        {
            var response = await _villaService.GetAsync<APIResponse>(villaId);
            if (response is not null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result)!);
                return View(_mapper.Map<VillaUpdateDTO>(model));
            }
            
            return NotFound();
        }

        [HttpPost]
        [ActionName("UpdateVilla")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaPOST(VillaUpdateDTO villaUpdateDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.UpdateAsync<APIResponse>(villaUpdateDTO);
                if (response is not null && response.IsSuccess)
                    return RedirectToAction(nameof(IndexVilla));
            }

            return View("UpdateVilla", villaUpdateDTO);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteVilla([FromQuery] int villaId)
        {
            var response = await _villaService.GetAsync<APIResponse>(villaId);
            if (response is not null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result)!);
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [ActionName("DeleteVilla")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaPOST(VillaDTO villaDTO)
        {
            var response = await _villaService.DeleteAsync<APIResponse>(villaDTO.Id);
            if (response is not null && response.IsSuccess)
                return RedirectToAction(nameof(IndexVilla));
            
            return View("DeleteVilla", villaDTO);
        }
    }
}
