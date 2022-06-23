using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Services {
	public class ServiceRequestViewModel {
		public string Comment { get; set; }
		public Guid ServiceRequestId { get; set; }
		public DateTime PostTime { get; set; }
		public DateTime From { get; set; }
		public DateTime To { get; set; }
		public string OwnerComment { get; set; }
		public int RequestNumber { get; set; }
		public short Status { get; set; }
		public string Note { get; set; }
		public short Rate { get; set; }
		public string ServiceTypeArabic { get; set; }
		public string ServiceTypeEnglish { get; set; }
		public string UnitName { get; set; }
		public string OwnerName { get; set; }
		public string OwnerPhone { get; set; }
		public short PresenterRate { get; set; }
		public string Icon { get; set; }
		public string Record { get; set; }
		public List<string> Attachments { get; set; }
		public decimal ServiceSubTypesTotalCost { get; set; }
		public List<ServiceRequestSubTypeOutPutViewModel> ServiceSubTypes { get; set; }
	}
}
