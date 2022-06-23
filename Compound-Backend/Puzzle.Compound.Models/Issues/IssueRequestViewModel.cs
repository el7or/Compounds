using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Issues
{
    public class IssueRequestViewModel
    {
        public string Comment { get; set; }
        public Guid IssueRequestId { get; set; }
        public DateTime PostTime { get; set; }
        public string OwnerComment { get; set; }
        public int RequestNumber { get; set; }
        public short Status { get; set; }
        public string Note { get; set; }
        public short Rate { get; set; }
        public string IssueTypeArabic { get; set; }
        public string IssueTypeEnglish { get; set; }
        public string CompoundNameArabic { get; set; }
        public string CompoundNameEnglish { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhone { get; set; }
        public short PresenterRate { get; set; }
        public string Icon { get; set; }
        public string Record { get; set; }
        public Location Location { get; set; }
        public List<string> Attachments { get; set; }
    }
}
