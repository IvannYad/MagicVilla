using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
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
            var response = await _villaService
                .GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if (response is not null && response.IsSuccess)
                villaDTOs = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result)!);

            return View(villaDTOs);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVilla()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ActionName("CreateVilla")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillaPOST(VillaCreateDTO villaCreateDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService
                    .CreateAsync<APIResponse>(villaCreateDTO, HttpContext.Session.GetString(SD.SessionToken));
                if (response is not null && response.IsSuccess)
                {
                    TempData["success"] = "Villa created successfully";
                    return RedirectToAction(nameof(IndexVilla));
                }
            }

            TempData["error"] = "Error while creating Villa";
            return View("CreateVilla", villaCreateDTO);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVilla([FromQuery]int villaId)
        {
            var response = await _villaService
                .GetAsync<APIResponse>(villaId, HttpContext.Session.GetString(SD.SessionToken));
            if (response is not null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result)!);
                return View(_mapper.Map<VillaUpdateDTO>(model));
            }

            TempData["error"] = "Error! No such Villa";
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ActionName("UpdateVilla")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaPOST(VillaUpdateDTO villaUpdateDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService
                    .UpdateAsync<APIResponse>(villaUpdateDTO, HttpContext.Session.GetString(SD.SessionToken));
                if (response is not null && response.IsSuccess)
                {
                    TempData["success"] = "Villa updated successfully";
                    return RedirectToAction(nameof(IndexVilla));
                }
            }

            TempData["error"] = "Error while creating Villa";
            return View("UpdateVilla", villaUpdateDTO);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVilla([FromQuery] int villaId)
        {
            var response = await _villaService
                .GetAsync<APIResponse>(villaId, HttpContext.Session.GetString(SD.SessionToken));
            if (response is not null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result)!);
                return View(model);
            }

            TempData["error"] = "Error! No such Villa";
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ActionName("DeleteVilla")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaPOST(VillaDTO villaDTO)
        {
            var response = await _villaService
                .DeleteAsync<APIResponse>(villaDTO.Id, HttpContext.Session.GetString(SD.SessionToken));
            if (response is not null && response.IsSuccess)
            {
                TempData["success"] = "Villa deleted successfully";
                return RedirectToAction(nameof(IndexVilla));
            }

            TempData["error"] = "Error while deleting Villa";
            return View("DeleteVilla", villaDTO);
        }
    }
}
