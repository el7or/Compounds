using System;

namespace Puzzle.Compound.Core.Models {
	public class CompanyUserReport {
    public Guid CompanyUserReportId { get; set; }
    public Guid CompanyUserCompoundId { get; set; }
    public Guid ReportTypeId { get; set; }
    public DateTime AssignedDate { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }

    public virtual ReportType ReportType { get; set; }
    public virtual CompanyUserCompound CompanyUserCompound { get; set; }
  }
}
