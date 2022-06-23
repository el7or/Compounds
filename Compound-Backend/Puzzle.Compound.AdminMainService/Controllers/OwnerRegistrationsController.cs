using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Puzzle.Compound.Common;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.OwnerRegistrations;
using Puzzle.Compound.Models.OwnerRegistrations.Filters;
using Puzzle.Compound.Models.Owners;
using Puzzle.Compound.Models.Units;
using Puzzle.Compound.Services;
using Puzzle.Compound.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OwnerRegistrationsController : ControllerBase
    {
        private readonly IOwnerRegistrationService ownerRegistrationService;
        private readonly IOwnerUnitRequestService ownerUnitRequestService;
        private readonly ICompoundOwnerService compoundOwnerService;
        private readonly IOwnerUnitService ownerUnitService;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        public OwnerRegistrationsController(IOwnerRegistrationService ownerRegistrationService,
            IOwnerUnitRequestService ownerUnitRequestService,
            ICompoundOwnerService compoundOwnerService,
            IOwnerUnitService ownerUnitService,
            IConfiguration configuration,
            IMapper mapper)
        {
            this.ownerRegistrationService = ownerRegistrationService;
            this.ownerUnitRequestService = ownerUnitRequestService;
            this.compoundOwnerService = compoundOwnerService;
            this.ownerUnitService = ownerUnitService;
            this.configuration = configuration;
            this.mapper = mapper;
        }


        [HttpGet("owners-with-filter")]
        public async Task<ActionResult> Get([FromQuery] OwnerRegistrationFilterViewModel ownerRegistrationFilter)
        {
            var ownerRegistrations = await ownerRegistrationService.GetOwnerRegistrationsAsync(ownerRegistrationFilter);

            return Ok(new PuzzleApiResponse(result: ownerRegistrations));
        }

        [HttpGet("owner")]
        public ActionResult GetOwnerInfo(string phone, Guid ownerRegistrationId, Guid? companyId)
        {
            phone = phone.Replace(" ", "");

            var ownerApprovalInfo = new OwnerApprovalInfoViewModel();

            var ownerRegistrationInfo = ownerRegistrationService.GetByPhone(phone);
            if (ownerRegistrationInfo != null)
            {
                var mappedRegistrationInfo = mapper.Map<OwnerRegistration, OwnerInfoViewModel>(ownerRegistrationInfo);
                var ownerRequests = ownerUnitRequestService.GetPendingUnits(ownerRegistrationInfo.OwnerRegistrationId, companyId);

                ownerApprovalInfo.OwnerRegistration = new OwnerRegistrationApproveViewModel
                {
                    Info = mappedRegistrationInfo,
                    Units = ownerRequests.ToList()
                };
            }

            var ownerInfo = compoundOwnerService.GetOwner(phone, ownerRegistrationInfo.OwnerRegistrationId);

            if (ownerInfo != null)
            {
                var mappedOwnerInfo = mapper.Map<CompoundOwner, OwnerInfoViewModel>(ownerInfo);
                var ownerUnits = ownerUnitService.GetOwnerUnits(ownerInfo.CompoundOwnerId);
                var mappedOwnerUnits = mapper.Map<IEnumerable<CompoundUnit>, IEnumerable<UnitInfoViewModel>>(ownerUnits);

                ownerApprovalInfo.Owner = new OwnerApproveViewModel
                {
                    Info = mappedOwnerInfo,
                    Units = mappedOwnerUnits.ToList()
                };
            }
            return Ok(new PuzzleApiResponse(result: ownerApprovalInfo));
        }
    }
}
