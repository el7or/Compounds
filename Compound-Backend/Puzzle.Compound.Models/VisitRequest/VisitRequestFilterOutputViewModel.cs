using Puzzle.Compound.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.VisitRequest {
	public class VisitRequestFilterOutputViewModel {
		public Guid VisitRequestId { get; set; }
		public string VisitorName { get; set; }
		public string Details { get; set; }
		public string CarNo { get; set; }
		public VisitType Type { get; set; }
		public DateTime? DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
		public int GroupNo { get; set; }
		public List<Day> Days { get; set; }
		public string Code { get; set; }
		public string QrCode { get; set; }
		public string OwnerName { get; set; }
		public string OwnerPhone { get; set; }
		public string UnitName { get; set; }
		public bool? IsConfirmed { get; set; }
		public bool IsCanceled { get; set; }
		public bool IsConsumed { get; set; }
		public VisitStatus Status {
			get {
				if (IsCanceled)
					return VisitStatus.Canceled;
				if (DateTo.HasValue && DateTo.Value.Date < DateTime.Now.Date)
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
	}
}
