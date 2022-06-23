using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Models.PrintCardRequest;
using Puzzle.Compound.Services;
using Puzzle.Compound.VisitsService.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.VisitsService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PrintCardRequestsController : ControllerBase
    {
        private readonly IPrintCardRequestService _printCardRequestService;

        public PrintCardRequestsController(IPrintCardRequestService printCardRequestService)
        {
            _printCardRequestService = printCardRequestService;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromForm]PrintCardAddDto model)
        {
            var input = new PrintCardAddViewModel
            {
                CompoundUnitId = model.CompoundUnitId,
                Details = model.Details,
                Name = model.Name,
                PictureName = model.Picture.FileName
            };
            using (var ms = new MemoryStream())
            {
                await model.Picture.CopyToAsync(ms);
                input.Picture = ms.ToArray();
            }
            var result = await _printCardRequestService.AddAsync(input);
            return Ok(result);
        }

        [HttpPost("approve")]
        public async Task<IActionResult> Approve(PrintCardApproveViewModel model)
        {
            await _printCardRequestService.ApproveOrRejectAsync(model);
            return Ok();
        }

        [HttpGet("cancelPrint/{id}")]
        public async Task<IActionResult> PrintCancel(Guid id)
        {
            await _printCardRequestService.PrintCancelAsync(id);
            return Ok();
        }

        [HttpPost("enableOrCancel")]
        public async Task<IActionResult> CancelCard(CardEnableOrCancelViewModel model)
        {
            await _printCardRequestService.CardEnableOrCancelAsync(model);
            return Ok();
        }

        [HttpPost("filter/user")]
        public async Task<IActionResult> FilterByUser(PagedInput model)
        {
            var result = await _printCardRequestService.FilterByUserAsync(model);
            return Ok(result);
        }

        [HttpPost("filter/unit")]
        public async Task<IActionResult> FilterByUnit(PrintCardFilterByUnitInputViewModel model)
        {
            var result = await _printCardRequestService.FilterByUnitAsync(model);
            return Ok(result);
        }

        [HttpPost("filter/company")]
        public async Task<IActionResult> FilterByCompany(PrintCardFilterByCompanyInputViewModel model)
        {
            var result = await _printCardRequestService.FilterByCompanyAsync(model);
            return Ok(result);
        }

        [HttpPost("filter/compound")]
        public async Task<IActionResult> FilterByCompound(PrintCardFilterByCompoundInputViewModel model)
        {
            var result = await _printCardRequestService.FilterByCompoundAsync(model);
            return Ok(result);
        }
    }
}
