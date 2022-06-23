using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundIssue
    {
        public Guid CompoundIssueId { get; set; }
        public Guid IssueTypeId { get; set; }
        public Guid CompoundId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public int Order { get; set; }
        public virtual IssueType IssueType { get; set; }
        public virtual Compound Compound { get; set; }
    }
}
