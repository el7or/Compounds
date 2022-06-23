using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Models.Units;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Puzzle.Compound.Models.Owners {
	public class OwnerRegisterViewModel {
		public Guid OwnerRegistrationId { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Phone { get; set; }
		[Required]
		public string Email { get; set; }
		public string CompanyId { get; set; }
		public string Gender { get; set; }
		public string Address { get; set; }
		public string WhatsAppNum { get; set; }
		public DateTime? BirthDate { get; set; }
		public string Image { get; set; }
		public DateTime? RegisterDate { get; set; }
		public OwnerRegistrationType? UserType { get; set; }
		public bool IsActive { get; set; }
		public List<SubUserAssignUnitViewModel> Units { get; set; }
	}
}