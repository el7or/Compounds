using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.OwnerUnits
{
    public class AddOwnerUnitsViewModel
    {
        public Guid CompoundOwnerId { get; set; }
        public Guid[] Units { get; set; }
    }
}
