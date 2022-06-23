using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Puzzle.Compound.Services;
using System;
using System.Threading.Tasks;
using Puzzle.Compound.Common;
using Puzzle.Compound.Models.Services;
using System.IO;
using System.Collections.Generic;

namespace Puzzle.Compound.OwnersMainService.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceRequestController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IServiceRequestService _serviceRequest;
        public ServiceRequestController(IMapper mapper, IServiceRequestService serviceRequest)
        {
            _mapper = mapper;
            _serviceRequest = serviceRequest;
        }

        [HttpGet("types")]
        public async Task<ActionResult> GetCompoundServices([FromHeader] string Language
        , [FromHeader] Guid compoundId)
        {
            var services = await _serviceRequest.GetCompoundServices(compoundId, Language);
            return Ok(new PuzzleApiResponse(services));
        }

        [HttpPut("{requestId}/cancel")]
        public async Task<ActionResult> CancelServiceRequest(Guid requestId, [FromBody] CancelRequestModel cancelModel)
        {
            var operationState = await _serviceRequest.CancelRequestByOwner(requestId, cancelModel);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPost]
        public async Task<ActionResult> AddServiceRequest([FromHeader] Guid compoundId,
            [FromForm] ServiceRequestModel model)
        {
            var attachs = new List<ServiceAttachmentModel>();
            if (model.Attachments != null && model.Attachments.Count > 0)
            {
                foreach (var file in model.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        attachs.Add(new ServiceAttachmentModel
                        {
                            File = ms.ToArray(),
                            FileName = file.FileName
                        });
                    }
                }
            }
            var operationState = await _serviceRequest.AddServiceRequest(compoundId, model, attachs);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPut("{requestId}")]
        public async Task<ActionResult> UpdateServiceRequest(Guid requestId, [FromHeader] Guid compoundId,
            [FromForm] ServiceUpdateModel model)
        {
            var attachs = new List<ServiceAttachmentModel>();
            if (model.Attachments != null && model.Attachments.Count > 0)
            {
                foreach (var file in model.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        attachs.Add(new ServiceAttachmentModel
                        {
                            File = ms.ToArray(),
                            FileName = file.FileName
                        });
                    }
                }
            }
            var operationState = await _serviceRequest.UpdateServiceRequest(requestId, compoundId, model, attachs);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPut("{requestId}/rate")]
        public async Task<ActionResult> EvaluateServiceRequest(Guid requestId, [FromBody] ServiceEvaluationModel model)
        {
            var operationState = await _serviceRequest.RateServiceByOwner(requestId, model);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPost("filter")]
        public async Task<ActionResult> GetServiceRequests([FromHeader] Guid compoundId, [FromBody] OwnerServiceFilter filter)
        {
            var requests = await _serviceRequest.GetOwnerRequests(compoundId, filter);
            return Ok(new PuzzleApiResponse(requests));
        }

        [HttpGet("{requestId}")]
        public async Task<ActionResult> GetRequest(Guid requestId, [FromHeader] string Language)
        {
            var request = await _serviceRequest.GetOwnerRequest(requestId, Language);
            return Ok(new PuzzleApiResponse(request));
        }

    }
}
