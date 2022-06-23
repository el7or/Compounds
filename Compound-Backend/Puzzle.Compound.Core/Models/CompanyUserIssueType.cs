using System;

namespace Puzzle.Compound.Core.Models
{
    public class CompanyUserIssueType
    {
        public Guid CompanyUserIssueId { get; set; }
        public Guid CompanyUserCompoundId { get; set; }
        public Guid IssueTypeId { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public virtual IssueType IssueType { get; set; }
        public virtual CompanyUserCompound CompanyUserCompound { get; set; }
    }
}
