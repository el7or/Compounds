using System;

namespace Puzzle.Compound.Models.Compounds {
	public class AddCompoundViewModel {
		public string NameAr { get; set; }
		public string NameEn { get; set; }
		public string Address { get; set; }
		public Location Location { get; set; }
		public PuzzleFileInfo Image { get; set; }
		public string Phone { get; set; }
		public string EmergencyPhone { get; set; }
		public string Email { get; set; }
		public string Mobile { get; set; }
		public Guid CompanyId { get; set; }
		public string TimeZoneText { get; set; }
		public int TimeZoneOffset { get; set; }
		public string TimeZoneValue { get; set; }
	}

	public class EditCompoundViewModel : AddCompoundViewModel {
		public Guid CompoundId { get; set; }
	}
}
