using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;


namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlansController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IPlanService planService;

        public PlansController(IMapper mapper, IPlanService planService)
        {
            this.mapper = mapper;
            this.planService = planService;
        }

        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var plans = planService.GetPlans();

                return Ok(new PuzzleApiResponse(result: plans));
            }
            catch (Exception ex)
            {
                return Ok(new PuzzleApiResponse(message: ex.Message));
            }
        }

        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            try
            {
                var plan = planService.GetPlanById(id);
                if (plan == null)
                {
                    return Ok(new PuzzleApiResponse(message: "Plan is not exist!"));
                }

                return Ok(new PuzzleApiResponse(result: plan));
            }
            catch(Exception ex)
            {
                return Ok(new PuzzleApiResponse(message: ex.Message));
            }
        }
    }
}
