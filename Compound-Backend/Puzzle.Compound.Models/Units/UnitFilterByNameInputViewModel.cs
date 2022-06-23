using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Units
{
    public class UnitFilterByTextInputViewModel : PagedInput
    {
        public string Text { get; set; }
        public Guid? CompoundId { get; set; }
    }
}
