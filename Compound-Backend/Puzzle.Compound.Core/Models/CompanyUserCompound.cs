using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Core.Models {
	public class CompanyUserCompound {
		public CompanyUserCompound() {
			CompanyUserServices = new HashSet<CompanyUserServiceType>();
			CompanyUserIssues = new HashSet<CompanyUserIssueType>();
			CompanyUserReports = new HashSet<CompanyUserReport>();
		}
		public Guid CompanyUserCompoundId { get; set; }
		public Guid CompanyUserId { get; set; }
		public Guid CompoundId { get; set; }
		public DateTime AssignedDate { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsActive { get; set; }

		public virtual Compound Compound { get; set; }
		public virtual CompanyUser CompanyUser { get; set; }
		public virtual ICollection<CompanyUserServiceType> CompanyUserServices { get; set; }
		public virtual ICollection<CompanyUserIssueType> CompanyUserIssues { get; set; }
		public virtual ICollection<CompanyUserReport> CompanyUserReports { get; set; }
	}
}
