using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Units
{
    public class OwnerUnitRequestViewModel
    {
        public IEnumerable<AddUnitRequestViewModel> Units { get; set; }
        public Guid OwnerRegistrationId { get; set; }
    }
}
