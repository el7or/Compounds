using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models {
	public partial class CompoundOwner {
		public CompoundOwner() {
			OwnerUnits = new HashSet<OwnerUnit>();
		}

		public Guid CompoundOwnerId { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string Gender { get; set; }
		public string Address { get; set; }
		public string WhatsAppNum { get; set; }
		public DateTime? BirthDate { get; set; }
		public DateTime? CreationDate { get; set; }
		public string Image { get; set; }
		public Guid? OwnerRegistrationId { get; set; }
		public bool? IsDeleted { get; set; }
		public bool? IsActive { get; set; }

		public virtual OwnerRegistration OwnerRegistration { get; set; }
		public virtual ICollection<OwnerUnit> OwnerUnits { get; set; }
	}
}
