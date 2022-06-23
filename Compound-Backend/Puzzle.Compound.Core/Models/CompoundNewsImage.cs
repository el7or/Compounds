using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundNewsImage
    {
        public Guid CompoundNewsImageId { get; set; }
        public Guid CompoundNewsId { get; set; }
        public string Path { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public virtual CompoundNews CompoundNews { get; set; }
    }
}
