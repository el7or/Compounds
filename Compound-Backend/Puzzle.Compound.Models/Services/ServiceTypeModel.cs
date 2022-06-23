using System;

namespace Puzzle.Compound.Models.Services {
	public class ServiceTypeModel {
		public Guid ServiceTypeId { get; set; }
		public Guid CompoundId { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }
		public bool IsFixed { get; set; }
		public string Icon { get; set; }
	}
}
