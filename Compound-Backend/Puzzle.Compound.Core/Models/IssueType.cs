using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Core.Models
{
    public class IssueType
    {
        public IssueType()
        {
            CompoundIssues = new HashSet<CompoundIssue>();
            IssueRequests = new HashSet<IssueRequest>();
            CompanyUserIssues = new HashSet<CompanyUserIssueType>();
        }
        public Guid IssueTypeId { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
        public int Order { get; set; }
        public string Icon { get; set; }
        public bool IsFixed { get; set; }
        public virtual ICollection<CompoundIssue> CompoundIssues { get; set; }
        public virtual ICollection<IssueRequest> IssueRequests { get; set; }
        public virtual ICollection<CompanyUserIssueType> CompanyUserIssues { get; set; }
    }
}
