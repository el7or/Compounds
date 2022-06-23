using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Core.Mappings;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.VisitRequest
{
    public class VisitRequestAddInputViewModel
    {
        public VisitRequestAddInputViewModel()
        {
            Files = new List<VisitAttachmentViewModel>();
        }
        public string VisitorName { get; set; }
        public string Details { get; set; }
        public string CarNo { get; set; }
        public VisitType Type { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int GroupNo { get; set; }
        public List<Day> Days { get; set; }
        public Guid CompoundUnitId { get; set; }
        public List<VisitAttachmentViewModel> Files { get; set; }
    }

    public class VisitAttachmentViewModel
    {
        public byte[] FileBytes { get; set; }
        // public VisitAttachmentType Type { get; set; }
        public string FileName { get; set; }
        public string FileBase64 { get; set; }
    }
}
