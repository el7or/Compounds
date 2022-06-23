using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Notifications
{
   public class NotificationFilterViewModel : PagedInput
    {
        public Guid? CompoundId { get; set; }
        public Guid? OwnerRegistrationId { get; set; }
        public string SearchText { get; set; }
    }
}
