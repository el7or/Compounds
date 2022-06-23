using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class PlanDetail
    {
        public Guid PlanDetailId { get; set; }
        public Guid PlanId { get; set; }
        public Guid PlanItemId { get; set; }
        public int ItemCount { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

        public virtual Plan Plan { get; set; }
        public virtual PlanItem PlanItem { get; set; }
    }
}
