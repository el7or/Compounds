using System;

namespace Puzzle.Compound.Models.Units {
	public class UnitInfoViewModel {
		public Guid CompanyId { get; set; }
		public Guid CompoundUnitId { get; set; }
		public string Name { get; set; }
		public Guid CompoundGroupId { get; set; }
		public int CompoundUnitTypeId { get; set; }
		public Guid CompoundId { get; set; }
		public string CompoundName { get; set; }
		public string CompoundGroupNameEn { get; set; }
		public string CompoundGroupNameAr { get; set; }
		public string CompoundTimeZone { get; set; }
		public int CompoundTimeOffset { get; set; }
		public int OwnersCount { get; set; }
		public int SubOwnersCount { get; set; }
	}
}
