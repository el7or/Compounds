using System;

namespace Puzzle.Compound.Core.Mappings
{
    public class UnitRequestInfoMap : MainUnitInfo
    {
        public DateTime RequestDate { get; set; }
        public bool? UserConfirmed { get; set; }
        public int Status { get; set; }
        public Guid OwnerRegistrationId { get; set; }
    }
}
