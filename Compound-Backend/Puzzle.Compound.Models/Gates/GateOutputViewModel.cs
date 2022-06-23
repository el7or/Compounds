using Puzzle.Compound.Common.Enums;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Gates {
	public class GateOutputViewModel {
		public Guid GateId { get; set; }
		public string GateName { get; set; }
		public GateEntryType EntryType { get; set; }
		public List<Guid> CompoundIds { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string UserId { get; set; }
	}
}
