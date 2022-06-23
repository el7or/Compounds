using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
   public class CompoundAdImage
    {
        public Guid CompoundAdImageId { get; set; }
        public Guid CompoundAdId { get; set; }
        public string Path { get; set; }
        public bool? IsMain { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public virtual CompoundAd CompoundAd { get; set; }
    }
}
