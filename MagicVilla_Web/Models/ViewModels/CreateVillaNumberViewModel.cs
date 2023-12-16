using MagicVilla_Utility;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace MagicVilla_Web.Models.ViewModels
{
    public class CreateVillaNumberViewModel
    {
        public VillaNumberCreateDTO VillaNumber { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> VillaList { get; set; }
        public async Task FillVillaList(IVillaService villaService, HttpContext httpContext)
        {
            if (VillaList is not null && VillaList.Any())
                return;
            
            var responseVillaList = await villaService
                .GetAllAsync<APIResponse>(httpContext.Session.GetString(SD.SessionToken));
            IEnumerable<VillaDTO> villaList = new List<VillaDTO>();
            if (responseVillaList is not null && responseVillaList.IsSuccess)
                villaList = JsonConvert.DeserializeObject<List<VillaDTO>>(responseVillaList.Result.ToString())!;
            
            this.VillaList = villaList.Select(villa => new SelectListItem
            {
                Text = villa.Name,
                Value = villa.Id.ToString(),
            });
        }
    }
}
