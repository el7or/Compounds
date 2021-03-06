using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.PushNotifications
{
    public class NotificationUserViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Guid NotificationScheduleId { get; set; }
        public Guid OwnerRegistrationId { get; set; }
        public bool IsRead { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
    }
}
