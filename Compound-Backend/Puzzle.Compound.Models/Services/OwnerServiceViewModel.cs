using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Services {
	public class OwnerServiceViewModel {
		public string Icon { get; set; }
		public string Date { get; set; }
		public string From { get; set; }
		public string To { get; set; }
		public string ServiceType { get; set; }
		public Guid ServiceTypeId { get; set; }
		public bool CanEdit { get; set; }
		public bool CanRate { get; set; }
		public string Note { get; set; }
		public short Rate { get; set; }
		public short PresenterRate { get; set; }
		public string Comment { get; set; }
		public string OwnerComment { get; set; }
		public int Status { get; set; }
		public DateTime PostTime { get; set; }
		public string Record { get; set; }
		public int RequestNumber { get; set; }
		public List<string> Attachments { get; set; }
		public string UnitName { get; set; }
		public decimal ServiceSubTypesTotalCost { get; set; }
		public List<ServiceRequestSubTypeOutPutViewModel> ServiceSubTypes { get; set; }
	}
}
