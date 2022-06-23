using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
    public class CompoundCall
    {
        public Guid CompoundCallId { get; set; }
        public Guid CompoundId { get; set; }
        public Guid OwnerRegistrationId { get; set; }
        public string EmergencyPhone { get; set; }
        public DateTime CallDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Compound Compound { get; set; }
        public virtual OwnerRegistration OwnerRegistration { get; set; }
    }
}
