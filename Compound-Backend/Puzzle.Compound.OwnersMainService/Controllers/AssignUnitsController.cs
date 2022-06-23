using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Models.Units;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssignUnitsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IOwnerAssignedUnitService ownerAssignedUnitService;

        public AssignUnitsController(IMapper mapper, IOwnerAssignedUnitService ownerAssignedUnitService)
        {
            this.mapper = mapper;
            this.ownerAssignedUnitService = ownerAssignedUnitService;
        }

        [HttpGet]
        public ActionResult Get(Guid ownerRegistrationId, Guid? companyId)
        {
            try
            {
                var ownerUnits = ownerAssignedUnitService.GetUnits(ownerRegistrationId, companyId);
                var mappedOwnerUnits = mapper.Map<IEnumerable<UnitInfoMap>, IEnumerable<UnitInfoViewModel>>(ownerUnits);

                var ownerUnitsData = new OwnerUnitViewModel
                {
                    Units = mappedOwnerUnits
                };

                return Ok(new PuzzleApiResponse(result: ownerUnitsData));
            }catch(Exception ex)
            {
                return Ok(new PuzzleApiResponse(message: ex.Message));
            }
        }

        //[HttpPost]
        //public ActionResult Post([FromBody] AddUnitRequestViewModel ownerUnit)
        //{
        //    var mappedRequest = mapper.Map<AddUnitRequestViewModel, OwnerAssignUnitRequest>(ownerUnit);

        //    var operationState = ownerUnitRequestService.AddUnitRequest(mappedRequest);
        //    if (operationState == Common.Enums.OperationState.Created)
        //        return Ok(ownerUnit);

        //    return Ok("Request can't be added");
        //}

        //[HttpDelete]
        //public ActionResult Delete(Guid requestId)
        //{
        //    var operationState = ownerUnitRequestService.DeleteRequest(requestId);
        //    if (operationState == Common.Enums.OperationState.Created)
        //        return Ok();

        //    return Ok("Request can't be deleted");
        //}

        //[HttpPut]
        //public ActionResult Put([FromBody] OwnerUnitUpdateViewModel ownerUnit)
        //{
        //    var result = ownerUnitRequestService.UpdateOwnerUnit(ownerUnit.OldUnitId, ownerUnit.NewUnitId, ownerUnit.OwnerId);
        //    if (result == Common.Enums.OperationState.NotExists)
        //    {
        //        return NotFound(ownerUnit);
        //    }
        //    else if(result == Common.Enums.OperationState.Exists)
        //    {
        //        return Ok(new ResponseStatus
        //        {
        //            Id = ownerUnit.NewUnitId.ToString(),
        //            Message = "This unit is already assigned to this owner!"
        //        });
        //    }
        //    else if (result == Common.Enums.OperationState.Updated)
        //    {
        //        return Ok(result);
        //    }
        //    else
        //    {
        //        return Problem("An error while updating owner unit!");
        //    }
        //}
    }
}
