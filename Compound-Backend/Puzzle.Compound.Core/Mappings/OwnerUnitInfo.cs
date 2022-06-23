using System;

namespace Puzzle.Compound.Core.Mappings
{
    public class MainOwnerUnitInfo : OwnerUnitInfo
    {
        public Guid CompoundOwnerId { get; set; }
        public int SubOwners { get; set; }
    }

    public class OwnerUnitInfo
    {
        public Guid? OwnerRegistrationId { get; set; }
        public Guid CompoundUnitId { get; set; }
        public string Name { get; set; }
    }
}
