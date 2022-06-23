using System;

namespace Puzzle.Compound.Models.Issues
{
    public class CompoundIssueModel
    {
        public Guid IssueTypeId { get; set; }
        public string IssueNameArabic { get; set; }
        public string IssueNameEnglish { get; set; }
        public bool Selected { get; set; }
        public int IssueOrder { get; set; }
        public int AssignOrder { get; set; }
        public bool IsFixed { get; set; }
    }
}
