using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class OwnerAssignedUnit
    {
        public Guid OwnerAssignedUnitId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? StartFrom { get; set; }
        public DateTime? EndTo { get; set; }
        public Guid OwnerRegistrationId { get; set; }
        public Guid CompoundUnitId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual CompoundUnit CompoundUnit { get; set; }
        public virtual OwnerRegistration OwnerRegistration { get; set; }
    }
}
