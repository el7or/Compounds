using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Models.SystemPageActions;
using Puzzle.Compound.Services;
using System;


namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SystemPageActionsController : ControllerBase
    {
        private readonly ISystemPageActionService systemPageActionService;

        public SystemPageActionsController(ISystemPageActionService systemPageActionService)
        {
            this.systemPageActionService = systemPageActionService;
        }

        [HttpPost("filter")]
        public PagedOutput<SystemPageActionInfoViewModel> Get(SystemPageActionFilterByTextInputViewModel model)
        {
            return systemPageActionService.GetActions(model);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var action = systemPageActionService.GetActionById(id);
            return Ok(new PuzzleApiResponse(result: action));
        }

        [HttpPost]
        public ActionResult Post(AddEditSystemPageActionViewModel action)
        {
            var result = systemPageActionService.AddAction(action);

            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "System page action is already exists!"));
            }
            else if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "Parent system page action is not exists!"));
            }
            else if (result == Common.Enums.OperationState.Created)
            {
                return Ok(new PuzzleApiResponse(result: new
                {
                    systemPageActionId = action.SystemPageActionId
                }));
            }
            else
            {
                return Ok(new PuzzleApiResponse(message: "Unable to add system page action!"));
            }
        }

        [HttpPut]
        public ActionResult Put(AddEditSystemPageActionViewModel action)
        {
            var result = systemPageActionService.EditAction(action);

            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "System page action is already exists!"));
            }
            else if (result == Common.Enums.OperationState.Updated)
            {
                return Ok(new PuzzleApiResponse(result: new
                {
                    systemPageId = action.SystemPageId
                }));
            }
            else
            {
                return Ok(new PuzzleApiResponse(message: "Unable to update system page action!"));
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var result = systemPageActionService.DeleteAction(id);

            if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "System page action not exists!"));
            }
            else if (result == Common.Enums.OperationState.Deleted)
            {
                return Ok(new PuzzleApiResponse(result: "System page action deleted successfully"));
            }

            return Ok(new PuzzleApiResponse(message: "System page action can't be deleted"));
        }
    }
}
