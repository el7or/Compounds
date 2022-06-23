using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Models.Gates;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers {
	[Route("api/[controller]")]
	public class GatesController : ControllerBase {
		private readonly IGateService _gateService;

		public GatesController(IGateService gateService) {
			_gateService = gateService;
		}

		[HttpGet("{id}")]
		[ProducesResponseType(typeof(GateOutputViewModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> Get(Guid id) {
			var gate = await _gateService.GetByIdAsync(id);
			return Ok(new PuzzleApiResponse(gate));
		}

		[HttpPost]
		[ProducesResponseType(typeof(GateOutputViewModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> Add([FromBody] GateAddViewModel model) {
			var gate = await _gateService.AddAsync(model);
			return Ok(new PuzzleApiResponse(gate));
		}

		[HttpPut]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Update([FromBody] GateUpdateViewModel model) {
			await _gateService.UpdateAsync(model);
			return Ok(new PuzzleApiResponse(new { }));
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Delete(Guid id, Guid compoundId) {
			await _gateService.DeleteAsync(id, compoundId);
			return Ok(new PuzzleApiResponse(new { }));
		}

		[HttpPost("filter")]
		[ProducesResponseType(typeof(PagedOutput<GateFilterInputViewModel>), StatusCodes.Status200OK)]
		public async Task<IActionResult> Filter([FromBody] GateFilterInputViewModel model) {
			var result = await _gateService.FilterAsync(model);
			return Ok(new PuzzleApiResponse(result));
		}

		[HttpGet("compound/{compoundId}")]
		[ProducesResponseType(typeof(GateOutputViewModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetForCompound(Guid compoundId) {
			var gates = await _gateService.GetForCompound(compoundId);
			return Ok(new PuzzleApiResponse(gates));
		}

		[HttpGet("all")]
		[ProducesResponseType(typeof(GateOutputViewModel), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAll() {
			var gates = await _gateService.GetAll();
			return Ok(new PuzzleApiResponse(gates));
		}
	}
}
