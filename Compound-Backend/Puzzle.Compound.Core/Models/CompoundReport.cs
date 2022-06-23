using System;

namespace Puzzle.Compound.Core.Models {
	public class CompoundReport {
		public Guid CompoundReportId { get; set; }
		public Guid ReportTypeId { get; set; }
		public Guid CompoundId { get; set; }
		public bool? IsDeleted { get; set; }
		public bool? IsActive { get; set; }
		public int Order { get; set; }
		public virtual ReportType ReportType { get; set; }
		public virtual Compound Compound { get; set; }
	}
}
