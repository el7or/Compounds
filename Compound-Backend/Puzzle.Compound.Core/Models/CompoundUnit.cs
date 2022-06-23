using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models {
	public partial class CompoundUnit {
		public CompoundUnit() {
			OwnerUnits = new HashSet<OwnerUnit>();
			OwnerAssignUnitRequests = new HashSet<OwnerAssignUnitRequest>();
			OwnerAssignedUnits = new HashSet<OwnerAssignedUnit>();
			VisitRequests = new HashSet<VisitRequest>();
			PrintCardRequests = new HashSet<PrintCardRequest>();
			ServiceRequests = new HashSet<ServiceRequest>();
			NotificationUnits = new HashSet<NotificationUnit>();
		}

		public Guid CompoundUnitId { get; set; }
		public string Name { get; set; }
		public Guid CompoundGroupId { get; set; }
		public int CompoundUnitTypeId { get; set; }
		public bool? IsDeleted { get; set; }
		public bool? IsActive { get; set; }

		public virtual CompoundGroup CompoundGroup { get; set; }
		public virtual CompoundUnitType CompoundUnitType { get; set; }
		public virtual ICollection<OwnerUnit> OwnerUnits { get; set; }
		public virtual ICollection<OwnerAssignUnitRequest> OwnerAssignUnitRequests { get; set; }
		public virtual ICollection<OwnerAssignedUnit> OwnerAssignedUnits { get; set; }
		public virtual ICollection<VisitRequest> VisitRequests { get; set; }
		public virtual ICollection<PrintCardRequest> PrintCardRequests { get; set; }
		public virtual ICollection<ServiceRequest> ServiceRequests { get; set; }
		public virtual ICollection<NotificationUnit> NotificationUnits { get; set; }
	}
}
