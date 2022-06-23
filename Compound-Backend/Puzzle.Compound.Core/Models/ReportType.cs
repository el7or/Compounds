using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Core.Models {
	public class ReportType {
		public ReportType() {
			CompoundReports = new HashSet<CompoundReport>();
			CompanyUserReports = new HashSet<CompanyUserReport>();
		}
		public Guid ReportTypeId { get; set; }
		public string ArabicName { get; set; }
		public string EnglishName { get; set; }
		public int Order { get; set; }
		public string Icon { get; set; }
		public bool IsFixed { get; set; }
		public virtual ICollection<CompoundReport> CompoundReports { get; set; }
		public virtual ICollection<CompanyUserReport> CompanyUserReports { get; set; }
	}
}
