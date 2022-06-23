using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundCardPrintRequest
    {
        public Guid CompoundCardRequestId { get; set; }
        public int CardsCount { get; set; }
        public DateTime RequestDate { get; set; }
        public Guid CompoundId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }

        public virtual Compound Compound { get; set; }
    }
}
