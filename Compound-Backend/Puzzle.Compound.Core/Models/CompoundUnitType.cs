using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundUnitType
    {
        public CompoundUnitType()
        {
            CompoundUnits = new HashSet<CompoundUnit>();
        }

        public int CompoundUnitTypeId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<CompoundUnit> CompoundUnits { get; set; }
    }
}
