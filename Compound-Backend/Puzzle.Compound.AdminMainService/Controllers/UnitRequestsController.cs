using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Models.UnitRequests;
using Puzzle.Compound.Services;
using System;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class UnitRequestsController : ControllerBase {
		private readonly IOwnerUnitRequestService ownerUnitRequestService;

		public UnitRequestsController(IOwnerUnitRequestService ownerUnitRequestService) {
			this.ownerUnitRequestService = ownerUnitRequestService;
		}

		[HttpGet("{requestId}/approve")]
		public IActionResult ApproveRequest(Guid requestId) {
			var result = ownerUnitRequestService.ApproveRequest(requestId);
			if (result == Common.Enums.OperationState.Created) {
				return Ok(new PuzzleApiResponse(result: "Request approved successfully"));
			} else if (result == Common.Enums.OperationState.UpdatedBefore) {
				return Ok(new PuzzleApiResponse(result: "Request is approved before"));
			} else if (result == Common.Enums.OperationState.NotExists) {
				return Ok(new PuzzleApiResponse(message: "Request Id not found!"));
			} else {
				return Ok(new PuzzleApiResponse(message: "Unable to approve this request!"));
			}
		}

		[HttpPost("approve")]
		public async Task<IActionResult> ApproveRequest(UnitRequestApprove unitRequest) {
			var result = await ownerUnitRequestService.ApproveRequest(unitRequest.CompoundOwnerId, unitRequest.OwnerRegistrationId);
			if (result == Common.Enums.OperationState.Updated) {
				return Ok(new PuzzleApiResponse(result: "Request approved successfully"));
			} else if (result == Common.Enums.OperationState.NotExists) {
				return Ok(new PuzzleApiResponse(message: "Owner is not found!"));
			} else {
				return Ok(new PuzzleApiResponse(message: "Unable to approve this request!"));
			}
		}
	}
}
