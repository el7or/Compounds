using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Puzzle.Compound.Services;
using System;
using System.Threading.Tasks;
using Puzzle.Compound.Common;
using Puzzle.Compound.Models.Issues;
using System.IO;
using System.Collections.Generic;

namespace Puzzle.Compound.OwnersMainService.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IssueRequestController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IIssueRequestService _issueRequest;
        public IssueRequestController(IMapper mapper, IIssueRequestService issueRequest)
        {
            _mapper = mapper;
            _issueRequest = issueRequest;
        }

        [HttpGet("types")]
        public async Task<ActionResult> GetCompoundIssues([FromHeader] string Language
        , [FromHeader] Guid compoundId)
        {
            var issues = await _issueRequest.GetCompoundIssues(compoundId, Language);
            return Ok(new PuzzleApiResponse(issues));
        }

        [HttpPut("{requestId}/cancel")]
        public async Task<ActionResult> CancelIssueRequest(Guid requestId, [FromBody] CancelIssueRequestModel cancelModel)
        {
            var operationState = await _issueRequest.CancelRequestByOwner(requestId, cancelModel);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPost]
        public async Task<ActionResult> AddIssueRequest([FromHeader] Guid compoundId,
            [FromForm] IssueRequestModel model)
        {
            var attachs = new List<IssueAttachmentModel>();
            if (model.Attachments != null && model.Attachments.Count > 0)
            {
                foreach (var file in model.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        attachs.Add(new IssueAttachmentModel
                        {
                            File = ms.ToArray(),
                            FileName = file.FileName
                        });
                    }
                }
            }
            var operationState = await _issueRequest.AddIssueRequest(compoundId, model, attachs);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPut("{requestId}")]
        public async Task<ActionResult> UpdateIssueRequest(Guid requestId, [FromHeader] Guid compoundId,
            [FromForm] IssueUpdateModel model)
        {
            var attachs = new List<IssueAttachmentModel>();
            if (model.Attachments != null && model.Attachments.Count > 0)
            {
                foreach (var file in model.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        attachs.Add(new IssueAttachmentModel
                        {
                            File = ms.ToArray(),
                            FileName = file.FileName
                        });
                    }
                }
            }
            var operationState = await _issueRequest.UpdateIssueRequest(requestId, compoundId, model, attachs);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPut("{requestId}/rate")]
        public async Task<ActionResult> EvaluateIssueRequest(Guid requestId, [FromBody] IssueEvaluationModel model)
        {
            var operationState = await _issueRequest.RateIssueByOwner(requestId, model);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPost("filter")]
        public async Task<ActionResult> GetIssueRequests([FromHeader] Guid compoundId, [FromBody] OwnerIssueFilter filter)
        {
            var requests = await _issueRequest.GetOwnerRequests(compoundId, filter);
            return Ok(new PuzzleApiResponse(requests));
        }

        [HttpGet("{requestId}")]
        public async Task<ActionResult> GetRequest(Guid requestId, [FromHeader] string Language)
        {
            var request = await _issueRequest.GetOwnerRequest(requestId, Language);
            return Ok(new PuzzleApiResponse(request));
        }

    }
}
