using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.CompanyUserRoles
{
    public class CompanyUserRoleFilterByTextInputViewModel : PagedInput
    {
        public Guid CompanyUserId { get; set; }
        public Guid CompanyRoleId { get; set; }
        public Guid CompoundId { get; set; }
        public string Text { get; set; }
    }
}
