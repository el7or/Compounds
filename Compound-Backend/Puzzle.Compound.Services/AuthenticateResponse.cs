using Newtonsoft.Json;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Models.Authentication;
using System;

namespace Puzzle.Compound.Services
{
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public AuthUserType UserType { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid[] CompoundsIds { get; set; }
        public string[] UserActions { get; set; }

        public string AccessToken { get; set; }
        public int AccessTokenExpiresIn { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }
        public int RefreshTokenExpiresIn { get; set; }
        public int ServerTimeMs { get; set; }

        public UserAuthenticationResponseModel AccessDetails { get; set; }

        public AuthenticateResponse(Guid userId,
            AuthUserType userType,
            Guid? companyId,
            string accessToken,
            int accessTokenExpiresIn,
            string refreshToken,
            int refreshTokenExpiresIn,
            int serverTimeMs)
        {
            Id = userId;
            UserType = userType;
            CompanyId = companyId;
            AccessToken = accessToken;
            AccessTokenExpiresIn = accessTokenExpiresIn;
            RefreshToken = refreshToken;
            RefreshTokenExpiresIn = refreshTokenExpiresIn;
            ServerTimeMs = serverTimeMs;
        }
    }
}