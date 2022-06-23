using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Models.CompanyUserRoles;
using Puzzle.Compound.Services;
using System;


namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyUserRolesController : ControllerBase
    {
        private readonly ICompanyUserRoleService companyUserRoleService;

        public CompanyUserRolesController(ICompanyUserRoleService companyUserRoleService)
        {
            this.companyUserRoleService = companyUserRoleService;
        }

        [HttpPost("filter")]
        public PagedOutput<CompanyUserRoleInfoViewModel> Get(CompanyUserRoleFilterByTextInputViewModel model)
        {
            return companyUserRoleService.GetUserRoles(model);
        }

        [HttpPost("{companyUserId}/roles")]
        public PagedOutput<CompanyUserRoleInfoViewModel> GetRoles(CompanyUserRoleFilterByTextInputViewModel model)
        {
            return companyUserRoleService.GetUserRoles(model);
        }

        [HttpPost("{compoundId}/companyrole-userroles")]
        public PagedOutput<CompanyUserRoleInfoViewModel> GetCompanyRoleUserRoles(CompanyUserRoleFilterByTextInputViewModel model)
        {
            return companyUserRoleService.GetUserRolesByCompanyRoleId(model);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var role = companyUserRoleService.GetUserRoleById(id);
            return Ok(new PuzzleApiResponse(result: role));
        }

        [HttpPost]
        public ActionResult Post(AddEditCompanyUserRoleViewModel role)
        {
            var result = companyUserRoleService.AddUserRole(role);

            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "User role is already exists!"));
            }
            else if (result == Common.Enums.OperationState.Created)
            {
                return Ok(new PuzzleApiResponse(result: new
                {
                    companyRoleId = role.CompanyRoleId
                }));
            }
            else
            {
                return Ok(new PuzzleApiResponse(message: "Unable to add user role!"));
            }
        }

        [HttpPut]
        public ActionResult Put(AddEditCompanyUserRoleViewModel role)
        {
            var result = companyUserRoleService.EditUserRole(role);

            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "User role is already exists!"));
            }
            else if (result == Common.Enums.OperationState.Updated)
            {
                return Ok(new PuzzleApiResponse(result: new
                {
                    companyRoleId = role.CompanyRoleId
                }));
            }
            else
            {
                return Ok(new PuzzleApiResponse(message: "Unable to update user role!"));
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var result = companyUserRoleService.DeleteUserRole(id);

            if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "User role not exists!"));
            }
            else if (result == Common.Enums.OperationState.Deleted)
            {
                return Ok(new PuzzleApiResponse(result: "User role deleted successfully"));
            }

            return Ok(new PuzzleApiResponse(message: "User role can't be deleted"));
        }

        [HttpPut("update-list")]
        public ActionResult UpdateUsersRoles(UpdateUserRoles model)
        {
            var result = companyUserRoleService.UpdateUsersRolesAsync(model);

            return Ok(result);
        }
    }
}
