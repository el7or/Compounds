using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundResidentsService
    {
        public Guid CompoundResidentServiceId { get; set; }
        public Guid CompoundServiceId { get; set; }
        public Guid CompoundResidentId { get; set; }
        public DateTime OrderDatetime { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
    }
}