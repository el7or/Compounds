using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.VisitRequest {
	public class ConfirmVisitWithAttachModel {
		public List<IFormFile> Files { get; set; }
	}

	public class ConfirmVisitWithAttachResponse {
		public Guid VisitRequestId { get; set; }
		public string[] Attachments { get; set; }
	}
}
