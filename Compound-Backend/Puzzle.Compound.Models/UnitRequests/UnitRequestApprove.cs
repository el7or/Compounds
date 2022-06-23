using System;

namespace Puzzle.Compound.Models.UnitRequests
{
    public class UnitRequestApprove
    {
        public Guid CompoundOwnerId { get; set; }
        public Guid OwnerRegistrationId { get; set; }
    }
}
