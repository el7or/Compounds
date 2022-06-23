using Puzzle.Compound.Models.Units;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Groups
{
    public class GroupInfoViewModel
    {
        public Guid CompoundGroupId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public DateTime CreationDate { get; set; }

        public Guid CompoundId { get; set; }
        public int SubGroupsCount { get; set; }
        public int UnitsCount { get; set; }
        public Guid? ParentGroupId { get; set; }
        public List<GroupUnitViewModel> CompoundUnits { get; set; }
    }

    public class GroupUnitViewModel
    {
        public Guid CompoundUnitId { get; set; }
        public string Name { get; set; }
    }
}
