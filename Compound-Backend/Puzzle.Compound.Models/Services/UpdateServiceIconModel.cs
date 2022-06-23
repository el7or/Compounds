using Microsoft.AspNetCore.Http;

namespace Puzzle.Compound.Models.Services {
	public class UpdateServiceIconModel {
		public IFormFile Icon { get; set; }
		public string FileName { get; set; }
	}
}
