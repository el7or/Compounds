using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Core.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.VisitRequest
{
    public class VisitRequestOutputViewModel
    {
        public Guid VisitRequestId { get; set; }
        public string VisitorName { get; set; }
        public string Details { get; set; }
        public string CarNo { get; set; }
        public VisitType Type { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public int GroupNo { get; set; }
        public List<Day> Days { get; set; }
        public string Code { get; set; }
        public string QrCode { get; set; }
        public bool? IsConfirmed { get; set; }
        public bool IsCanceled { get; set; }
        public Guid OwnerRegistrationId { get; set; }
        public Guid CompoundUnitId { get; set; }
        public bool IsConsumed { get; set; }
        public List<string> Files { get; set; }
        public List<PuzzleFileInfo> Attachments { get; set; }
        public string OwnerName { get; set; }
        public OwnerRegistrationType? UserType { get; set; }
        public string UnitName { get; set; }
        public bool CanUpload { get; set; }
        public VisitStatus Status
        {
            get
            {
                if (IsCanceled)
                    return VisitStatus.Canceled;
                if (DateTo.HasValue && DateTo.Value.Date < DateTime.UtcNow.Date)
                    return VisitStatus.Expired;
                if (IsConsumed)
                    return VisitStatus.Consumed;
                if (IsConfirmed == true)
                    return VisitStatus.Confirmed;
                if (IsConfirmed == false)
                    return VisitStatus.NotConfirmed;
                return VisitStatus.Pending;
            }
        }
        public bool CanEdit { 
            get 
            {
                switch (Type)
                {
                    case VisitType.Once:
                        return !IsConsumed;
                    case VisitType.Periodic:
                        return true;
                    case VisitType.Labor:
                        return IsConfirmed == null && Status != VisitStatus.Expired;
                    case VisitType.Group:
                        return IsConfirmed == null && Status != VisitStatus.Expired;
                    default:
                        return false;
                }
            }
        }
        public bool CanShare
        {
            get
            {
                switch (Type)
                {
                    case VisitType.Once:
                        return !IsConsumed && Status != VisitStatus.Expired;
                    case VisitType.Group:
                        return IsConfirmed == true && Status != VisitStatus.Expired;
                    default:
                        return Status != VisitStatus.Expired;
                }
            }
        }

        public VisitRequestOwnerViewModel Owner { get; set; }
    }

    public class VisitRequestOwnerViewModel
    {
        public Guid? CompoundOwnerId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<string> Units { get; set; }
    }
}
