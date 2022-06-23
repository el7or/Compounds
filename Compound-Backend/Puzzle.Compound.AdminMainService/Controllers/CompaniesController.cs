using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Amazon;
using Puzzle.Compound.Common;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Companies;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompaniesController : PuzzleBaseController
    {
        private readonly ICompanyService companyService;
        private readonly IS3Service s3Service;

        public CompaniesController(IMapper mapper, ICompanyService companyService,
                IS3Service s3Service) :
                base(mapper)
        {
            this.companyService = companyService;
            this.s3Service = s3Service;
        }

        [HttpGet]
        public ActionResult Get([FromQuery] string name, [FromQuery] Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(name) && id == null)
            {
                var source = companyService.GetCompanies();
                var companiesList = Mapper.Map<IEnumerable<Company>, IEnumerable<CompanyInfoViewModel>>(source);

                return Ok(new PuzzleApiResponse(companiesList));
            }
            else
            {
                if (id != null)
                {
                    var company = companyService.GetCompanyById(id.Value);
                    var mappedCompany = Mapper.Map<Company, CompanyInfoViewModel>(company);
                    return Ok(new PuzzleApiResponse(mappedCompany));
                }
                else
                {
                    var companies = companyService.GetCompaniesByName(name);

                    if (HttpContext.Request.Headers.ContainsKey("Platform") && HttpContext.Request.Headers["Platform"] == "web")
                    {
                        var companiesList = Mapper.Map<IEnumerable<Company>, IEnumerable<CompanyInfoViewModel>>(companies);
                        return Ok(new PuzzleApiResponse(companiesList));
                    }
                    else
                    {
                        var companiesList = Mapper.Map<IEnumerable<Company>, IEnumerable<CompanyInfo2ViewModel>>(companies);
                        return Ok(new PuzzleApiResponse(companiesList));
                    }
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Post([FromBody] AddCompanyViewModel registerData)
        {
            if (registerData.Logo != null && registerData.Logo.SizeInBytes > 2097152)
            {
                return Ok(new PuzzleApiResponse(message: "File should be less than or equal 2 MB!"));
            }

            var companyInfo = Mapper.Map<AddCompanyViewModel, Company>(registerData);
            var operationState = await companyService.AddCompanyWithUser(companyInfo, registerData.Email, registerData.Password);
            if (operationState == Common.Enums.OperationState.Created)
            {
                string logoUrl = "";

                if (registerData.Logo != null)
                {
                    // Upload Company Logo
                    var fileBytes = Convert.FromBase64String(registerData.Logo.FileBase64);
                    string newFileName = "";

                    logoUrl = s3Service.UploadFile("company", registerData.Logo.FileName, fileBytes, out newFileName);

                    if (!string.IsNullOrEmpty(logoUrl))
                    {
                        companyInfo.Logo = newFileName;

                        var logoUpdateOperation = companyService.UpdateCompany(companyInfo);
                    }
                }

                return Ok(new PuzzleApiResponse(result: new
                {
                    logo = logoUrl,
                    companyId = companyInfo.CompanyId
                }));
            }
            else if (operationState == Common.Enums.OperationState.Exists)
            {
                return Ok(new PuzzleApiResponse(message: "Company user is already exists!"));
            }
            else
            {
                return Ok(new PuzzleApiResponse(message: "Unable to create company!"));
            }
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet("compounds/{companyId}")]
        public async Task<ActionResult> GetCompanyCompounds(Guid companyId)
        {
            var compounds = await companyService.GetCompanyCompounds(companyId);
            return Ok(new PuzzleApiResponse(compounds));
        }
    }
}
