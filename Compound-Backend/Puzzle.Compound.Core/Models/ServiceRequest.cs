using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Core.Models
{
    public class ServiceRequest
    {
        public ServiceRequest()
        {
            ServiceAttachments = new HashSet<ServiceAttachment>();
            ServiceRequestSubTypes = new HashSet<ServiceRequestSubType>();
        }
        public Guid ServiceRequestId { get; set; }
        public Guid ServiceTypeId { get; set; }        
        public Guid OwnerRegistrationId { get; set; }
        public int RequestNumber { get; set; }
        public DateTime PostTime { get; set; }        
        public short Status { get; set; } // 0 => pending , 1 => done , 2 => cancelled
        public DateTime UpdateStatusTime { get; set; }
        public Guid UpdateStatusBy { get; set; }
        public Guid CompoundUnitId { get; set; }
        public Guid CompoundId { get; set; }
        public string Note { get; set; }
        public short Rate { get; set; }
        public short PresenterRate { get; set; }
        public string Comment { get; set; }
        public string OwnerComment { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime Date { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public short CancelType { get; set; } // 0 => owner , 1 => admin
        public string Record { get; set; }
        public decimal ServiceSubTypesTotalCost { get; set; }
        public virtual OwnerRegistration OwnerRegistration { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public virtual CompoundUnit CompoundUnit { get; set; }
        public virtual Compound Compound { get; set; }
        public virtual ICollection<ServiceAttachment> ServiceAttachments { get; set; }
        public virtual ICollection<ServiceRequestSubType> ServiceRequestSubTypes { get; set; }
    }
}
