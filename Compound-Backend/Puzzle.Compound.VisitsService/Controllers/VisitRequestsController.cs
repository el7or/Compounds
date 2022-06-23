using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Models.VisitRequest;
using Puzzle.Compound.Services;
using Puzzle.Compound.VisitsService.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Puzzle.Compound.VisitsService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VisitRequestsController : ControllerBase
    {
        private readonly ILogger<VisitRequestsController> _logger;
        private readonly IVisitRequestService _visitRequestService;

        public VisitRequestsController(ILogger<VisitRequestsController> logger, IVisitRequestService visitRequestService)
        {
            _logger = logger;
            _visitRequestService = visitRequestService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VisitRequestOutputViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id, [FromHeader] Guid compoundId, [FromHeader] int timezone,
                [FromHeader] string timeZoneValue)
        {
            var result = await _visitRequestService.GetByIdAsync(id);
            if (result.DateFrom.HasValue)
            {
                result.DateFrom = !string.IsNullOrEmpty(timeZoneValue) ?
                TimeZoneInfo.ConvertTimeFromUtc(result.DateFrom.Value, TimeZoneInfo.FindSystemTimeZoneById(timeZoneValue))
                : result.DateFrom.Value.AddHours(timezone);
                result.TimeFrom = result.DateFrom.Value.ToShortTimeString();
            }
            if (result.DateTo.HasValue)
            {
                result.DateTo = !string.IsNullOrEmpty(timeZoneValue) ?
                TimeZoneInfo.ConvertTimeFromUtc(result.DateTo.Value, TimeZoneInfo.FindSystemTimeZoneById(timeZoneValue))
                : result.DateTo.Value.AddHours(timezone);
                result.TimeTo = result.DateTo.Value.ToShortTimeString();
            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(VisitRequestOutputViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Add([FromHeader] int timezone, [FromForm] VisitRequestDto model,
                [FromHeader] string timeZoneValue)
        {
            var vrVm = new VisitRequestAddInputViewModel
            {
                CarNo = model.CarNo,
                CompoundUnitId = model.CompoundUnitId,
                Days = model.Days,
                Details = model.Details,
                GroupNo = model.GroupNo,
                Type = model.Type,
                VisitorName = model.VisitorName
            };
            if (model.DateFrom.HasValue)
            {
                vrVm.DateFrom = !string.IsNullOrEmpty(timeZoneValue) ?
                TimeZoneInfo.ConvertTimeToUtc(model.DateFrom.Value, TimeZoneInfo.FindSystemTimeZoneById(timeZoneValue))
                : model.DateFrom.Value.AddHours(-timezone);
            }
            if (model.DateTo.HasValue)
            {
                vrVm.DateTo = !string.IsNullOrEmpty(timeZoneValue) ?
                TimeZoneInfo.ConvertTimeToUtc(model.DateTo.Value, TimeZoneInfo.FindSystemTimeZoneById(timeZoneValue))
                : model.DateTo.Value.AddHours(-timezone);
            }
            if (model.Files != null && model.Files.Count > 0)
            {
                foreach (var file in model.Files)
                {
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        vrVm.Files.Add(new VisitAttachmentViewModel
                        {
                            FileBytes = ms.ToArray(),
                            FileName = file.FileName
                        });
                    }
                }
            }
            var result = await _visitRequestService.AddAsync(vrVm);
            if (result.DateFrom.HasValue)
            {
                result.DateFrom = !string.IsNullOrEmpty(timeZoneValue) ?
                TimeZoneInfo.ConvertTimeFromUtc(result.DateFrom.Value,
                TimeZoneInfo.FindSystemTimeZoneById(timeZoneValue))
                : result.DateFrom.Value.AddHours(timezone);
                result.TimeFrom = result.DateFrom.Value.ToShortTimeString();
            }
            if (result.DateTo.HasValue)
            {
                result.DateTo = !string.IsNullOrEmpty(timeZoneValue) ?
                TimeZoneInfo.ConvertTimeFromUtc(result.DateTo.Value, TimeZoneInfo.FindSystemTimeZoneById(timeZoneValue))
                : result.DateTo.Value.AddHours(timezone);
                result.TimeTo = result.DateTo.Value.ToShortTimeString();
            }
            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromForm] VisitRequestUpdateDto model, [FromHeader] int timezone
                , [FromHeader] string timeZoneValue)
        {
            var vrVm = new VisitRequestUpdateViewModel
            {
                Id = model.Id,
                CarNo = model.CarNo,
                CompoundUnitId = model.CompoundUnitId,
                DateFrom = model.DateFrom.HasValue ? model.DateFrom.Value.AddHours(-timezone) : model.DateFrom,
                DateTo = model.DateTo.HasValue ? model.DateTo.Value.AddHours(-timezone) : model.DateTo,
                Days = model.Days,
                Details = model.Details,
                GroupNo = model.GroupNo,
                Type = model.Type,
                VisitorName = model.VisitorName
            };
            if (model.DateFrom.HasValue)
            {
                vrVm.DateFrom = !string.IsNullOrEmpty(timeZoneValue) ?
                TimeZoneInfo.ConvertTimeToUtc(model.DateFrom.Value, TimeZoneInfo.FindSystemTimeZoneById(timeZoneValue))
                : model.DateFrom.Value.AddHours(-timezone);
            }
            if (model.DateTo.HasValue)
            {
                vrVm.DateTo = !string.IsNullOrEmpty(timeZoneValue) ?
                TimeZoneInfo.ConvertTimeToUtc(model.DateTo.Value, TimeZoneInfo.FindSystemTimeZoneById(timeZoneValue))
                : model.DateTo.Value.AddHours(-timezone);
            }
            if (model.Files != null && model.Files.Count > 0)
            {
                foreach (var file in model.Files)
                {
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        vrVm.Files.Add(new VisitAttachmentViewModel
                        {
                            FileBytes = ms.ToArray(),
                            FileName = file.FileName
                        });
                    }
                }
            }
            await _visitRequestService.UpdateAsync(vrVm);
            return Ok();
        }

        [HttpPost("filter/compound")]
        [ProducesResponseType(typeof(PagedOutput<VisitRequestOutputViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterByCompound(VisitRequestFilterByCompoundInputViewModel model)
        {
            var result = await _visitRequestService.FilterByCompoundAsync(model);
            return Ok(result);
        }

        [HttpPost("approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Approve(VisitRequestApproveInputViewModel model)
        {
            await _visitRequestService.ApproveAsync(model);
            return Ok();
        }

        [HttpGet("cancel/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Cancel(Guid id)
        {
            await _visitRequestService.CancelAsync(id);
            return Ok();
        }

        [HttpPost("filter/company")]
        [ProducesResponseType(typeof(PagedOutput<VisitRequestOutputViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterByCompany(VisitRequestFilterByCompanyInputViewModel model)
        {
            var result = await _visitRequestService.FilterByCompanyAsync(model);
            return Ok(result);
        }

        [HttpPost("filter/unit")]
        [ProducesResponseType(typeof(PagedOutput<VisitRequestOutputViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterByUnit(VisitRequestFilterByUnitInputViewModel model, [FromHeader] Guid compoundId)
        {
            var result = await _visitRequestService.FilterByUnitAsync(model);
            return Ok(result);
        }

        [HttpPost("filter/user")]
        [ProducesResponseType(typeof(PagedOutput<VisitRequestOutputViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterByUser(VisitRequestFilterByUserInputViewModel model, [FromHeader] Guid compoundId)
        {
            var result = await _visitRequestService.FilterByUserAsync(model, compoundId);
            return Ok(result);
        }

        [HttpPost("myQrCode/{compoundUnitId}")]
        [ProducesResponseType(typeof(PagedOutput<VisitRequestOutputViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddOwnerVisit(Guid compoundUnitId)
        {
            var result = await _visitRequestService.AddOwnerVisitAsync(compoundUnitId);
            return Ok(result);
        }

        [HttpPost("filter/gate/{gateId}")]
        [ProducesResponseType(typeof(PagedOutput<VisitRequestOutputViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterByGate(VisitRequestFilterByUserInputViewModel model, Guid gateId)
        {
            var result = await _visitRequestService.FilterByGateAsync(model, gateId);
            return Ok(result);
        }

        [HttpPut("confirm-with-attachments/{visitRequestId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfirmWithAttachments(Guid visitRequestId, [FromForm] ConfirmVisitWithAttachModel model)
        {
            var attachments = new List<VisitAttachmentViewModel>();
            if (model.Files != null && model.Files.Count > 0)
                foreach (var file in model.Files)
                {
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        attachments.Add(new VisitAttachmentViewModel
                        {
                            FileBytes = ms.ToArray(),
                            FileName = file.FileName
                        });
                    }
                }
            var response = await _visitRequestService.ConfirmWithAttachments(visitRequestId, attachments);
            return Ok(response);
        }

        [HttpPost("request/qrCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RequestByQrCode(ValidateVisitCodeModel model)
        {
            var visitReq = await _visitRequestService.ValidateVisitByQrCode(model.GateId, model.Code);
            return Ok(visitReq);
        }

        [HttpPost("request/code")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RequestByCode(ValidateVisitCodeModel model)
        {
            var visitReq = await _visitRequestService.ValidateVisitByCode(model.GateId, model.Code);
            return Ok(visitReq);
        }
    }
}
