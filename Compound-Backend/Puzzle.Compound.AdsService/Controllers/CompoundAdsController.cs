using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Authorization;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Models.Ads;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdsService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CompoundAdsController : ControllerBase
    {
        private readonly ICompoundAdService _compoundAdService;

        public CompoundAdsController(ICompoundAdService compoundAdService)
        {
            _compoundAdService = compoundAdService;
        }

        //[ApiAuthorizationFilter("AdsList")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AdFilterViewModel model)
        {
            var compoundAdList = await _compoundAdService.GetAsync(model);
            return Ok(new PuzzleApiResponse(compoundAdList));
        }

        //[ApiAuthorizationFilter("AdsDisplay")]
        [HttpGet("{adId:Guid}")]
        public async Task<IActionResult> GetById(Guid adId)
        {
            var compoundAd = await _compoundAdService.GetByIdAsync(adId);
            return Ok(new PuzzleApiResponse(compoundAd));
        }

        //[ApiAuthorizationFilter("AdsAdd")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdInputViewModel model)
        {
            var operationState = await _compoundAdService.AddAsync(model);
            return Ok(new PuzzleApiResponse(operationState));
        }

        //[ApiAuthorizationFilter("AdsEdit")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] AdInputViewModel model)
        {
            var operationState = await _compoundAdService.UpdateAsync(model);
            return Ok(new PuzzleApiResponse(operationState));
        }

        //[ApiAuthorizationFilter("AdsDelete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var operationState = await _compoundAdService.DeleteAsync(id);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpGet("getList")]
        public async Task<IActionResult> GetMobileAd([FromQuery] AdFilterViewModel model, [FromHeader] string Language)
        {
            var compoundAdList = await _compoundAdService.GetMobileAdsAsync(model, Language);
            return Ok(new PuzzleApiResponse(compoundAdList));
        }

        [HttpGet("getDetails/{compoundAdId:Guid}/{ownerRegistrationId:Guid}")]
        public async Task<IActionResult> GetMobileAdById(Guid compoundAdId, Guid ownerRegistrationId, [FromHeader] string Language)
        {
            var compoundAd = await _compoundAdService.GetMobileAdByIdAsync(compoundAdId, ownerRegistrationId, Language);
            return Ok(new PuzzleApiResponse(compoundAd));
        }

        [HttpGet("postAdAction/{compoundAdId:Guid}/{ownerRegistrationId:Guid}/{actionType:int}")]
        public async Task<IActionResult> PostAdAction(Guid compoundAdId, Guid ownerRegistrationId, ActionType actionType)
        {
            var compoundAd = await _compoundAdService.PostAdActionAsync(compoundAdId, ownerRegistrationId, actionType);
            return Ok(new PuzzleApiResponse(compoundAd));
        }
    }
}
