using System;

namespace Puzzle.Compound.Models.Compounds
{
   public class CompoundViewModel
    {
		public Guid CompoundId { get; set; }
		public string NameAr { get; set; }
		public string NameEn { get; set; }
		public string Address { get; set; }
		public string Location { get; set; }
		public string Image { get; set; }
		public string Phone { get; set; }
		public string EmergencyPhone { get; set; }
		public string Email { get; set; }
		public string Mobile { get; set; }
		public DateTime CreationDate { get; set; }
		public Guid CompanyId { get; set; }
		public bool? IsDeleted { get; set; }
		public bool? IsActive { get; set; }
		public string TimeZoneText { get; set; }
		public int TimeZoneOffset { get; set; }
		public string TimeZoneValue { get; set; }
		public int? OwnersCount { get; set; }
		public int? UnitsCount { get; set; }
		public int? ServicesCount { get; set; }
	}
}
