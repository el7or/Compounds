using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models {
	public partial class Plan {
		public Plan() {
			PlanDetails = new HashSet<PlanDetail>();
			Companies = new HashSet<Company>();
		}

		public Guid PlanId { get; set; }
		public string PlanNameAr { get; set; }
		public string PlanNameEn { get; set; }
		public bool? IsDeleted { get; set; }
		public bool? IsActive { get; set; }

		public virtual ICollection<Company> Companies { get; set; }
		public virtual ICollection<PlanDetail> PlanDetails { get; set; }
	}
}
