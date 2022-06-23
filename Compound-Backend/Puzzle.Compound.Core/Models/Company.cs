using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class Company
    {
        public Company()
        {
            CompanyUsers = new HashSet<CompanyUser>();
            Compounds = new HashSet<Compound>();
            VisitRequests = new HashSet<VisitRequest>();
            CompanyRoles = new HashSet<CompanyRole>();
        }

        public Guid CompanyId { get; set; }
        public string Name_Ar { get; set; }
        public string Name_En { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string WhatsAppNum { get; set; }
        public string Logo { get; set; }
        public DateTime CreationDate { get; set; }
        public int? CompanyType { get; set; }
        public Guid PlanId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public string ChargeName { get; set; }

        public virtual Plan Plan { get; set; }
        public virtual ICollection<CompanyUser> CompanyUsers { get; set; }
        public virtual ICollection<CompanyRole> CompanyRoles { get; set; }
        public virtual ICollection<Compound> Compounds { get; set; }
        public virtual ICollection<VisitRequest> VisitRequests { get; set; }
    }
}
