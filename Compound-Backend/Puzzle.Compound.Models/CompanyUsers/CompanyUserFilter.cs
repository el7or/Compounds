using Puzzle.Compound.Common.Models;
using System;

namespace Puzzle.Compound.Models.CompanyUsers {
	public class CompanyUserFilter : PagedInput
	{
		public Guid? CompanyId { get; set; }
		public Guid? CompanyRoleId { get; set; }
		public string SearchText { get; set; }
	}
}
