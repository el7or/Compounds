using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundGate
    {
        public Guid CompoundGateId { get; set; }
        public Guid GateId { get; set; }
        public Guid CompoundId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

        public virtual Compound Compound { get; set; }
        public virtual Gate Gate { get; set; }
    }
}
