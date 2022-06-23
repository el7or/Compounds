using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.VisitRequest
{
    public class VisitRequestApproveInputViewModel
    {
        public bool IsApproved { get; set; }
        public Guid Id { get; set; }
        public List<VisitAttachmentViewModel> Attachments { get; set; }
    }
}
