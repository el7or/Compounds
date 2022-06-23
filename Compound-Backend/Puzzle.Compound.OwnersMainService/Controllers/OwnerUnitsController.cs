using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Services;
using System;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OwnerUnitsController : ControllerBase
    {
        private readonly IOwnerUnitService compoundGroupUnitService;

        public OwnerUnitsController(IOwnerUnitService compoundGroupUnitService)
        {
            this.compoundGroupUnitService = compoundGroupUnitService;
        }

        [HttpGet]
        public ActionResult Get(Guid ownerRegistrationId, Guid? companyId)
        {
            try
            {
                if (ownerRegistrationId.Equals(Guid.Empty))
                {
                    return Ok(new PuzzleApiResponse(message: "ownerRegistrationId is required!"));
                }

                var ownerUnits = compoundGroupUnitService.GetUnitsByRegistrationId(ownerRegistrationId, companyId);

                var ownerUnitsData = new
                {
                    Units = ownerUnits
                };

                return Ok(new PuzzleApiResponse(result: ownerUnitsData));
            }
            catch (Exception ex)
            {
                return Ok(new PuzzleApiResponse(message: ex.Message));
            }
        }


        [HttpGet]
        [Route("exists/{ownerRegistrationId}/{unitId}")]
        public async Task<ActionResult> Get(Guid ownerRegistrationId, Guid unitId)
        {
            try
            {
                if (ownerRegistrationId.Equals(Guid.Empty))
                {
                    return Ok(new PuzzleApiResponse(message: "ownerRegistrationId is required!"));
                }

                if (unitId.Equals(Guid.Empty))
                {
                    return Ok(new PuzzleApiResponse(message: "unitId is required!"));
                }

                var isUnitExists = await compoundGroupUnitService.IsUnitExists(ownerRegistrationId, unitId);


                return Ok(new PuzzleApiResponse(result: new
                {
                    exists = isUnitExists
                }));
            }
            catch (Exception ex)
            {
                return Ok(new PuzzleApiResponse(message: ex.Message));
            }
        }
    }
}
