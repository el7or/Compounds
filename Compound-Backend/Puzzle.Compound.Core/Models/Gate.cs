using Puzzle.Compound.Common.Enums;
using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models {
	public partial class Gate {
		public Gate() {
			CompoundGates = new HashSet<CompoundGate>();
			VisitTransactionHistories = new HashSet<VisitTransactionHistory>();
		}

		public Guid GateId { get; set; }
		public string GateName { get; set; }
		public GateEntryType EntryType { get; set; }
		public bool? IsDeleted { get; set; }
		public bool? IsActive { get; set; }
		public Guid? CompanyUserId { get; set; }
		public virtual CompanyUser User { get; set; }
		public virtual ICollection<CompoundGate> CompoundGates { get; set; }
		public virtual ICollection<VisitTransactionHistory> VisitTransactionHistories { get; set; }
	}
}
