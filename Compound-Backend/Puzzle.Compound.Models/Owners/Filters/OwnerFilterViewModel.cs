using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Owners.Filters
{
    public class OwnerFilterViewModel : PagedInput
    {
        public Guid? CompoundId { get; set; }
        public Guid? CompanyId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string SearchText { get; set; }
    }
}
