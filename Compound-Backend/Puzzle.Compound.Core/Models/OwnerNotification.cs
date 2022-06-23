using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
   public partial class OwnerNotification
    {
        public Guid OwnerNotificationId { get; set; }
        public Guid OwnerRegistrationId { get; set; }
        public Guid CompoundNotificationId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual OwnerRegistration OwnerRegistration { get; set; }
        public virtual CompoundNotification CompoundNotification { get; set; }
    }
}
