using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models {
	public partial class CompoundSecurity {
		public Guid CompoundSecurityId { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string NationalId { get; set; }
		public Guid CompoundId { get; set; }
		public bool? IsDeleted { get; set; }
		public bool? IsActive { get; set; }

		public virtual Compound Compound { get; set; }
	}
}
