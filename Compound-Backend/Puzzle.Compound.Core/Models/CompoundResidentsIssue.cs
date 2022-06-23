using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundResidentsIssue
    {
        public Guid CompoundResidentIssueId { get; set; }
        public Guid CompoundIssueId { get; set; }
        public Guid CompoundResidentId { get; set; }
        public DateTime OrderDatetime { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
    }
}