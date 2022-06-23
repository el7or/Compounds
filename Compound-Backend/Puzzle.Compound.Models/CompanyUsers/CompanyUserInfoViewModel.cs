using System;

namespace Puzzle.Compound.Models.CompanyUsers {
	public class CompanyUserInfoViewModel {
		public Guid CompanyUserId { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string NameAr { get; set; }
		public string NameEn { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public bool? IsActive { get; set; }
		public Guid? CompanyId { get; set; }
		public int? UserType { get; set; }
		public PuzzleFileInfo UserImage { get; set; }
		public CompoundUserInfo[] CompanyUserCompounds { get; set; }
	}

	public class CompoundUserInfo {
		public Guid CompoundId { get; set; }
		public Guid[] Services { get; set; }
		public Guid[] Issues { get; set; }
		public Guid[] Reports { get; set; }
	}
}
