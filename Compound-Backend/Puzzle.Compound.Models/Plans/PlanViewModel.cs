using System;
using System.ComponentModel.DataAnnotations;

namespace Puzzle.Compound.Models.Plans
{
    public class AddPlanViewModel
    {
        [Required]
        public string PlanNameAr { get; set; }

        [Required]
        public string PlanNameEn { get; set; }
    }

    public class PlanInfoViewModel
    {
        public Guid PlanId { get; set; }
        public string PlanNameAr { get; set; }
        public string PlanNameEn { get; set; }
    }
}
