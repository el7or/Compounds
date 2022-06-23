using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompanyUser
    {
        public CompanyUser()
        {
            CompanyUserCompounds = new HashSet<CompanyUserCompound>();
            CompanyUserRoles = new HashSet<CompanyUserRole>();
            CompoundNotices = new HashSet<CompoundNotice>();
        }

        public Guid CompanyUserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? Online { get; set; }
        public bool? IsVerified { get; set; }
        public DateTime? CreationDate { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? Id { get; set; }
        public int? UserType { get; set; }
        public string Image { get; set; }

        public virtual Company Company { get; set; }
        public virtual Gate Gate { get; set; }
        public virtual ICollection<CompanyUserCompound> CompanyUserCompounds { get; set; }
        public virtual ICollection<CompanyUserRole> CompanyUserRoles { get; set; }
        public virtual ICollection<CompoundNotice> CompoundNotices { get; set; }
    }
}
