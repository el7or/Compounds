using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundStore
    {
        public Guid CompoundStoreId { get; set; }
        public string StoreName { get; set; }
        public string OwnerName { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public Guid CompoundId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

        public virtual Compound Compound { get; set; }
    }
}
