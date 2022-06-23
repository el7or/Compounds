using System;

namespace Puzzle.Compound.Models.Units
{
    public class AddEditUnitViewModel
    {
        public Guid? CompoundUnitId { get; set; }
        public string Name { get; set; }
        public Guid CompoundGroupId { get; set; }
        public int CompoundUnitTypeId { get; set; }
    }
}
