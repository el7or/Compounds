using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Models.CompanyUsers;
using Puzzle.Compound.Services;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class CompanyUsersValidationController : ControllerBase {
        private readonly IAuthorizationService authorizationService;

		public CompanyUsersValidationController(IAuthorizationService authorizationService) {
            this.authorizationService = authorizationService;
		}


		[HttpPost("validate")]
		public async Task<ActionResult> Post([FromBody] CompanyUserValidationViewModel data) {

			var valid = await authorizationService.Validate(data.CompanyId.ToString(), data.CompanyUserId.ToString(), data.ActionName);

			return Ok(new PuzzleApiResponse
			{
				Result = valid
			});
		}
	}
}
