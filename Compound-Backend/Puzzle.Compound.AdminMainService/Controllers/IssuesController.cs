using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Models.Issues;
using Puzzle.Compound.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IssuesController : PuzzleBaseController
    {
        private readonly IIssueRequestService _issueRequest;

        public IssuesController(IMapper mapper, IIssueRequestService issueRequest) : base(mapper)
        {
            _issueRequest = issueRequest;
        }

        [HttpGet("compound-issues/{compoundId}")]
        public async Task<ActionResult> GetCompoundIssues(Guid compoundId)
        {
            var issues = await _issueRequest.GetIssueAssignment(compoundId);
            return Ok(new PuzzleApiResponse(issues));
        }

        [HttpPut("compound-issues/{compoundId}")]
        public async Task<ActionResult> UpdateCompoundIssues(Guid compoundId, [FromBody] CompoundIssueModel[] assignments)
        {
            var operationState = await _issueRequest.UpdateIssueAssignment(compoundId, assignments);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPost]
        public async Task<ActionResult> GetIssueRequests(IssueRequestFilter filter)
        {
            var requests = await _issueRequest.GetIssueRequests(filter);
            return Ok(new PuzzleApiResponse(requests));
        }

        [HttpPut("{requestId}/status")]
        public async Task<ActionResult> UpdateRequestStatus(Guid requestId, [FromBody] UpdateIssueStatusModel status)
        {
            var operationState = await _issueRequest.UpdateRequestStatus(requestId, status.Status);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPut("{requestId}/comment")]
        public async Task<ActionResult> AddRequestComment(Guid requestId, [FromBody] AddIssueCommentModel commentModel)
        {
            var operationState = await _issueRequest.AddRequestComment(requestId, commentModel);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpGet("{requestId}")]
        public async Task<ActionResult> GetIssueRequest(Guid requestId)
        {
            var request = await _issueRequest.GetIssueRequest(requestId);
            return Ok(new PuzzleApiResponse(request));
        }

        [HttpPut("type-icon/{issueTypeId}")]
        public async Task<ActionResult> UpdateIssueIcon(Guid issueTypeId, [FromForm] UpdateIssueIconModel iconModel)
        {
            using (var ms = new MemoryStream())
            {
                await iconModel.Icon.CopyToAsync(ms);
                await _issueRequest.UpdateIssueIcon(issueTypeId, iconModel.Icon.FileName, ms.ToArray());
            }
            return Ok();
        }

        [HttpPost("compounds-issues")]
        public async Task<ActionResult> GetCompoundsIssues(CompoundIssueInput compounds)
        {
            var issues = await _issueRequest.GetCompoundsIssues(compounds);
            return Ok(new PuzzleApiResponse(issues));
        }

    }
}
