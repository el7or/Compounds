using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.CompanyRoles
{
    public class CompanyRoleInfoViewModel
    {
        public Guid CompanyRoleId { get; set; }
        public Guid CompanyId { get; set; }
        public string RoleArabicName { get; set; }
        public string RoleEnglishName { get; set; }
        public int? PagesCount { get; set; }
        public int? UsersCount { get; set; }
    }
    public class CompanyCurrentRoleViewModel
    {
        public Guid CompanyRoleId { get; set; }
        public Guid CompanyId { get; set; }
        public string RoleArabicName { get; set; }
        public string RoleEnglishName { get; set; }
    }
}
