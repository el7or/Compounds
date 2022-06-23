using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompanyUserRole
    {
        public Guid CompanyUserRoleId { get; set; }
        public Guid CompanyUserId { get; set; }
        public Guid CompanyRoleId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid CreationUser { get; set; }
        public Guid? ModificationUser { get; set; }

        public virtual CompanyRole CompanyRole { get; set; }
        public virtual CompanyUser CompanyUser { get; set; }
        public virtual Compound Compound { get; set; }
    }
}
