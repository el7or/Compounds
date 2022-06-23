using System;

namespace Puzzle.Compound.Models.Companies
{
    public class CompanyInfo2ViewModel
    {
        public Guid CompanyId { get; set; }
        public string Name_Ar { get; set; }
        public string Name_En { get; set; }
    }

    public class CompanyInfoViewModel
    {
        public Guid CompanyId { get; set; }
        public string Name_Ar { get; set; }
        public string Name_En { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Location Location { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string WhatsAppNum { get; set; }
        public string Logo { get; set; }
        public DateTime CreationDate { get; set; }
        public PlanViewModel Plan { get; set; }
        public bool? IsActive { get; set; }

    }
}