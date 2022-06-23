using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Owners;
using Puzzle.Compound.Models.Units;
using Puzzle.Compound.Security;
using Puzzle.Compound.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.OwnersMainService.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class RegistrationsController : ControllerBase {
		private readonly IMapper mapper;
		private readonly IConfiguration configuration;
		private readonly IOwnerRegistrationService ownerRegistrationService;
		private readonly ICompoundOwnerService compoundOwnerService;
		private readonly IOwnerUnitService compoundGroupUnitService;
		private readonly IOwnerUnitRequestService ownerUnitRequestService;
		private readonly ICompanyService companyService;
		private readonly ITokenService tokenService;
		private readonly IOwnerAssignedUnitService ownerAssignedUnitService;

		public RegistrationsController(IMapper mapper,
				IConfiguration configuration,
				IOwnerRegistrationService ownerRegistrationService,
				ICompoundOwnerService compoundOwnerService,
				IOwnerUnitService compoundGroupUnitService,
				IOwnerUnitRequestService ownerUnitRequestService,
				ICompanyService companyService,
				ITokenService tokenService,
				IOwnerAssignedUnitService ownerAssignedUnitService) {
			this.mapper = mapper;
			this.configuration = configuration;
			this.ownerRegistrationService = ownerRegistrationService;
			this.compoundOwnerService = compoundOwnerService;
			this.compoundGroupUnitService = compoundGroupUnitService;
			this.ownerUnitRequestService = ownerUnitRequestService;
			this.companyService = companyService;
			this.tokenService = tokenService;
			this.ownerAssignedUnitService = ownerAssignedUnitService;
		}


		[HttpPost]
		[Route("login")]
		public ActionResult UserLogin([FromHeader] string Language, [FromBody] OwnerLoginViewModel loginData) {
			try {
				loginData.Phone = loginData.Phone.Replace(" ", "");
				var encryptionKey = configuration.GetSection("Security:EncryptionKey").Value;
				var phone = Encryption.DecryptData(loginData.Phone, encryptionKey);
				if (phone is null) {
					return Ok(new PuzzleApiResponse(message: "Phone is required!"));
				}
				Guid.TryParse(loginData.CompanyId, out Guid parsedCompanyId);
				Guid? companyId = null;
				if (parsedCompanyId != Guid.Empty)
					companyId = parsedCompanyId;

				var loginResult = new OwnerLoginResultViewModel();

				var userInfo = ownerRegistrationService.GetByPhone(phone);
				if (userInfo != null) {
					loginResult.OwnerRegistrationId = userInfo.OwnerRegistrationId;
					loginResult.UserType = userInfo.UserType;
					loginResult.UserInfo = mapper.Map<LoginUserInfo>(userInfo);
					if (userInfo.IsBlocked.HasValue && userInfo.IsBlocked.Value) {
						return Ok(new PuzzleApiResponse(message: "Phone is blocked!"));
					}

					if (userInfo.IsDeleted)
						return Ok(new PuzzleApiResponse(message: "your account is deleted!"));

					if (!userInfo.IsActive)
						return Ok(new PuzzleApiResponse(message: "your account is unactive!"));

					if (userInfo.UserType != OwnerRegistrationType.Owner) {
						// Get Tenant & Sub user units
						var ownerUnits = ownerAssignedUnitService.GetUnits(userInfo.OwnerRegistrationId, companyId, Language);
						var mappedOwnerUnits = mapper.Map<IEnumerable<UnitInfoMap>, IEnumerable<UnitInfoViewModel>>(ownerUnits);
						loginResult.Units = mappedOwnerUnits.ToList();
						loginResult.Compounds = ownerAssignedUnitService.GetAssignedCompounds(userInfo.OwnerRegistrationId, companyId, Language).ToList();
						loginResult.Status = OwnerStatus.GoToDashboard;
					} else {
						var ownerUnits = compoundGroupUnitService.GetUnitsByRegistrationId(userInfo.OwnerRegistrationId, companyId, Language);
						var mappedOwnerUnits = mapper.Map<IEnumerable<UnitInfoMap>, List<UnitInfoViewModel>>(ownerUnits);
						loginResult.Units = mappedOwnerUnits;
						loginResult.Compounds = compoundGroupUnitService.GetUnitsOwnerCompounds(userInfo.OwnerRegistrationId, companyId, Language).ToList();
						if (mappedOwnerUnits.Count == 0) {
							var ownerRequests = ownerUnitRequestService.GetPendingUnits(userInfo.OwnerRegistrationId, companyId);
							loginResult.Status = (ownerRequests.Count() > 0) ? OwnerStatus.WaitingForPreview : OwnerStatus.GoToAddUnits;
						} else {
							var userConfirmed = userInfo.UserConfirmed.HasValue && userInfo.UserConfirmed.Value;

							loginResult.Status = userConfirmed ? OwnerStatus.GoToDashboard : OwnerStatus.GoToAddUnits;
						}

						if (loginResult.Status == OwnerStatus.GoToAddUnits || loginResult.Status == OwnerStatus.WaitingForPreview) {
							loginResult.OwnerRegistrationId = userInfo.OwnerRegistrationId;
						}

						// Return the company settings 
						loginResult.CompanySetting = new {
							Emergency = true,
							Visits = true,
							Services = new Dictionary<string, string>
								{
																		{"Service 1","" },
																		{"Service 2","" },
																		{"Service 3","" }
														}
						};
					}

					// Generate token
					var token = tokenService.Authenticate(userInfo.OwnerRegistrationId, companyId, null, AuthUserType.Owner, ipAddress(), null);
					loginResult.Token = token;
					return Ok(new PuzzleApiResponse(loginResult));
				} else {
					loginResult.Status = OwnerStatus.GoToRegistration;

					// Get existing owners 
					var ownersList = compoundOwnerService.GetCompoundOwnersByPhone(phone, companyId);
					if (ownersList.Count() > 0) {
						IEnumerable<UnitInfoMap> ownerUnits = null;

						if (ownersList.Count() == 1) {
							var owner = ownersList.FirstOrDefault();
							ownerUnits = compoundGroupUnitService.GetUnitsByOwnerId(owner.CompoundOwnerId, companyId);
							var mappedOwnerUnits = mapper.Map<IEnumerable<UnitInfoMap>, List<UnitInfoViewModel>>(ownerUnits);

							loginResult.Units = mappedOwnerUnits;
							loginResult.CompanySetting = new {
								Emergency = true,
								Visits = true,
								Services = new Dictionary<string, string>
									{
																		{"Service 1","" },
																		{"Service 2","" },
																		{"Service 3","" }
																}
							};
						} else {
							ownerUnits = compoundGroupUnitService.GetUnitsByPhone(phone, companyId, Language);
							var mappedOwnerUnits = mapper.Map<IEnumerable<UnitInfoMap>, List<UnitInfoViewModel>>(ownerUnits);
							loginResult.Units = mappedOwnerUnits;
						}
					}
				}

				loginResult.Status = OwnerStatus.GoToRegistration;

				return Ok(new PuzzleApiResponse(loginResult));
			} catch (Exception ex) {
				return Ok(new PuzzleApiResponse(ex.Message, 500));
			}
		}

		[HttpPost]
		[Route("register")]
		public async Task<ActionResult> Register([FromBody] OwnerRegisterViewModel registerData) {
			try {
				registerData.Phone = registerData.Phone.Replace(" ", "");
				var encryptionKey = configuration.GetSection("Security:EncryptionKey").Value;
				var phone = Encryption.DecryptData(registerData.Phone, encryptionKey);
				if (phone is null) {
					return Ok(new PuzzleApiResponse(message: "Phone is required!"));
				} else {
					registerData.Phone = phone;
				}
				Guid.TryParse(registerData.CompanyId, out Guid parsedCompanyId);

				if (!parsedCompanyId.Equals(Guid.Empty)) {
					var companyExists = companyService.IsCompanyExists(parsedCompanyId);
					if (!companyExists) {
						return Ok(new PuzzleApiResponse(message: "Company is not found!"));
					}
				}
				Guid? companyId = null;
				if (parsedCompanyId != Guid.Empty)
					companyId = parsedCompanyId;

				var userInfo = mapper.Map<OwnerRegisterViewModel, OwnerRegistration>(registerData);
				var userOperationState = await ownerRegistrationService.AddUser(userInfo);
				if (userOperationState == OperationState.Created) {
					var existingOwners = compoundOwnerService.GetCompoundOwnersByPhone(registerData.Phone, companyId);

					if (existingOwners.Count() > 0) {
						// Assign created user to the owner
						var assignState = compoundOwnerService.AssignRegistrationId(userInfo.OwnerRegistrationId, existingOwners);
					}

					// Generate token
					var token = tokenService.Authenticate(userInfo.OwnerRegistrationId, companyId, null, AuthUserType.Owner, ipAddress(), null);
					var registrationResult = new OwnerRegisterResultViewModel {
						OwnerRegistrationId = userInfo.OwnerRegistrationId,
						Token = token
					};
					return Ok(new PuzzleApiResponse(registrationResult));
				} else if (userOperationState == OperationState.Exists) {
					return Ok(new PuzzleApiResponse(message: $"User with phone: {registerData.Phone} already exists"));
				} else {
					return Ok("Unable to register the owner");
				}
			} catch (Exception ex) {
				return Ok(new PuzzleApiResponse(ex.Message, 500));
			}
		}

		private void setTokenCookie(string token) {
			var cookieOptions = new CookieOptions {
				HttpOnly = true,
				Expires = DateTime.UtcNow.AddDays(7)
			};
			Response.Cookies.Append("refreshToken", token, cookieOptions);
		}

		private string ipAddress() {
			if (Request.Headers.ContainsKey("X-Forwarded-For"))
				return Request.Headers["X-Forwarded-For"];
			else
				return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
		}

	}
}
