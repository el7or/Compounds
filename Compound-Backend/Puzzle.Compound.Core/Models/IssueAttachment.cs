using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
    public class IssueAttachment
    {
        public Guid IssueAttachmentId { get; set; }
        public string Path { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Guid IssueRequestId { get; set; }
        public virtual IssueRequest IssueRequest { get; set; }
    }
}
