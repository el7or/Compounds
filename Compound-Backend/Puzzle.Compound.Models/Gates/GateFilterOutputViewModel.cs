using Puzzle.Compound.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Gates
{
    public class GateFilterOutputViewModel
    {
        public Guid GateId { get; set; }
        public string GateName { get; set; }
        public GateEntryType EntryType { get; set; }
    }
}
