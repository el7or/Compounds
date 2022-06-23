using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Issues
{
    public class OwnerIssueViewModel
    {
        public string Icon { get; set; }
        public string IssueType { get; set; }
        public bool CanEdit { get; set; }
        public bool CanRate { get; set; }
        public string Note { get; set; }
        public short Rate { get; set; }
        public short PresenterRate { get; set; }
        public string Comment { get; set; }
        public string OwnerComment { get; set; }
        public int Status { get; set; }
        public DateTime PostTime { get; set; }
        public string Record { get; set; }
        public int RequestNumber { get; set; }
        public Location Location { get; set; }
        public List<string> Attachments { get; set; }
        public string CompoundNameArabic { get; set; }
        public string CompoundNameEnglish { get; set; }
    }
}
