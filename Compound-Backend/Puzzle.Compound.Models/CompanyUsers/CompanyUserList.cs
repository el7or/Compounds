using Puzzle.Compound.Models.CompanyRoles;
using System;

namespace Puzzle.Compound.Models.CompanyUsers {
	public class CompanyUserList {
		public Guid CompanyUserId { get; set; }
		public string NameAr { get; set; }
		public string NameEn { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
		public bool? IsActive { get; set; }
        public bool? IsSelected { get; set; }
        public CompanyCurrentRoleViewModel CurrentRole { get; set; }
    }
}
