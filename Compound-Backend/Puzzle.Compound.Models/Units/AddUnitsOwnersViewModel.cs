using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Units
{
    public class AddUnitsOwnersViewModel
    {
        public Guid CompoundId { get; set; }
        public IEnumerable<UnitOwnerImportViewModel> UnitsOwners { get; set; }
        public string ConnectionId { get; set; }
    }

    public class UnitOwnerImportViewModel
    {
        public string Group { get; set; }
        public string Unit { get; set; }
        public int UnitType { get; set; }
        public string OwnerName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}
