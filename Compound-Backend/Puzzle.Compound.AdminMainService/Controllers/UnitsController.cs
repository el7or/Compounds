using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Core.Models;
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
    public class UnitsController : ControllerBase
    {
        private readonly ICompoundUnitService unitService;
        private readonly IOwnerUnitService ownerUnitService;
        private readonly IMapper mapper;

        public UnitsController(ICompoundUnitService unitService,
            IOwnerUnitService ownerUnitService,
            IMapper mapper)
        {
            this.unitService = unitService;
            this.ownerUnitService = ownerUnitService;
            this.mapper = mapper;
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var unit = unitService.GetUnitById(id);
            var mappedUnit = mapper.Map<CompoundUnit, UnitInfoViewModel>(unit);

            return Ok(new PuzzleApiResponse(result: mappedUnit));
        }

        [HttpGet("{id}/owners")]
        public async Task<IActionResult> GetUnitMainOwners(Guid id)
        {
            var owners = await ownerUnitService.GetUnitMainOwnersAsync(id, null);
            return Ok(new PuzzleApiResponse(result: owners));
        }

        [HttpPost("filter")]
        public async Task<IActionResult> FilterUnits(UnitFilterByTextInputViewModel model)
        {
            var units = await unitService.FilterUnitsAsync(model);

            return Ok(new PuzzleApiResponse(result: units));
        }

        [HttpGet("sub-owners")]
        public async Task<IActionResult> GetUnitSubOwners(Guid unitId, Guid mainOwnerRegistrationId)
        {
            var owners = await ownerUnitService.GetUnitSubOwnersAsync(unitId, mainOwnerRegistrationId);
            return Ok(new PuzzleApiResponse(result: owners));
        }

        [HttpPost]
        public ActionResult Post(AddEditUnitViewModel unit)
        {
            var mappedUnit = mapper.Map<AddEditUnitViewModel, CompoundUnit>(unit);
            var result = unitService.AddUnit(mappedUnit);
            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "Unit name is already exists!"));
            }
            else if (result == Common.Enums.OperationState.Created)
            {
                return Ok(new PuzzleApiResponse(result: "Unit added successfully"));
            }

            return Ok(new PuzzleApiResponse(message: "Unit can't be added"));
        }

        [HttpPut]
        public ActionResult Put(AddEditUnitViewModel unit)
        {
            var mappedUnit = mapper.Map<AddEditUnitViewModel, CompoundUnit>(unit);
            var result = unitService.EditUnit(mappedUnit);
            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "Unit name is already exists!"));
            }
            else if (result == Common.Enums.OperationState.Updated)
            {
                return Ok(new PuzzleApiResponse(result: "Unit updated successfully"));
            }

            return Ok(new PuzzleApiResponse(message: "Unit can't be updated"));
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var result = unitService.DeleteUnit(id);
            if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "Unit is not exists!"));
            }
            else if (result == Common.Enums.OperationState.Deleted)
            {
                return Ok(new PuzzleApiResponse(result: "Unit deleted successfully"));
            }

            return Ok(new PuzzleApiResponse(message: "Unit can't be deleted"));
        }

        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportUnitsOwnersAsync(AddUnitsOwnersViewModel model)
        {
            var result = await unitService.ImportUnitsOwnersAsync(model);

                return Ok(new PuzzleApiResponse(result));
        }
    }
}
