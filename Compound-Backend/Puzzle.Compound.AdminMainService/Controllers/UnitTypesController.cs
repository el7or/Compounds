using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.UnitTypes;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;


namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitTypesController : ControllerBase
    {
        private readonly ICompoundUnitTypeService unitTypeService;
        private readonly IMapper mapper;

        public UnitTypesController(ICompoundUnitTypeService unitTypeService,
            IMapper mapper)
        {
            this.unitTypeService = unitTypeService;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var unitTypes = unitTypeService.GetUnitTypes();
            var mappedUnitTypes = mapper.Map<IEnumerable<CompoundUnitType>, IEnumerable<UnitTypeInfoViewModel>>(unitTypes);

            return Ok(new PuzzleApiResponse(result: mappedUnitTypes));
        }

        [HttpGet("{id}")]
        public CompoundUnitType Get(int id)
        {
            return unitTypeService.GetUnitTypeById(id);
        }

        [HttpPost]
        public ActionResult Post([FromBody] CompoundUnitType unitType)
        {
            var result = unitTypeService.AddUnitType(unitType);
            if (result != null)
                return Ok(result);

            return Ok(new
            {
                message = "Unit Type can't be added"
            });
        }

        [HttpPut]
        public ActionResult Put([FromBody] CompoundUnitType unitType)
        {
            var result = unitTypeService.EditUnitType(unitType);
            if (result != null)
                return Ok(result);

            return Ok(new
            {
                message = "Unit Type can't be updated"
            });
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var result = unitTypeService.DeleteUnitType(id);
            if (result != "")
                return Ok(result);

            return NotFound("");
        }
    }
}
