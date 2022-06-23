using System;

namespace Puzzle.Compound.Models.VisitRequest {
	public class RequestByQrCodeViewModel {
		public string Code { get; set; }
		public Guid? UserId { get; set; }
		public Guid GateId { get; set; }
		public int Timezone { get; set; }

		}
}
