using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class PlanItem
    {
        public PlanItem()
        {
            PlanDetails = new HashSet<PlanDetail>();
        }

        public Guid PlanItemId { get; set; }
        public string PlanItemNameAr { get; set; }
        public string PlanItemNameEn { get; set; }
        public string PlanItemDetailAr { get; set; }
        public string PlanItemDetailEn { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<PlanDetail> PlanDetails { get; set; }
    }
}
