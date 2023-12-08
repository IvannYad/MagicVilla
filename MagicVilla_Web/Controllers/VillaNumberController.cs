using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Models.ViewModels;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IMapper _mapper;

        public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper)
        {
            _villaNumberService = villaNumberService;
            _mapper = mapper;
        }
        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO> villaNumberDTOs = new List<VillaNumberDTO>();
            var response = await _villaNumberService.GetAllAsync<APIResponse>();
            if (response is not null && response.IsSuccess)
                villaNumberDTOs = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result)!);

            return View(villaNumberDTOs);
        }

        [HttpGet]
        public async Task<IActionResult> CreateVillaNumber([FromServices] IVillaService villaService)
        {
            var response = await villaService.GetAllAsync<APIResponse>();
            IEnumerable<VillaDTO> villaList = new List<VillaDTO>();
            if (response is not null && response.IsSuccess)
                villaList = JsonConvert.DeserializeObject<List<VillaDTO>>(response.Result.ToString())!;

            var createVillaNumberViewModel = new CreateVillaNumberViewModel()
            {
                VillaNumber = new VillaNumberCreateDTO(),
                VillaList = villaList.Select(villa => new SelectListItem
                {
                    Text = villa.Name,
                    Value = villa.Id.ToString(),
                })
            };

            return View(createVillaNumberViewModel);
        }

        [HttpPost]
        [ActionName("CreateVillaNumber")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillaNumberPOST(CreateVillaNumberViewModel createVillaNumberViewModel)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.CreateAsync<APIResponse>(createVillaNumberViewModel.VillaNumber);
                if (response is not null && response.IsSuccess)
                    return RedirectToAction(nameof(IndexVillaNumber));
            }

            return View("CreateVillaNumber", createVillaNumberViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateVillaNumber([FromQuery]int villaNo)
        {
            var response = await _villaNumberService.GetAsync<APIResponse>(villaNo);
            if (response is not null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result)!);
                return View(_mapper.Map<VillaNumberUpdateDTO>(model));
            }
            
            return NotFound();
        }

        [HttpPost]
        [ActionName("UpdateVillaNumber")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumberPOST(VillaNumberUpdateDTO villaNumberUpdateDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.UpdateAsync<APIResponse>(villaNumberUpdateDTO);
                if (response is not null && response.IsSuccess)
                    return RedirectToAction(nameof(IndexVillaNumber));
            }

            return View("UpdateVillaNumber", villaNumberUpdateDTO);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteVillaNumber([FromQuery] int villaNo)
        {
            var response = await _villaNumberService.GetAsync<APIResponse>(villaNo);
            if (response is not null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result)!);
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        [ActionName("DeleteVillaNumber")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaNumberPOST(VillaNumberDTO villaNumberDTO)
        {
            var response = await _villaNumberService.DeleteAsync<APIResponse>(villaNumberDTO.VillaNo);
            if (response is not null && response.IsSuccess)
                return RedirectToAction(nameof(IndexVillaNumber));
            
            return View("DeleteVillaNumber", villaNumberDTO);
        }
    }
}
