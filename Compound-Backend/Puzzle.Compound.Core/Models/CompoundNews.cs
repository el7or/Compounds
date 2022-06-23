using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundNews
    {
        public CompoundNews()
        {
            Images = new HashSet<CompoundNewsImage>();
        }
        public Guid CompoundNewsId { get; set; }
        public Guid CompoundId { get; set; }
        public string EnglishTitle { get; set; }
        public string ArabicTitle { get; set; }
        public string EnglishSummary { get; set; }
        public string ArabicSummary { get; set; }
        public string EnglishDetails { get; set; }
        public string ArabicDetails { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime? ForegroundTillDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }

        public virtual Compound Compound { get; set; }
        public virtual ICollection<CompoundNewsImage> Images { get; set; }
    }
}
