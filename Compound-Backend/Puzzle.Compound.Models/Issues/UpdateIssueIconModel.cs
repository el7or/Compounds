using Microsoft.AspNetCore.Http;

namespace Puzzle.Compound.Models.Issues
{
    public class UpdateIssueIconModel
    {
        public IFormFile Icon { get; set; }
        public string FileName { get; set; }
    }
}
