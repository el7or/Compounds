using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Models.Services;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceSubTypesController : ControllerBase
    {
        private readonly IServiceSubTypeService _subTypeService;

        public ServiceSubTypesController(IServiceSubTypeService subTypeService)
        {
            _subTypeService = subTypeService;
        }

        [HttpGet("{compoundId:Guid}/{serviceTypeId:Guid}")]
        public async Task<IActionResult> GetSubTypesByTypeId(Guid compoundId, Guid serviceTypeId)
        {
            var serviceSubTypes = await _subTypeService.GetSubTypesByTypeIdAsync(compoundId, serviceTypeId);
            return Ok(new PuzzleApiResponse(serviceSubTypes));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceSubTypeInputViewModel model)
        {
            var result = await _subTypeService.AddAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ServiceSubTypeInputViewModel model)
        {
            var result = await _subTypeService.UpdateAsync(model);
            return Ok(new PuzzleApiResponse(result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var operationState = await _subTypeService.DeleteAsync(id);
            return Ok(new PuzzleApiResponse(operationState));
        }
    }
}
