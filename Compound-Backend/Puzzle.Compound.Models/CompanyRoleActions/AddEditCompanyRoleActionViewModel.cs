using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.CompanyRoleActions
{
    public class AddEditCompanyRoleActionViewModel
    {
        public Guid? ActionsInCompanyRolesId { get; set; }
        public Guid CompanyRoleId { get; set; }
        public Guid SystemPageActionId { get; set; }
    }

    public class UpdatePagesActionsInRoles
    {
        public IEnumerable<AddEditCompanyRoleActionViewModel> PageActionsInRoles { get; set; }
    }
}
