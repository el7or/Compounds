using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Models.SystemPageActions;
using Puzzle.Compound.Models.SystemPages;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SystemPagesController : ControllerBase
    {
        private readonly ISystemPageService systemPageService;
        private readonly ISystemPageActionService systemPageActionService;

        public SystemPagesController(ISystemPageService systemPageService, ISystemPageActionService systemPageActionService)
        {
            this.systemPageService = systemPageService;
            this.systemPageActionService = systemPageActionService;
        }

        [HttpPost("filter")]
        public PagedOutput<SystemPageInfoViewModel> Get(SystemPageFilterByTextInputViewModel model)
        {
            return systemPageService.GetPages(model);
        }

        [HttpGet("role/{roleId}")]
        public IEnumerable<SystemPageInfoViewModel> GetByRole(Guid roleId)
        {
            return systemPageService.GetPagesByRole(roleId);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var page = systemPageService.GetPageById(id);
            return Ok(new PuzzleApiResponse(result: page));
        }

        [HttpGet("{id}/subPages")]
        public IActionResult GetPageSubPages(Guid id)
        {
            var subPages = systemPageService.GetSubPages(id);

            return Ok(new PuzzleApiResponse(result: subPages));
        }

        [HttpPost("{id}/actions")]
        public PagedOutput<SystemPageActionInfoViewModel> GetActions(SystemPageActionFilterByTextInputViewModel model)
        {
            return systemPageActionService.GetActions(model);
        }

        [HttpPost]
        public ActionResult Post(AddEditSystemPageViewModel page)
        {
            var result = systemPageService.AddPage(page);

            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "System page is already exists!"));
            }
            else if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "Parent system page is not exists!"));
            }
            else if (result == Common.Enums.OperationState.Created)
            {
                return Ok(new PuzzleApiResponse(result: new
                {
                    systemPageId = page.SystemPageId
                }));
            }
            else
            {
                return Ok(new PuzzleApiResponse(message: "Unable to add system page!"));
            }
        }

        [HttpPut]
        public ActionResult Put(AddEditSystemPageViewModel page)
        {
            var result = systemPageService.EditPage(page);

            if (result == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "System page is already exists!"));
            }
            else if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "Parent system page is not exists!"));
            }
            else if (result == Common.Enums.OperationState.Updated)
            {
                return Ok(new PuzzleApiResponse(result: new
                {
                    systemPageId = page.SystemPageId
                }));
            }
            else
            {
                return Ok(new PuzzleApiResponse(message: "Unable to update system page!"));
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var result = systemPageService.DeletePage(id);

            if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "System page not exists!"));
            }
            else if (result == Common.Enums.OperationState.Deleted)
            {
                return Ok(new PuzzleApiResponse(result: "System page deleted successfully"));
            }

            return Ok(new PuzzleApiResponse(message: "System page can't be deleted"));
        }
    }
}
