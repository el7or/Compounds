using Puzzle.Compound.Models.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Owners
{
    public class OwnerInfoViewModel
    {
        public Guid? CompoundOwnerId { get; set; }
        public Guid? OwnerRegistrationId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string WhatsAppNum { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Image { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsActive { get; set; }
        public int? SubOwnersCount { get; set; }
        public Guid[] UnitsIds { get; set; }
        public List<string> UnitsNames { get; set; }
        public List<UnitInfoViewModel> Units { get; set; }
        public List<OwnerRegisterViewModel> SubOwners { get; set; }
    }
}
