using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.PrintCardRequest
{
    public class PrintCardFilterByCompoundInputViewModel : PagedInput
    {
        public List<Guid> Ids { get; set; }
    }
}
