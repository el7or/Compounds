using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Companies;
using Puzzle.Compound.Services;
using System;
using System.Threading.Tasks;

namespace Puzzle.Compound.WebMainService.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class CompaniesController : ControllerBase {
		private readonly IMapper mapper;
		private readonly ICompanyService companyService;

		public CompaniesController(IMapper mapper, ICompanyService companyService) {
			this.mapper = mapper;
			this.companyService = companyService;
		}

		[HttpPost]
		public async Task<ActionResult> Post([FromBody] AddCompanyViewModel registerData) {
			try {
				var companyInfo = mapper.Map<AddCompanyViewModel, Company>(registerData);

				var operationState = await companyService.AddCompanyWithUser(companyInfo, registerData.Email, registerData.Password);
				if (operationState == Common.Enums.OperationState.Created) {
					return Ok(registerData);
				} else if (operationState == Common.Enums.OperationState.Exists) {
					return Ok("Company user is already exists!");
				} else {
					return Problem("Unable to create company!", "", 500);
				}
			} catch (Exception ex) {
				return Problem(ex.Message, "", 500);
			}
		}
	}
}
