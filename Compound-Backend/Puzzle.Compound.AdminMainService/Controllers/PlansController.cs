using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Plans;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;


namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlansController : ControllerBase
    {
        private readonly IPlanService planService;

        public PlansController(IPlanService planService)
        {
            this.planService = planService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Get()
        {
            var plans = planService.GetPlans();
            return Ok(new PuzzleApiResponse(result: plans));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult Get(Guid id)
        {
            var plan = planService.GetPlanById(id);
            return Ok(new PuzzleApiResponse
            {
                Result = plan
            });
        }

        [HttpPost]
        public ActionResult Post([FromBody] AddPlanViewModel plan)
        {
            var result = planService.AddPlan(plan);
            if (result == Common.Enums.OperationState.Exists) 
            {
                return Ok(new PuzzleApiResponse("Plan is already exists!"));
            }
            else if(result == Common.Enums.OperationState.Created)
            {
                return Ok(new PuzzleApiResponse("Saved successfully!", result: ""));
            }
            else
            {
                return Ok(new PuzzleApiResponse("Unable to create plan!"));
            }
        }

        [HttpPut]
        public ActionResult Put([FromBody] Plan plan)
        {
            var result = planService.EditPlan(plan);
            if (result != null)
                return Ok(result);

            return Ok(new
            {
                message = "Plan can't be updated"
            });
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var result = planService.DeletePlan(id);
            if (result != "")
                return Ok(result);

            return NotFound("");
        }
    }
}
