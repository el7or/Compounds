using System;

namespace Puzzle.Compound.Models.Issues
{
    public class IssueRequestList
    {
        public Guid IssueRequestId { get; set; }
        public int RequestNumber { get; set; }
        public string RequestedBy { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string IssueTypeArabic { get; set; }
        public string IssueTypeEnglish { get; set; }
        public short Status { get; set; }
        public DateTime PostTime { get; set; }
        public short Rate { get; set; }
        public Location Location { get; set; }
        public string Icon { get; set; }
        public Guid IssueTypeId { get; set; }
        public string CompoundNameEnglish { get; set; }
        public string CompoundNameArabic { get; set; }
    }
}
