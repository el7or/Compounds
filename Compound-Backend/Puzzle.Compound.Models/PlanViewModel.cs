using System;

namespace Puzzle.Compound.Models
{
    public class PlanViewModel
    {
        public Guid PlanId { get; set; }
        public string PlanNameAr { get; set; }
        public string PlanNameEn { get; set; }
        public bool? IsActive { get; set; }
    }
}
