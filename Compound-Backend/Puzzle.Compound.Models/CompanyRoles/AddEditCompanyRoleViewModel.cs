using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.CompanyRoles
{
    public class AddEditCompanyRoleViewModel
    {
        public Guid? CompanyRoleId { get; set; }
        public Guid CompanyId { get; set; }
        public string RoleArabicName { get; set; }
        public string RoleEnglishName { get; set; }
    }
}
