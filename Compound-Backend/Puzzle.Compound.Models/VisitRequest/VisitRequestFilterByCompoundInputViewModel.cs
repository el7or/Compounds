using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.VisitRequest
{
    public class VisitRequestFilterByCompoundInputViewModel : PagedInput
    {
        public List<Guid> Ids { get; set; }
    }
}
