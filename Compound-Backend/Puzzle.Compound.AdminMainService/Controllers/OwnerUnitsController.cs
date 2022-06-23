using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.OwnerUnits;
using Puzzle.Compound.Models.Units;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OwnerUnitsController : ControllerBase
    {
        private readonly IOwnerUnitService ownerUnitService;
        private readonly ICompoundOwnerService compoundOwnerService;
        private readonly IMapper mapper;

        public OwnerUnitsController(IOwnerUnitService ownerUnitService,
            ICompoundOwnerService compoundOwnerService,
            IMapper mapper)
        {
            this.ownerUnitService = ownerUnitService;
            this.compoundOwnerService = compoundOwnerService;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(Guid ownerId, Guid? companyId)
        {
            var units = ownerUnitService.GetUnitsByOwnerId(ownerId, companyId);
            var mappedUnits = mapper.Map<IEnumerable<UnitInfoMap>, IEnumerable<UnitInfoViewModel>>(units);

            return Ok(new PuzzleApiResponse(result: mappedUnits));
        }

        [HttpPost]
        public ActionResult Post(AddOwnerUnitsViewModel addOwnerUnits)
        {
            var existingOwner = compoundOwnerService.GetCompoundOwnerById(addOwnerUnits.CompoundOwnerId);
            if (existingOwner == null)
            {
                return Ok(new PuzzleApiResponse(message: "Owner is not exists!"));
            }

            var result = ownerUnitService.AddOwnerUnits(addOwnerUnits.CompoundOwnerId, addOwnerUnits.Units);

            if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(result: "One of the provided units not exists!"));
            }
            else if (result == Common.Enums.OperationState.Created)
            {
                return Ok(new PuzzleApiResponse(result: "Units assigned successfully"));
            }

            return Ok(new PuzzleApiResponse(message: "Unable to assign the units!"));
        }

    }
}
