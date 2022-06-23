using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
   public partial class NotificationUnit
    {
        public Guid NotificationUnitId { get; set; }
        public Guid CompoundNotificationId { get; set; }
        public Guid CompoundUnitId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public virtual CompoundNotification CompoundNotification { get; set; }
        public virtual CompoundUnit CompoundUnit { get; set; }
    }
}
