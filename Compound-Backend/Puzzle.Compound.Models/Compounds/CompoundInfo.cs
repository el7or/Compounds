using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Compounds {
	public class CompoundInfo {
		public Guid CompoundId { get; set; }
		public string CompoundName { get; set; }
		public string TimeZoneText { get; set; }
		public int TimeOffset { get; set; }
		public string TimeZoneValue { get; set; }
	}
}
