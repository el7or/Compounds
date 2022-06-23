using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.CompanyUserRoles
{
    public class CompanyUserRoleInfoViewModel
    {
        public Guid CompanyUserRoleId { get; set; }
        public Guid CompanyUserId { get; set; }
        public Guid CompanyRoleId { get; set; }
        public DateTime AssignedDate { get; set; }
    }
}
