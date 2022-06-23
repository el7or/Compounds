using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Notifications
{
   public class NotificationOutputViewModel
    {
        public Guid CompoundNotificationId { get; set; }
        public string EnglishTitle { get; set; }
        public string ArabicTitle { get; set; }
        public string EnglishMessage { get; set; }
        public string ArabicMessage { get; set; }
        public bool? IsOwnerOnly { get; set; }
        public bool? IsOwnerRead { get; set; }
        public DateTime? OwnerReadOn { get; set; }
        public List<Guid> ToGroupsIds { get; set; }
        public List<Guid> ToUnitsIds { get; set; }
        public List<PuzzleFileInfo> Images { get; set; }
    }
   public class NotificationMobileOutputViewModel
    {
        public Guid CompoundNotificationId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool? IsOwnerOnly { get; set; }
        public bool? IsOwnerRead { get; set; }
        public DateTime? OwnerReadOn { get; set; }
        public List<Guid> ToUnitsIds { get; set; }
        public List<PuzzleFileInfo> Images { get; set; }
    }
}
