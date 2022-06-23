using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompoundNotification
    {
        public CompoundNotification()
        {
            Images = new HashSet<CompoundNotificationImage>();
            OwnerNotifications = new HashSet<OwnerNotification>();
            NotificationUnits = new HashSet<NotificationUnit>();
        }
        public Guid CompoundNotificationId { get; set; }
        public Guid CompoundId { get; set; }
        public string EnglishTitle { get; set; }
        public string ArabicTitle { get; set; }
        public string EnglishMessage { get; set; }
        public string ArabicMessage { get; set; }
        public bool? IsOwnerOnly { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }

        public virtual Compound Compound { get; set; }
        public virtual ICollection<CompoundNotificationImage> Images { get; set; }
        public virtual ICollection<OwnerNotification> OwnerNotifications { get; set; }
        public virtual ICollection<NotificationUnit> NotificationUnits { get; set; }
    }
}
