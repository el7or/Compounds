using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.VisitRequest
{
    public class VisitRequestFilterByUserInputViewModel : PagedInput
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<VisitType> Types { get; set; }
        public bool IsUpcoming { get; set; }
    }
}
