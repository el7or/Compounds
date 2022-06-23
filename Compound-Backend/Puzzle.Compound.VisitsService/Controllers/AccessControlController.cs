using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Models.VisitRequest;
using Puzzle.Compound.Services;
using System.Threading.Tasks;

namespace Puzzle.Compound.VisitsService.Controllers {
	[ApiController]
	[AllowAnonymous]
	[Route("api/[controller]")]
	public class AccessControlController : ControllerBase {
		private readonly IVisitRequestService _visitRequestService;

		public AccessControlController(IVisitRequestService visitRequestService) {
			_visitRequestService = visitRequestService;
		}

		[HttpPost("qrCode")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> RequestByQrCode(RequestVisitGateByCodeViewModel model) {
			await _visitRequestService.RequestGateByQrCodeAsync(model, model.Timezone);
			return Ok();
		}

		[HttpPost("pinCode")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> RequestByCode(RequestVisitGateByCodeViewModel model) {
			await _visitRequestService.RequestGateByCodeAsync(model, model.Timezone);
			return Ok();
		}
	}
}
