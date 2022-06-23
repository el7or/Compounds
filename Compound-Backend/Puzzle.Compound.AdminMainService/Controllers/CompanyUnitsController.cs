using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Services;
using System;


namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyUnitsController : ControllerBase
    {
        private readonly ICompoundUnitService compoundUnitService;

        public CompanyUnitsController(ICompoundUnitService compoundUnitService)
        {
            this.compoundUnitService = compoundUnitService;
        }

        [HttpGet]
        public ActionResult Get(Guid companyId, string unitName)
        {
            unitName = unitName?.ToLower();
            var unitInfo = compoundUnitService.GetUnitsByCompanyId(companyId, unitName);

            if (unitInfo == null)
            {
                return Ok(new PuzzleApiResponse(message: "Unit not found!"));
            }

            return Ok(new PuzzleApiResponse(result: unitInfo));
        }

    }
}
