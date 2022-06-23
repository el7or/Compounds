using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Core.Models
{
    public partial class ActionsInCompanyRoles
    {
        public Guid ActionsInCompanyRolesId { get; set; }
        public Guid CompanyRoleId { get; set; }
        public Guid SystemPageActionId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid CreationUser { get; set; }
        public Guid? ModificationUser { get; set; }

        public virtual CompanyRole CompanyRoles { get; set; }
        public virtual SystemPageAction SystemPageActions { get; set; }
    }
}
