using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
   public partial class CompoundNotificationImage
    {
        public Guid CompoundNotificationImageId { get; set; }
        public Guid CompoundNotificationId { get; set; }
        public string Path { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public virtual CompoundNotification CompoundNotification { get; set; }
    }
}
