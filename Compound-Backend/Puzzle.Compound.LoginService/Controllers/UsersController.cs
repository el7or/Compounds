using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.LoginService.Models;
using Puzzle.Compound.Services;
using System;
using System.Threading.Tasks;

namespace Puzzle.Compound.LoginService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IGateService _gateService;
        private readonly ICompanyUserService companyUserService;

        public UsersController(ITokenService tokenService, IGateService gateService,
            ICompanyUserService companyUserService)
        {
            _tokenService = tokenService;
            _gateService = gateService;
            this.companyUserService = companyUserService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest model)
        {
            var response = _tokenService.Authenticate(model.Username, model.Password, ipAddress());

            if (response == null)
            {
                return Ok(new PuzzleApiResponse(message: "Username or password is incorrect"));
            }

            var accessDetails = companyUserService.GetCompanyUserAccessInformation(response.Id);
            response.AccessDetails = accessDetails;

            setTokenCookie(response.RefreshToken);

            return Ok(new PuzzleApiResponse(result: response));
        }

        [AllowAnonymous]
        [HttpPost("gate-login")]
        public async Task<IActionResult> GateLogin([FromBody] AuthenticateRequest model)
        {
            var response = await _gateService.AuthenticateGate(model.Username, model.Password);
            if (response == null)
            {
                return Ok(new PuzzleApiResponse(message: "Username or password is incorrect"));
            }
            response.Token = _tokenService.Authenticate(response.UserId, null, null, AuthUserType.Gate, ipAddress(), null);
            return Ok(new PuzzleApiResponse(result: response));
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            var refreshToken = refreshTokenRequest.RefreshToken; //Request.Cookies["refreshToken"];
            var response = await _tokenService.RefreshTokenAsync(refreshToken, ipAddress());

            if (response == null)
                return Ok(new PuzzleApiResponse(message: "Invalid token"));

            setTokenCookie(response.RefreshToken);

            return Ok(new PuzzleApiResponse(result: response));
        }

        [HttpPost("revoke-token")]
        public IActionResult RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return Ok(new PuzzleApiResponse(message: "Token is required"));

            var response = _tokenService.RevokeToken(token, ipAddress());

            if (!response)
                return Ok(new PuzzleApiResponse(message: "Token not found"));

            return Ok(new PuzzleApiResponse(message: "Token revoked"));
        }


        //[HttpGet("{id}/refresh-tokens")]
        //public IActionResult GetRefreshTokens(Guid id)
        //{
        //    var user = _tokenService.GetUserById(id);
        //    if (user == null) return NotFound();

        //    return Ok(user.RefreshTokens);
        //}

        // helper methods

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
