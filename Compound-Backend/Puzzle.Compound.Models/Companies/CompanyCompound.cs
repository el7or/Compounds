using Puzzle.Compound.Models.CompoundReports;
using Puzzle.Compound.Models.Issues;
using Puzzle.Compound.Models.Services;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Companies {
	public class CompanyCompound {
		public Guid CompoundId { get; set; }
		public string NameAr { get; set; }
		public string NameEn { get; set; }
		public List<CompoundServiceOutput> Services { get; set; }
		public List<CompoundIssueOutput> Issues { get; set; }
	}
}
