using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class OwnerAssignUnitRequest
    {
        public Guid OwnerAssignUnitRequestId { get; set; }
        public DateTime RequestDate { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }
        public Guid OwnerRegistrationId { get; set; }
        public Guid CompoundUnitId { get; set; }

        public virtual CompoundUnit CompoundUnit { get; set; }
        public virtual OwnerRegistration OwnerRegistration { get; set; }
    }
}
