using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundGroup
    {
        public CompoundGroup()
        {
            CompoundUnits = new HashSet<CompoundUnit>();
            Groups = new HashSet<CompoundGroup>();
        }

        public Guid CompoundGroupId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public Guid CompoundId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public Guid? ParentGroupId { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual Compound Compound { get; set; }
        public virtual CompoundGroup Group { get; set; }
        public virtual ICollection<CompoundUnit> CompoundUnits { get; set; }
        public virtual ICollection<CompoundGroup> Groups { get; set; }
    }
}
