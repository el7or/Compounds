using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.PushNotifications
{
    public class NotificationScheduleViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string NotificationText { get; set; }
        public string NotificationTextEn { get; set; }
        public string NotificationData { get; set; }
        public Guid RecordId { get; set; }
        public DateTime PostDatetime { get; set; }
        public DateTime SendDateTime { get; set; }
        public DateTime ScheduleDateTime { get; set; }
        public int NotificationType { get; set; }
        public bool HasSend { get; set; }
        public bool HasError { get; set; }
        public string GlobalType { get; set; }
        public int? PrioritySend { get; set; }

        public List<NotificationUserViewModel> NotificationUsers { get; set; }
    }
}
