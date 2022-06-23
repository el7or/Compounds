using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Puzzle.Compound.Amazon;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Hubs;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Owners;
using Puzzle.Compound.Models.Owners.Filters;
using Puzzle.Compound.Models.Units;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompoundOwnersController : ControllerBase
    {
        private readonly ICompoundOwnerService compoundOwnerService;
        private readonly IOwnerUnitService ownerUnitService;
        private readonly IOwnerRegistrationService ownerRegistrationService;
        private readonly IS3Service s3Service;
        private readonly IMapper mapper;
        private readonly IHubContext<CounterHub> _hub;

        public CompoundOwnersController(ICompoundOwnerService compoundOwnerService,
                IOwnerUnitService ownerUnitService,
                IMapper mapper, IOwnerRegistrationService ownerRegistrationService,
                IS3Service s3Service, IHubContext<CounterHub> hub)
        {
            this.compoundOwnerService = compoundOwnerService;
            this.ownerUnitService = ownerUnitService;
            this.ownerRegistrationService = ownerRegistrationService;
            this.s3Service = s3Service;
            this.mapper = mapper;
            _hub = hub;
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredOwners([FromQuery] OwnerFilterViewModel ownerFilter)
        {
            var compoundOwnersList = await compoundOwnerService.GetFilteredOwnersAsync(ownerFilter);
            return Ok(new PuzzleApiResponse(compoundOwnersList));
        }

        [HttpGet]
        public ActionResult Get([FromQuery] OwnerFilterViewModel ownerFilter)
        {
            var owners = compoundOwnerService.GetCompoundOwners(ownerFilter);
            var mappedOwners = mapper.Map<IEnumerable<CompoundOwner>, IEnumerable<OwnerInfoViewModel>>(owners);
            return Ok(new PuzzleApiResponse(result: mappedOwners));
        }

        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            var owner = compoundOwnerService.GetCompoundOwnerByIdWithSubOwnersCount(id);
            var ownerUnits = ownerUnitService.GetOwnerUnits(owner.CompoundOwnerId.Value);
            var mappedOwnerUnits = mapper.Map<IEnumerable<CompoundUnit>, IEnumerable<UnitInfoViewModel>>(ownerUnits);
            owner.Units = mappedOwnerUnits.ToList();
            if (owner.SubOwnersCount > 0)
            {
                var users = ownerRegistrationService.GetUsersByMainRegistrationId(owner.OwnerRegistrationId.Value);
                var mappedSubUsers = mapper.Map<IEnumerable<OwnerRegistration>, IEnumerable<OwnerRegisterViewModel>>(users);
                owner.SubOwners = mappedSubUsers.ToList();
            }
            return Ok(new PuzzleApiResponse(result: owner));
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] OwnerInputViewModel compoundOwner)
        {
            if (compoundOwner.Image != null && compoundOwner.Image.SizeInBytes > 2097152)
            {
                return Ok(new PuzzleApiResponse(message: "File should be less than or equal 2 MB!"));
            }


            var mappedOwner = mapper.Map<OwnerInputViewModel, CompoundOwner>(compoundOwner);
            var operationState = compoundOwnerService.AddCompoundOwner(mappedOwner);

            if (operationState == Common.Enums.OperationState.Created)
            {
                string logoUrl = "";

                if (compoundOwner.Image != null)
                {
                    // Upload owner image
                    var fileBytes = Convert.FromBase64String(compoundOwner.Image.FileBase64);
                    string newFileName = "";

                    logoUrl = s3Service.UploadFile("owner", compoundOwner.Image.FileName, fileBytes, out newFileName);

                    if (!string.IsNullOrEmpty(logoUrl))
                    {
                        mappedOwner.Image = newFileName;

                        var logoUpdateOperation = compoundOwnerService.EditCompoundOwner(mappedOwner);
                    }
                }

                operationState = ownerUnitService.AddOwnerUnits(mappedOwner.CompoundOwnerId, compoundOwner.UnitsIds);

                if (operationState == Common.Enums.OperationState.Created)
                {
                    return Ok(new PuzzleApiResponse(result: new
                    {
                        mappedOwner.CompoundOwnerId,
                        imageUrl = logoUrl
                    }));
                }
                await _hub.Clients.All.SendAsync("UpdatePendingListCount", true);
            }
            return Ok(new PuzzleApiResponse(message: "Owner can't be added!"));
        }

        [HttpPut]
        public async Task<ActionResult> PutAsync(OwnerInputViewModel compoundOwner)
        {
            if (compoundOwner.Image != null && compoundOwner.Image.SizeInBytes > 2097152)
            {
                return Ok(new PuzzleApiResponse(message: "File should be less than or equal 2 MB!"));
            }

            var mappedOwner = mapper.Map<OwnerInputViewModel, CompoundOwner>(compoundOwner);

            string logoUrl = "";

            if (compoundOwner.Image != null && string.IsNullOrEmpty(compoundOwner.Image.Path))
            {
                // Upload owner image
                var fileBytes = Convert.FromBase64String(compoundOwner.Image.FileBase64);
                string newFileName = "";

                logoUrl = s3Service.UploadFile("owner", compoundOwner.Image.FileName, fileBytes, out newFileName);
                if (!string.IsNullOrEmpty(logoUrl))
                {
                    mappedOwner.Image = newFileName;
                }
            }
            //else
            //{
            //    logoUrl = compoundOwner.Image.Path;
            //}

            var operationState = compoundOwnerService.EditCompoundOwner(mappedOwner);

            if (operationState == Common.Enums.OperationState.Updated)
            {
                operationState = ownerUnitService.DeleteOwnerUnits(mappedOwner.CompoundOwnerId);
                if (operationState == Common.Enums.OperationState.None)
                {
                    operationState = ownerUnitService.AddOwnerUnits(mappedOwner.CompoundOwnerId, compoundOwner.UnitsIds);

                    if (operationState == Common.Enums.OperationState.Created)
                    {
                        return Ok(new PuzzleApiResponse(result: new
                        {
                            compoundOwner.CompoundOwnerId,
                        }));
                    }
                }
                await _hub.Clients.All.SendAsync("UpdatePendingListCount", true);
            }
            return Ok(new PuzzleApiResponse(message: "Owner can't be updated!"));
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var result = compoundOwnerService.DeleteCompoundOwner(id);
            if (result == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "Owner is not exists!"));
            }
            else if (result == Common.Enums.OperationState.Deleted)
            {
                ownerUnitService.DeleteOwnerUnits(id);

                return Ok(new PuzzleApiResponse(result: "Owner deleted successfully"));
            }

            return Ok(new PuzzleApiResponse(message: "Owner can't be deleted"));
        }
    }
}
