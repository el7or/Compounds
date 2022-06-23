using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Amazon;
using Puzzle.Compound.Common;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.CompanyUsers;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CompanyUsersController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICompanyUserService companyUserService;
        private readonly IS3Service s3Service;

        public CompanyUsersController(IMapper mapper, ICompanyUserService companyUserService,
            IS3Service s3Service)
        {
            this.mapper = mapper;
            this.companyUserService = companyUserService;
            this.s3Service = s3Service;
        }

        [HttpGet]
        public ActionResult Get(Guid companyId)
        {
            var users = companyUserService.GetByCompanyId(companyId);
            var mappedCompanyUsers = mapper.Map<IEnumerable<CompanyUser>, IEnumerable<CompanyUserInfoViewModel>>(users);

            return Ok(mappedCompanyUsers);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> GetByIdAsync(Guid userId)
        {
            var user = await companyUserService.GetUserByIdAsync(userId);
            var mappedUser = mapper.Map<CompanyUser, CompanyUserInfoViewModel>(user);
            return Ok(new PuzzleApiResponse(mappedUser));
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CompanyUserViewModel companyUser)
        {
            if (companyUser.UserImage != null && companyUser.UserImage.SizeInBytes > 2097152)
            {
                return Ok(new PuzzleApiResponse(message: "Image should be less than or equal 2 MB!"));
            }
            var mappedCompanyUser = mapper.Map<CompanyUserViewModel, CompanyUser>(companyUser);
            string imageUrl = "";
            if (companyUser.UserImage != null)
            {
                var fileBytes = Convert.FromBase64String(companyUser.UserImage.FileBase64);
                string newFileName = "";
                imageUrl = s3Service.UploadFile("companyUser", companyUser.UserImage.FileName, fileBytes, out newFileName);
                if (!string.IsNullOrEmpty(imageUrl))
                    mappedCompanyUser.Image = newFileName;
            }
            var addResult = await companyUserService.AddUser(mappedCompanyUser, companyUser.Compounds);
            if (addResult == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "Company user exists!"));
            }
            else if (addResult == Common.Enums.OperationState.Created)
            {
                return Ok(new PuzzleApiResponse(result: "Company user saved successfully!"));
            }
            else
            {
                return Ok(new PuzzleApiResponse(message: "Unable to create user!"));
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] CompanyUserViewModel companyUser)
        {
            if (companyUser.UserImage != null && companyUser.UserImage.SizeInBytes > 2097152)
            {
                return Ok(new PuzzleApiResponse(message: "Image should be less than or equal 2 MB!"));
            }
            var mappedCompanyUser = mapper.Map<CompanyUserViewModel, CompanyUser>(companyUser);
            string imageUrl = "";
            if (companyUser.UserImage != null && companyUser.UserImage.FileBase64 != null)
            {
                var fileBytes = Convert.FromBase64String(companyUser.UserImage.FileBase64);
                string newFileName = "";
                imageUrl = s3Service.UploadFile("companyUser", companyUser.UserImage.FileName, fileBytes, out newFileName);
                if (!string.IsNullOrEmpty(imageUrl))
                    mappedCompanyUser.Image = newFileName;
            }
            var operationState = await companyUserService.EditUser(mappedCompanyUser, companyUser.Compounds);
            if (operationState == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "Company user not exists!"));
            }
            else if (operationState == Common.Enums.OperationState.Updated)
            {
                return Ok(new PuzzleApiResponse(result: mappedCompanyUser.CompanyUserId));
            }
            else
            {
                return Ok(new PuzzleApiResponse(message: "Unable to update company user!"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var operationState = await companyUserService.DeleteUser(id);
            if (operationState == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse("Company user not exists!", 200, null));
            }
            else if (operationState == Common.Enums.OperationState.Deleted)
            {
                return Ok(new PuzzleApiResponse(id));
            }
            else
            {
                return Ok(new PuzzleApiResponse("Unable to delete company user!", 200, null));
            }
        }


        [HttpGet("search")]
        public async Task<ActionResult> GetFilteredUsers([FromQuery] CompanyUserFilter filter)
        {
            var users = await companyUserService.FilterCompanyUsers(filter);
            return Ok(new PuzzleApiResponse(users));
        }

        [HttpPut("{userId}/status")]
        public async Task<ActionResult> UpdateStatus(Guid userId, [FromBody] CompanyUserStatusModel model)
        {

            var operationState = await companyUserService.activateUser(userId, model.Status);
            if (operationState == Common.Enums.OperationState.NotExists)
            {
                return Ok(new PuzzleApiResponse(message: "Company user not exists!"));
            }
            else if (operationState == Common.Enums.OperationState.Updated)
            {
                return Ok(new PuzzleApiResponse(result: userId));
            }
            else
            {
                return Ok(new PuzzleApiResponse(message: "Unable to update company user!"));
            }
        }

    }
}
