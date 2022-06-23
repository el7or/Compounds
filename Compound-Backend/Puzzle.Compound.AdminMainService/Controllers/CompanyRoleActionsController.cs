using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Models.CompanyRoleActions;
using Puzzle.Compound.Models.SystemPageActions;
using Puzzle.Compound.Services;
using System;


namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyRoleActionsController : ControllerBase
    {
        private readonly ICompanyRoleActionsService companyRoleActionsService;

        public CompanyRoleActionsController(ICompanyRoleActionsService companyRoleActionsService)
        {
            this.companyRoleActionsService = companyRoleActionsService;
        }

        [HttpPost("filter")]
        public PagedOutput<CompanyRoleActionInfoViewModel> Get(CompanyRoleActionFilterByTextInputViewModel model)
        {
            return companyRoleActionsService.GetRoleActions(model);
        }

        [HttpPost("{systemPageActionId}/actions")]
        public PagedOutput<CompanyRoleActionInfoViewModel> GetActionsBySystemPageActionId(CompanyRoleActionFilterByTextInputViewModel model)
        {
            return companyRoleActionsService.GetRoleActionsBySystemPageActionId(model);
        }

        [HttpPost("{companyUserRoleId}/userrole-actions")]
        public PagedOutput<CompanyRoleActionInfoViewModel> GetActionsByCompanyUserRoleId(CompanyRoleActionFilterByTextInputViewModel model)
        {
            return companyRoleActionsService.GetRoleActionsByCompanyUserRoleId(model);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var action = companyRoleActionsService.GetRoleActionById(id);
            return Ok(new PuzzleApiResponse(result: action));
        }

        [HttpPost]
        public ActionResult Post(AddEditCompanyRoleActionViewModel action)
        {
            var result = companyRoleActionsService.AddRoleAction(action);

            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "Role action is already exists!"));
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
                return Ok(new PuzzleApiResponse(message: "Unable to add role action!"));
            }
        }

        [HttpPut]
        public ActionResult Put(AddEditCompanyRoleActionViewModel action)
        {
            var result = companyRoleActionsService.EditRoleAction(action);

            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "Role action is already exists!"));
            }
            else if (result == Common.Enums.OperationState.Updated)
            {
                return Ok(new PuzzleApiResponse(result: new
                {
                    actionsInCompanyRolesId = action.ActionsInCompanyRolesId
                }));
            }
            else
            {
                return Ok(new PuzzleApiResponse(message: "Unable to update role action!"));
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var result = companyRoleActionsService.DeleteRoleAction(id);

            if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "Role action not exists!"));
            }
            else if (result == Common.Enums.OperationState.Deleted)
            {
                return Ok(new PuzzleApiResponse(result: "Role action deleted successfully"));
            }

            return Ok(new PuzzleApiResponse(message: "Role action can't be deleted"));
        }

        [HttpPut("update-list")]
        public ActionResult UpdatePagesActionsInRoles(UpdatePagesActionsInRoles model)
        {
            var result = companyRoleActionsService.UpdatePagesActionsInRoles(model);

            return Ok(result);
        }
    }
}
