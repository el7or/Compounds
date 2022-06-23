using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundInstruction
    {
        public Guid CompoundInstructionId { get; set; }
        public string InstructionContent { get; set; }
        public DateTime AddedDate { get; set; }
        public Guid CompoundId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

        public virtual Compound Compound { get; set; }
    }
}
