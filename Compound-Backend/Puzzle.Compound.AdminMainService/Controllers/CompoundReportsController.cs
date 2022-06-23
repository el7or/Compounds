using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Models.CompoundReports;
using Puzzle.Compound.Services;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CompoundReportsController : PuzzleBaseController {
		private readonly ICompoundReportService _reportServ;
		public CompoundReportsController(IMapper mapper, ICompoundReportService reportServ) : base(mapper) {
			_reportServ = reportServ;
		}

		[HttpPost]
		public async Task<ActionResult> GetCompoundsReports(CompoundReportInput compounds) {
			var services = await _reportServ.GetCompoundsReports(compounds);
			return Ok(new PuzzleApiResponse(services));
		}

	}
}
