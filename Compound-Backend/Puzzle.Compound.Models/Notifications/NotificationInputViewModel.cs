using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Notifications
{
    public class NotificationInputViewModel
    {
        public Guid? CompoundNotificationId { get; set; }
        public Guid CompoundId { get; set; }
        public string EnglishTitle { get; set; }
        public string ArabicTitle { get; set; }
        public string EnglishMessage { get; set; }
        public string ArabicMessage { get; set; }
        public bool? IsOwnerOnly { get; set; }
        public List<Guid> ToUnitsIds { get; set; }
        public List<PuzzleFileInfo> Images { get; set; }
    }
}
