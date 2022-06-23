using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.CompanyRoles
{
    public class CompanyRoleFilterByTextInputViewModel : PagedInput
    {
        public Guid CompanyId { get; set; }
        public string Text { get; set; }
    }
}
