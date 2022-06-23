using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Authorization;
using Puzzle.Compound.Common;
using Puzzle.Compound.Models.News;
using Puzzle.Compound.Services;
using System;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompoundNewsController : ControllerBase
    {
        private readonly ICompoundNewsService _compoundNewsService;

        public CompoundNewsController(ICompoundNewsService compoundNewsService)
        {
            _compoundNewsService = compoundNewsService;
        }

        //[ApiAuthorizationFilter("NewsList")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] NewsFilterViewModel model)
        {
            var compoundNewsList = await _compoundNewsService.GetAsync(model);
            return Ok(new PuzzleApiResponse(compoundNewsList));
        }

        //[ApiAuthorizationFilter("NewsDisplay")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var compoundNews = await _compoundNewsService.GetByIdAsync(id);
            return Ok(new PuzzleApiResponse(compoundNews));
        }

        //[ApiAuthorizationFilter("NewsAdd")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NewsInputViewModel model)
        {
            var operationState = await _compoundNewsService.AddAsync(model);
            return Ok(new PuzzleApiResponse(operationState));
        }

        //[ApiAuthorizationFilter("NewsEdit")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] NewsInputViewModel model)
        {
            var operationState = await _compoundNewsService.UpdateAsync(model);
            return Ok(new PuzzleApiResponse(operationState));
        }

        //[ApiAuthorizationFilter("NewsDelete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var operationState = await _compoundNewsService.DeleteAsync(id);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpGet("mobile-news")]
        public async Task<IActionResult> GetMobileNews([FromQuery] NewsFilterViewModel model, [FromHeader] string Language)
        {
            var compoundNewsList = await _compoundNewsService.GetMobileNewsAsync(model, Language);
            return Ok(new PuzzleApiResponse(compoundNewsList));
        }

        [HttpGet("mobile-news/{id}")]
        public async Task<IActionResult> GetMobileNewsById(Guid id, [FromHeader] string Language)
        {
            var compoundNews = await _compoundNewsService.GetMobileNewsByIdAsync(id, Language);
            return Ok(new PuzzleApiResponse(compoundNews));
        }
    }
}
