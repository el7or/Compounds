using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundService
    {
        public CompoundService()
        {
            ServiceSubTypes = new HashSet<ServiceSubType>();
        }
        public Guid CompoundServiceId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid CompoundId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public int Order { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public virtual Compound Compound { get; set; }
        public virtual ICollection<ServiceSubType> ServiceSubTypes { get; set; }
    }
}
