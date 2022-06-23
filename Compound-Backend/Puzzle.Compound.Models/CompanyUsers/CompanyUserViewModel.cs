using System;

namespace Puzzle.Compound.Models.CompanyUsers {
	public class CompanyUserViewModel {
		public Guid? CompanyUserId { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string NameAr { get; set; }
		public string NameEn { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public Guid? CompanyId { get; set; }
		public Guid? Id { get; set; }
		public int? UserType { get; set; }
		public CompoundUserInfo[] Compounds { get; set; }
		public PuzzleFileInfo UserImage { get; set; }
	}
}
