using Puzzle.Compound.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
   public class CompoundAdHistory
    {
        public Guid CompoundAdHistoryId { get; set; }
        public Guid CompoundAdId { get; set; }
        public Guid OwnerRegistrationId { get; set; }
        public ActionType ActionType { get; set; }
        public DateTime ActionDate { get; set; }

        public virtual OwnerRegistration OwnerRegistration { get; set; }
        public virtual CompoundAd CompoundAd { get; set; }
    }
}
