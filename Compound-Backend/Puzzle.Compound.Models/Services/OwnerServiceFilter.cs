using Puzzle.Compound.Common.Models;
using System;

namespace Puzzle.Compound.Models.Services {
	public class OwnerServiceFilter : PagedInput {
		public Guid OwnerRegistrationId { get; set; }
		public DateTime? DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
		public bool IsUpcoming { get; set; }
		public Guid [] ServiceTypes { get; set; }
	}
}
