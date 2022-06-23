using Puzzle.Compound.Core.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
    public class VisitRequestAttachment
    {
        public Guid VisitRequestAttachmentId { get; set; }
        public string Path { get; set; }
        // public VisitAttachmentType Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Guid VisitRequestId { get; set; }
        public virtual VisitRequest VisitRequest { get; set; }
    }
}
