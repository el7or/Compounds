using System;
using System.ComponentModel.DataAnnotations;

namespace Puzzle.Compound.Models.Owners
{
    public class OwnerInputViewModel
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
        public int? SubOwnersCount { get; set; }
        public bool? isActive { get; set; }
        public Guid[] UnitsIds { get; set; }
        public PuzzleFileInfo Image { get; set; }
    }
}
