using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundNearbyPlace
    {
        public Guid CompoundPlaceId { get; set; }
        public string PlaceName { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }
        public string Type { get; set; }
        public Guid CompoundId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

        public virtual Compound Compound { get; set; }
    }
}
