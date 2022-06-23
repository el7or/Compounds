using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Owners;
using Puzzle.Compound.Models.Units;
using Puzzle.Compound.Security;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.OwnersMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OwnerSubUsersController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly IOwnerRegistrationService ownerRegistrationService;
        private readonly IOwnerAssignedUnitService ownerAssignedUnitService;
        private readonly ICompanyService companyService;
        private readonly UserIdentity userIdentity;
        private readonly IOwnerUnitService ownerUnitServ;

        public OwnerSubUsersController(IMapper mapper,
                        IConfiguration configuration,
                        IOwnerRegistrationService ownerRegistrationService,
                        IOwnerAssignedUnitService ownerAssignedUnitService,
                        ICompanyService companyService,
                        UserIdentity userIdentity,
                        IOwnerUnitService ownerUnitServ)
        {
            this.mapper = mapper;
            this.configuration = configuration;
            this.ownerRegistrationService = ownerRegistrationService;
            this.ownerAssignedUnitService = ownerAssignedUnitService;
            this.companyService = companyService;
            this.userIdentity = userIdentity;
            this.ownerUnitServ = ownerUnitServ;
        }

        [HttpGet]
        public ActionResult Get(Guid mainRegistrationId, Guid? companyId)
        {
            try
            {
                if (mainRegistrationId.Equals(Guid.Empty))
                {
                    return Ok(new PuzzleApiResponse(message: "mainRegistrationId is required!"));
                }

                if (companyId.HasValue)
                {
                    if (!companyId.Equals(Guid.Empty))
                    {
                        var companyExists = companyService.IsCompanyExists(companyId.Value);
                        if (!companyExists)
                        {
                            return Ok(new PuzzleApiResponse(message: "Company is not found!"));
                        }
                    }
                }
                var users = ownerRegistrationService.GetUsersByMainRegistrationId(mainRegistrationId);
                var mappedUserInfo = mapper.Map<IEnumerable<OwnerRegistration>, IEnumerable<OwnerRegisterViewModel>>(users);

                return Ok(new PuzzleApiResponse(result: mappedUserInfo));
            }
            catch (Exception ex)
            {
                return Ok(new PuzzleApiResponse(message: ex.Message));
            }
        }

        [HttpGet("{ownerRegisterationId}")]
        public async Task<ActionResult> Get(Guid ownerRegisterationId)
        {
            if (ownerRegisterationId == null || ownerRegisterationId.Equals(Guid.Empty))
                return Ok(new PuzzleApiResponse(message: "ownerRegisterationId is required"));
            var register = await ownerRegistrationService.GetUserById(ownerRegisterationId);
            var registeration = mapper.Map<OwnerRegisterViewModel>(register);
            return Ok(new PuzzleApiResponse(registeration));
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] AddOwnerSubUserViewModel userData)
        {
            try
            {
                var encryptionKey = configuration.GetSection("Security:EncryptionKey").Value;
                var phone = Encryption.DecryptData(userData.Phone, encryptionKey);
                if (phone is null)
                {
                    return Ok(new PuzzleApiResponse(message: "Phone is required!"));
                }
                else
                {
                    userData.Phone = phone;
                }
                // sub user shoud be assigned units
                if (userData.Units == null || !userData.Units.Any())
                {
                    return Ok(new PuzzleApiResponse(message: "units is required!"));
                }

                Guid.TryParse(userData.CompanyId, out Guid companyId);

                if (!companyId.Equals(Guid.Empty))
                {
                    var companyExists = companyService.IsCompanyExists(companyId);
                    if (!companyExists)
                    {
                        return Ok(new PuzzleApiResponse(message: "Company is not found!"));
                    }
                }
                var userInfo = mapper.Map<AddOwnerSubUserViewModel, OwnerRegistration>(userData);

                userInfo.CreatedByRegistrationId = userIdentity.Id;
                byte[] img = null;
                string imgName = null;
                if (userData.Image != null)
                    using (var ms = new MemoryStream())
                    {
                        await userData.Image.CopyToAsync(ms);
                        img = ms.ToArray();
                        imgName = userData.Image.FileName;
                    }
                var userOperationState = await ownerRegistrationService.AddUser(userInfo, img, imgName);
                if (userOperationState == OperationState.Created)
                {
                    if (userData.Units != null && userData.Units.Count() > 0)
                    {
                        // Add units 
                        var mappedUnits = mapper.Map<IEnumerable<SubUserAssignUnitViewModel>, IEnumerable<OwnerAssignedUnit>>(userData.Units);
                        ownerAssignedUnitService.AddUnits(mappedUnits, userInfo.OwnerRegistrationId);
                    }
                    var registrationResult = new OwnerRegisterResultViewModel
                    {
                        OwnerRegistrationId = userInfo.OwnerRegistrationId
                    };
                    return Ok(new PuzzleApiResponse(registrationResult));
                }
                else if (userOperationState == OperationState.Exists)
                {
                    return Ok(new PuzzleApiResponse(message: $"User with phone: {userData.Phone} already exists"));
                }
                else
                {
                    return Ok(new PuzzleApiResponse(message: "Unable to add new user"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new PuzzleApiResponse(message: ex.Message));
            }
        }

        [HttpPut("{registerationId}")]
        public async Task<ActionResult> Put(Guid registerationId, [FromForm] AddOwnerSubUserViewModel userData)
        {
            var encryptionKey = configuration.GetSection("Security:EncryptionKey").Value;
            var phone = Encryption.DecryptData(userData.Phone, encryptionKey);
            if (phone is null)
            {
                return Ok(new PuzzleApiResponse(message: "Phone is required!"));
            }
            else
            {
                userData.Phone = phone;
            }
            if (userData.Units == null || !userData.Units.Any())
            {
                return Ok(new PuzzleApiResponse(message: "units is required!"));
            }
            Guid.TryParse(userData.CompanyId, out Guid companyId);
            if (!companyId.Equals(Guid.Empty))
            {
                var companyExists = companyService.IsCompanyExists(companyId);
                if (!companyExists)
                {
                    return Ok(new PuzzleApiResponse(message: "Company is not found!"));
                }
            }
            var userInfo = await ownerRegistrationService.GetUserById(registerationId);
            userData.Phone = userInfo.Phone;
            mapper.Map(userData, userInfo);
            byte[] img = null;
            string imgName = null;
            if (userData.Image != null)
                using (var ms = new MemoryStream())
                {
                    await userData.Image.CopyToAsync(ms);
                    img = ms.ToArray();
                    imgName = userData.Image.FileName;
                }
            await ownerRegistrationService.EditUser(userInfo, img, imgName);
            if (userData.Units != null && userData.Units.Count() > 0)
            {
                if (userInfo.UserType == OwnerRegistrationType.Owner)
                {
                    if (!userInfo.CompoundOwners.Any())
                        return Ok(new PuzzleApiResponse(message: "Registeration not related to owner"));
                    var mappedUnits = mapper.Map<IEnumerable<SubUserAssignUnitViewModel>, IEnumerable<OwnerUnit>>(userData.Units);
                    ownerUnitServ.UpdateUnits(mappedUnits, userInfo.CompoundOwners.FirstOrDefault().CompoundOwnerId);
                }
                else
                {
                    var mappedUnits = mapper.Map<IEnumerable<SubUserAssignUnitViewModel>, IEnumerable<OwnerAssignedUnit>>(userData.Units);
                    ownerAssignedUnitService.UpdateUnits(mappedUnits, userInfo.OwnerRegistrationId);
                }
            }
            return Ok(new PuzzleApiResponse(new OwnerRegisterResultViewModel
            {
                OwnerRegistrationId = userInfo.OwnerRegistrationId
            }));
        }

        [HttpDelete("{ownerRegisterationId}")]
        public async Task<ActionResult> Delete(Guid ownerRegisterationId)
        {
            if (ownerRegisterationId == null || ownerRegisterationId.Equals(Guid.Empty))
                return Ok(new PuzzleApiResponse(message: "ownerRegisterationId is required"));
            var operationState = await ownerRegistrationService.DeleteSubUser(ownerRegisterationId);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPut("{ownerRegisterationId}/active")]
        public async Task<ActionResult> Activate(Guid ownerRegisterationId)
        {
            if (ownerRegisterationId == null || ownerRegisterationId.Equals(Guid.Empty))
                return Ok(new PuzzleApiResponse(message: "ownerRegisterationId is required"));
            var operationState = await ownerRegistrationService.ActivateSubUser(ownerRegisterationId);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPut("{ownerRegisterationId}/disactive")]
        public async Task<ActionResult> DisActivate(Guid ownerRegisterationId)
        {
            if (ownerRegisterationId == null || ownerRegisterationId.Equals(Guid.Empty))
                return Ok(new PuzzleApiResponse(message: "ownerRegisterationId is required"));
            var operationState = await ownerRegistrationService.DisActivateSubUser(ownerRegisterationId);
            return Ok(new PuzzleApiResponse(operationState));
        }

    }
}