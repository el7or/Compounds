using System;

namespace Puzzle.Compound.Models.Gates {
	public class GateLoginModel {
		public Guid UserId { get; set; }
		public Guid GateId { get; set; }
		public string GateName { get; set; }
		public int TimezoneOffset { get; set; }
		public string TimezoneValue { get; set; }
		public int EntryType { get; set; }
		public object Token { get; set; }
	}
}
