using System;

namespace Puzzle.Compound.Models.Issues
{
    public class CompoundIssueOutput
    {
        public Guid IssueTypeId { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
    }
}
