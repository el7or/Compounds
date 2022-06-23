using System;

namespace Puzzle.Compound.Models.Groups
{
    public class AddEditCompoundGroupViewModel
    {
        public Guid? CompoundGroupId { get; set; }

        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public Guid CompoundId { get; set; }
        public Guid? ParentGroupId { get; set; }
    }
}
