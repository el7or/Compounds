using System;

namespace Puzzle.Compound.Models.VisitRequest {
	public class RequestVisitGateByCodeViewModel {
		public string Code { get; set; }
		public Guid GateId { get; set; }
		public string SecurityKey { get; set; }
		public int Timezone { get; set; }
	}
}
