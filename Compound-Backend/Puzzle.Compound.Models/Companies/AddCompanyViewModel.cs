using System;

namespace Puzzle.Compound.Models.Companies
{
    public class AddCompanyViewModel
    {
        public string Name_Ar { get; set; }
        public string Name_En { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Location Location { get; set; }
        public string Website { get; set; }
        public string WhatsAppNum { get; set; }
        public PuzzleFileInfo Logo { get; set; }
        public string ChargeName { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public Guid PlanId { get; set; }
    }

}
