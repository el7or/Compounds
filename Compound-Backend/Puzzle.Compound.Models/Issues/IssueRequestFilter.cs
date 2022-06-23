using Puzzle.Compound.Common.Models;
using System;

namespace Puzzle.Compound.Models.Issues
{
    public class IssueRequestFilter : PagedInput
    {
        public Guid CompoundId { get; set; }
        public Guid[] IssueTypeIds { get; set; }
        public short? Status { get; set; }
        public string SearchText { get; set; }
    }
}
