using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class RefreshToken
    {
        public Guid RefreshTokenId { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool? IsExpired { get; set; }
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public bool? IsActive { get; set; }
        public Guid UserId { get; set; }
        public int UserType { get; set; }
    }
}
