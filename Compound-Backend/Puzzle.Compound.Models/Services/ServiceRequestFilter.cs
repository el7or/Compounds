using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Services {
	public class ServiceRequestFilter : PagedInput{
		public Guid CompoundId { get; set; }
		public Guid[] ServiceTypeIds { get; set; }
		public short? Status { get; set; }
		public DateTime? From { get; set; }
		public DateTime? To { get; set; }
		public string SearchText { get; set; }
	}
}
