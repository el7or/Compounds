using System;

namespace Puzzle.Compound.Models.Issues
{
    public class IssueTypeModel
    {
        public Guid IssueTypeId { get; set; }
        public Guid CompoundId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsFixed { get; set; }
        public string Icon { get; set; }
    }
}
