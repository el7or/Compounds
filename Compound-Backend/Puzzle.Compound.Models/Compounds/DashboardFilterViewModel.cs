using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Compounds
{
    public class DashboardFilterViewModel
    {
        public Guid CompoundId { get; set; }
        public ChartScope? ChartScope { get; set; }
        public List<Guid> ServiceTypesIds { get; set; }
        public List<Guid> IssueTypesIds { get; set; }
    }
}
