using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Core.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
    public class VisitRequest
    {
        public VisitRequest()
        {
            Attachments = new HashSet<VisitRequestAttachment>();
            VisitTransactionHistories = new HashSet<VisitTransactionHistory>();
            PrintCardRequests = new HashSet<PrintCardRequest>();
        }
        public Guid VisitRequestId { get; set; }
        public string VisitorName { get; set; }
        public string Details { get; set; }
        public string CarNo { get; set; }
        public VisitRequestType Type { get; set; }
        public VisitType VisitType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string QrCode { get; set; }
        public string Code { get; set; }
        public int GroupNo { get; set; }
        public string Days { get; set; }
        public bool? IsConfirmed { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsConsumed { get; set; }
        public bool IsCanceled { get; set; }
        public Guid OwnerRegistrationId { get; set; }
        public Guid CompoundUnitId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid CompoundId { get; set; }

        public virtual Company Company { get; set; }
        public virtual Compound Compound { get; set; }
        public virtual CompoundUnit CompoundUnit { get; set; }
        public virtual OwnerRegistration OwnerRegistration { get; set; }
        public virtual ICollection<VisitRequestAttachment> Attachments { get; set; }
        public virtual ICollection<VisitTransactionHistory> VisitTransactionHistories { get; set; }
        public virtual ICollection<PrintCardRequest> PrintCardRequests { get; set; }
    }
}
