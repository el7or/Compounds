using Puzzle.Compound.Common.Enums;
using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models {
	public partial class OwnerRegistration {
		public OwnerRegistration() {
			CompoundOwners = new HashSet<CompoundOwner>();
			OwnerAssignUnitRequests = new HashSet<OwnerAssignUnitRequest>();
			OwnerAssignedUnits = new HashSet<OwnerAssignedUnit>();
			VisitRequests = new HashSet<VisitRequest>();
			PrintCardRequests = new HashSet<PrintCardRequest>();
			ServiceRequests = new HashSet<ServiceRequest>();
			IssueRequests = new HashSet<IssueRequest>();
			OwnerNotifications = new HashSet<OwnerNotification>();
			CompoundCalls = new HashSet<CompoundCall>();
		}

        public Guid OwnerRegistrationId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string WhatsAppNum { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Image { get; set; }
        public DateTime? RegisterDate { get; set; }
        public OwnerRegistrationType? UserType { get; set; }
        public Guid? MainRegistrationId { get; set; }
        public Guid? CreatedByRegistrationId { get; set; }
        public bool? UserConfirmed { get; set; }
        public bool? IsBlocked { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

		public virtual ICollection<CompoundOwner> CompoundOwners { get; set; }
		public virtual ICollection<OwnerAssignUnitRequest> OwnerAssignUnitRequests { get; set; }
		public virtual ICollection<OwnerAssignedUnit> OwnerAssignedUnits { get; set; }
		public virtual ICollection<VisitRequest> VisitRequests { get; set; }
		public virtual ICollection<PrintCardRequest> PrintCardRequests { get; set; }
		public virtual ICollection<ServiceRequest> ServiceRequests { get; set; }
		public virtual ICollection<IssueRequest> IssueRequests { get; set; }
		public virtual ICollection<OwnerNotification> OwnerNotifications { get; set; }
		public virtual ICollection<CompoundAdHistory> CompoundAdHistories { get; set; }
		public virtual ICollection<CompoundCall> CompoundCalls { get; set; }
		public virtual ICollection<NotificationUser> NotificationUsers { get; set; } = new HashSet<NotificationUser>();
		public virtual ICollection<RegistrationForUser> RegistrationForUsers { get; set; } = new HashSet<RegistrationForUser>();
	}
}
