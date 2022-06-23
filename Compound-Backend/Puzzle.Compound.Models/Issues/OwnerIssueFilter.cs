using Puzzle.Compound.Common.Models;
using System;

namespace Puzzle.Compound.Models.Issues
{
    public class OwnerIssueFilter : PagedInput
    {
        public Guid OwnerRegistrationId { get; set; }
        public bool IsUpcoming { get; set; }
        public Guid[] IssueTypes { get; set; }
    }
}
