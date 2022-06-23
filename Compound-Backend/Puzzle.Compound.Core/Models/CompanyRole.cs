using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompanyRole
    {
        public CompanyRole()
        {
            CompanyUserRoles = new HashSet<CompanyUserRole>();
            ActionsInCompanyRoles = new HashSet<ActionsInCompanyRoles>();
        }

        public Guid CompanyRoleId { get; set; }
        public Guid CompanyId { get; set; }
        public string RoleArabicName { get; set; }
        public string RoleEnglishName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid CreationUser { get; set; }
        public Guid? ModificationUser { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<CompanyUserRole> CompanyUserRoles { get; set; }
        public virtual ICollection<ActionsInCompanyRoles> ActionsInCompanyRoles { get; set; }
    }
}
