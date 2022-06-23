using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Models.CompanyRoles;
using Puzzle.Compound.Services;
using System;


namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyRolesController : ControllerBase
    {
        private readonly ICompanyRoleService companyRoleService;

        public CompanyRolesController(ICompanyRoleService companyRoleService)
        {
            this.companyRoleService = companyRoleService;
        }

        [HttpPost("filter")]
        public PagedOutput<CompanyRoleInfoViewModel> Get(CompanyRoleFilterByTextInputViewModel model)
        {
            return companyRoleService.GetRolesByName(model);
        }

        [HttpPost("{companyId}/roles")]
        public PagedOutput<CompanyRoleInfoViewModel> GetCompanyRoles(CompanyRoleFilterByTextInputViewModel model)
        {
            return companyRoleService.GetRolesByCompanyId(model);
        }


        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var role = companyRoleService.GetRoleById(id);
            return Ok(new PuzzleApiResponse(result: role));
        }

        [HttpPost]
        public ActionResult Post(AddEditCompanyRoleViewModel role)
        {
            var result = companyRoleService.AddRole(role);

            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "Role is already exists!"));
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
                return Ok(new PuzzleApiResponse(message: "Unable to add role!"));
            }
        }

        [HttpPut]
        public ActionResult Put(AddEditCompanyRoleViewModel role)
        {
            var result = companyRoleService.EditRole(role);

            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "Role is already exists!"));
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
                return Ok(new PuzzleApiResponse(message: "Unable to update role!"));
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var result = companyRoleService.DeleteRole(id);

            if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "Role not exists!"));
            }
            else if (result == Common.Enums.OperationState.Deleted)
            {
                return Ok(new PuzzleApiResponse(result: "Role deleted successfully"));
            }

            return Ok(new PuzzleApiResponse(message: "Role can't be deleted"));
        }
    }
}
