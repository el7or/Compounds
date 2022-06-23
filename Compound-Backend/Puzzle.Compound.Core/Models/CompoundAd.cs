using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundAd
    {
        public CompoundAd()
        {
            Images = new HashSet<CompoundAdImage>();
            CompoundAdHistories = new HashSet<CompoundAdHistory>();
        }

        public Guid CompoundAdId { get; set; }
        public Guid CompoundId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AdUrl { get; set; }
        public string EnglishTitle { get; set; }
        public string ArabicTitle { get; set; }
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public virtual Compound Compound { get; set; }
        public virtual ICollection<CompoundAdImage> Images { get; set; }
        public virtual ICollection<CompoundAdHistory> CompoundAdHistories { get; set; }
    }
}
