using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Issues
{
    public class IssueRequestModel
    {
        public Guid OwnerRegistrationId { get; set; }
        public Guid IssueTypeId { get; set; }
        public string Note { get; set; }
        public Location Location { get; set; }
        public IFormFile Record { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
