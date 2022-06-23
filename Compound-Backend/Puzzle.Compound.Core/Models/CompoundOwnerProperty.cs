using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundOwnerProperty
    {
        public Guid CompoundOwnerPropertyId { get; set; }
        public string Location { get; set; }
        public Guid CompoundId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

        public virtual Compound Compound { get; set; }
    }
}
