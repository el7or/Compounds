using System;

namespace Puzzle.Compound.Models.Units {
	public class SubUserAssignUnitViewModel {
		public DateTime? Start_From { get; set; }
		public DateTime? End_To { get; set; }
		public Guid Compound_Unit_Id { get; set; }
		public string CompoundUnitName { get; set; }
	}
}
