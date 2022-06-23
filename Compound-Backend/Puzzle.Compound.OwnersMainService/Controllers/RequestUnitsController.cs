using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Units;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RequestUnitsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IOwnerUnitRequestService ownerUnitRequestService;

        public RequestUnitsController(IMapper mapper, IOwnerUnitRequestService ownerUnitRequestService)
        {
            this.mapper = mapper;
            this.ownerUnitRequestService = ownerUnitRequestService;
        }

        [HttpGet]
        public ActionResult GetByOwnerRegistrationId(Guid ownerRegistrationId, Guid? companyId)
        {
            try
            {
                if (ownerRegistrationId.Equals(Guid.Empty))
                {
                    return Ok(new PuzzleApiResponse(message: "ownerRegistrationId is required!"));
                }

                var ownerUnits = ownerUnitRequestService.GetPendingUnits(ownerRegistrationId, companyId);
                var mappedOwnerUnits = mapper.Map<IEnumerable<UnitRequestInfoMap>, IEnumerable<UnitRequestInfoViewModel>>(ownerUnits);

                var ownerUnitsData = new OwnerUnitViewModel
                {
                    Units = mappedOwnerUnits
                };

                return Ok(new PuzzleApiResponse(result: ownerUnitsData));
            }
            catch(Exception ex)
            {
                return Ok(new PuzzleApiResponse(message: ex.Message));
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] OwnerUnitRequestViewModel ownerUnitRequest)
        {
            try
            {
                var mappedRequests = mapper.Map<IEnumerable<AddUnitRequestViewModel>, IEnumerable<OwnerAssignUnitRequest>>(ownerUnitRequest.Units);

                var operationState = ownerUnitRequestService.AddUnitsRequest(mappedRequests, ownerUnitRequest.OwnerRegistrationId);
                if (operationState == Common.Enums.OperationState.Created)
                {
                    return Ok(new PuzzleApiResponse(result: "Saved successfully"));
                }

                return Ok(new PuzzleApiResponse(message: "Request can't be added"));
            }
            catch(Exception ex)
            {
                return Ok(new PuzzleApiResponse(message: ex.Message));
            }
        }

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
