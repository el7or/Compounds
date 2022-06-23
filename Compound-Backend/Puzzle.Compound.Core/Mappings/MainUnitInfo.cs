using System;

namespace Puzzle.Compound.Core.Mappings {
	public class MainUnitInfo {
		public Guid CompanyId { get; set; }
		public Guid CompoundUnitId { get; set; }
		public string Name { get; set; }
		public Guid CompoundGroupId { get; set; }
		public int CompoundUnitTypeId { get; set; }
		public Guid CompoundId { get; set; }
		public string CompoundName { get; set; }
		public string CompoundTimeZone { get; set; }
		public int CompoundTimeOffset { get; set; }
	}
}
