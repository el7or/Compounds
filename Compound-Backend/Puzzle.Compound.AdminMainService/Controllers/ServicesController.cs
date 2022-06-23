using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Models.Services;
using Puzzle.Compound.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServicesController : PuzzleBaseController
    {
        private readonly IServiceRequestService _serviceRequest;

        public ServicesController(IMapper mapper, IServiceRequestService serviceRequest) : base(mapper)
        {
            _serviceRequest = serviceRequest;
        }

        [HttpGet("compound-services/{compoundId}")]
        public async Task<ActionResult> GetCompoundServices(Guid compoundId)
        {
            var services = await _serviceRequest.GetServiceAssignment(compoundId);
            return Ok(new PuzzleApiResponse(services));
        }

        [HttpPut("compound-services/{compoundId}")]
        public async Task<ActionResult> UpdateCompoundServices(Guid compoundId, [FromBody] CompoundServiceModel[] assignments)
        {
            var operationState = await _serviceRequest.UpdateServiceAssignment(compoundId, assignments);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPost]
        public async Task<ActionResult> GetServiceRequests(ServiceRequestFilter filter)
        {
            var requests = await _serviceRequest.GetServiceRequests(filter);
            return Ok(new PuzzleApiResponse(requests));
        }

        [HttpPut("{requestId}/status")]
        public async Task<ActionResult> UpdateRequestStatus(Guid requestId, [FromBody] UpdateStatusModel status)
        {
            var operationState = await _serviceRequest.UpdateRequestStatus(requestId, status.Status);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPut("{requestId}/comment")]
        public async Task<ActionResult> AddRequestComment(Guid requestId, [FromBody] AddCommentModel commentModel)
        {
            var operationState = await _serviceRequest.AddRequestComment(requestId, commentModel);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpGet("{requestId}")]
        public async Task<ActionResult> GetServiceRequest(Guid requestId)
        {
            var request = await _serviceRequest.GetServiceRequest(requestId);
            return Ok(new PuzzleApiResponse(request));
        }

        [HttpPut("type-icon/{serviceTypeId}")]
        public async Task<ActionResult> UpdateServiceIcon(Guid serviceTypeId, [FromForm] UpdateServiceIconModel iconModel)
        {
            using (var ms = new MemoryStream())
            {
                await iconModel.Icon.CopyToAsync(ms);
                await _serviceRequest.UpdateServiceIcon(serviceTypeId, iconModel.Icon.FileName, ms.ToArray());
            }
            return Ok();
        }

        [HttpPost("compounds-services")]
        public async Task<ActionResult> GetCompoundsServices(CompoundServiceInput compounds)
        {
            var services = await _serviceRequest.GetCompoundsServices(compounds);
            return Ok(new PuzzleApiResponse(services));
        }

    }
}
