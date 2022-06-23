using Puzzle.Compound.Common.Models;
using System;

namespace Puzzle.Compound.Models.CompanyRoleActions
{
    public class CompanyRoleActionFilterByTextInputViewModel : PagedInput
    {
        public Guid CompanyRoleId { get; set; }
        public Guid SystemPageActionId { get; set; }
        public string Text { get; set; }
    }
}
