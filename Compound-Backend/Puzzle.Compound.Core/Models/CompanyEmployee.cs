using System;
using System.Collections.Generic;

#nullable disable

namespace Puzzle.Compound.Core.Models
{
    public partial class CompanyEmployee
    {
        public Guid CompanyEmployeeId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Guid CompanyId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
    }
}
