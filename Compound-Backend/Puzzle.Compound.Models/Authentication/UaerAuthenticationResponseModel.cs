using Puzzle.Compound.Models.CompanyRoles;
using Puzzle.Compound.Models.Compounds;
using Puzzle.Compound.Models.SystemPages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Authentication
{
    public class UserAuthenticationResponseModel
    {
        public List<CompoundResponseModel> Compounds { get; set; }
        public List<Guid> RolesIds { get; set; }
        public List<string> UserActions { get; set; }        
    }

    public class CompoundResponseModel
    {
        public Guid CompoundId { get; set; }
        public List<Guid> ServiceTypesIds { get; set; }
        public List<Guid> IssueTypesIds { get; set; }
    }
}
