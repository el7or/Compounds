using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class OwnerUnit
    {
        public Guid OwnerUnitId { get; set; }
        public Guid CompoundUnitId { get; set; }
        public Guid? CompoundOwnerId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

        public virtual CompoundOwner CompoundOwner { get; set; }
        public virtual CompoundUnit CompoundUnit { get; set; }
    }
}
