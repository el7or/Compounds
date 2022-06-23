using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Groups;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;


namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompoundGroupsController : ControllerBase
    {
        private readonly ICompoundGroupService groupService;
        private readonly ICompoundUnitService unitService;

        public CompoundGroupsController(ICompoundGroupService groupService,
            ICompoundUnitService unitService)
        {
            this.groupService = groupService;
            this.unitService = unitService;
        }

        [HttpGet]
        public IEnumerable<CompoundGroup> Get()
        {
            return groupService.GetGroups();
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var groups = groupService.GetGroupById(id);
            return Ok(new PuzzleApiResponse(result: groups));
        }

        [HttpGet("{id}/units")]
        public IActionResult GetGroupUnits(Guid id)
        {
            var units = unitService.GetUnitsByGroupId(id);
            return Ok(new PuzzleApiResponse(result: units));
        }

        [HttpGet("{id}/subGroups")]
        public IActionResult GetGroupSubGroups(Guid id)
        {
            var subGroups = groupService.GetSubGroups(id);

            return Ok(new PuzzleApiResponse(result: subGroups));
        }

        [HttpPost]
        public ActionResult Post(AddEditCompoundGroupViewModel group)
        {
            var result = groupService.AddGroup(group);

            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "Group is already exists in this compound!"));
            }
            else if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "Parent group is not exists!"));
            }
            else if (result == Common.Enums.OperationState.Created)
            {
                return Ok(new PuzzleApiResponse(result: new
                {
                    compoundGroupId = group.CompoundGroupId
                }));
            }
            else
            {
                return Ok(new PuzzleApiResponse(message: "Unable to add group!"));
            }
        }

        [HttpPut]
        public ActionResult Put(AddEditCompoundGroupViewModel group)
        {
            var result = groupService.EditGroup(group);

            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "Group is already exists in this compound!"));
            }
            else if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "Parent group is not exists!"));
            }
            else if (result == Common.Enums.OperationState.Updated)
            {
                return Ok(new PuzzleApiResponse(result: new
                {
                    compoundGroupId = group.CompoundGroupId
                }));
            }
            else
            {
                return Ok(new PuzzleApiResponse(message: "Unable to update group!"));
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var result = groupService.DeleteGroup(id);

            if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "Compound group not exists!"));
            }
            else if (result == Common.Enums.OperationState.Deleted)
            {
                return Ok(new PuzzleApiResponse(result: "Compound group deleted successfully"));
            }

            return Ok(new PuzzleApiResponse(message: "Compound group can't be deleted"));
        }
    }
}
