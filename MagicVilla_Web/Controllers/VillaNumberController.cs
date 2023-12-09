using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Models.ViewModels;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using MagicVilla_Web.Services;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper, IVillaService villaService)
        {
            _villaNumberService = villaNumberService;
            _mapper = mapper;
            _villaService = villaService;
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
        public async Task<IActionResult> CreateVillaNumber()
        {
            var response = await _villaService.GetAllAsync<APIResponse>();
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

                ModelState.AddModelError("VillaNumber.VillaNo", string.Join('\n', response.ErrorMessages));
            }

            await createVillaNumberViewModel.FillVillaList(_villaService);

            return View(nameof(CreateVillaNumber), createVillaNumberViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateVillaNumber([FromQuery]int villaNo)
        {
            var response = await _villaNumberService.GetAsync<APIResponse>(villaNo);
            if (response is not null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result)!);
                var updateVillaNumberViewModel = new UpdateVillaNumberViewModel()
                {
                    VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(model),
                };
                await updateVillaNumberViewModel.FillVillaList(_villaService);
                return View(updateVillaNumberViewModel);
            }
            
            return NotFound();
        }

        [HttpPost]
        [ActionName("UpdateVillaNumber")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumberPOST([FromQuery] int villaNo
            , UpdateVillaNumberViewModel updateVillaNumberViewModel)
        {
            if (ModelState.IsValid)
            {
                var responseUpdate = await _villaNumberService
                    .UpdateAsync<APIResponse>(villaNo, updateVillaNumberViewModel.VillaNumber);
                if (responseUpdate is not null && responseUpdate.IsSuccess)
                    return RedirectToAction(nameof(IndexVillaNumber));

                ModelState.AddModelError("VillaNumber.VillaNo", string.Join('\n', responseUpdate.ErrorMessages));
            }

            await updateVillaNumberViewModel.FillVillaList(_villaService);
            var responseVillaNumber = await _villaNumberService.GetAsync<APIResponse>(villaNo);
            if (responseVillaNumber is not null && responseVillaNumber.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(responseVillaNumber.Result)!);
                updateVillaNumberViewModel.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(model);
                return View(nameof(UpdateVillaNumber), updateVillaNumberViewModel);
            }

            // If error occurred while retrieving VillaNumber. 
            ModelState.AddModelError("VillaNumber.VillaNo", string.Join('\n', responseVillaNumber.ErrorMessages));
            return View(nameof(UpdateVillaNumber), updateVillaNumberViewModel);
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
            
            return RedirectToAction(nameof(DeleteVillaNumber), new { villaNo = villaNumberDTO.VillaNo});
        }
    }
}
