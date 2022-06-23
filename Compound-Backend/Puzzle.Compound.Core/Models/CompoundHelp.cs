using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundHelp
    {
        public Guid CompoundHelpId { get; set; }
        public string Title { get; set; }
        public string HelpContent { get; set; }
        public DateTime RequestDate { get; set; }
        public bool IsResolved { get; set; }
        public Guid CompoundId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

        public virtual Compound Compound { get; set; }
    }
}
