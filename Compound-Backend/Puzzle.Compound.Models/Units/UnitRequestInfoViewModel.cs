using System;

namespace Puzzle.Compound.Models.Units
{
    public class UnitRequestInfoViewModel: UnitInfoViewModel
    {
        public DateTime RequestDate { get; set; }
        public bool? UserConfirmed { get; set; }
        public int Status { get; set; }
        public Guid OwnerRegistrationId { get; set; }
    }

    public class AddUnitRequestViewModel
    {
        public Guid CompanyId { get; set; }
        public Guid CompoundUnitId { get; set; }
    }
}
