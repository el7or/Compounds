using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Mappings
{
    public class OwnerRegistrationFullInfo
    {
        public Guid OwnerRegistrationId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string WhatsAppNum { get; set; }
        public DateTime? RegisterDate { get; set; }
        public DateTime? BirthDate { get; set; }
        public int OwnerStatus { get; set; }
        public bool? UserConfirmed { get; set; }
        public Guid CompoundId { get; set; }
        public string CompoundNameAr { get; set; }
        public string CompoundNameEn { get; set; }
    }
}
