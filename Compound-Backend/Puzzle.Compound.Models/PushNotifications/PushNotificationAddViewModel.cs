using Puzzle.Compound.Common.Enums;
using System;

namespace Puzzle.Compound.Models.PushNotifications
{
    public class PushNotificationAddViewModel
    {
        public PushNotificationType NotificationType { get; set; }
        public Guid RecordId { get; set; }
    }
}
