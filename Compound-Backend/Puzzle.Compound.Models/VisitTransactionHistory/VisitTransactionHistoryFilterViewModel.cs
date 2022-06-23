using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Models;
using System;

namespace Puzzle.Compound.Models.VisitTransactionHistory
{
   public class VisitTransactionHistoryFilterViewModel : PagedInput
    {
        public Guid? CompoundId { get; set; }
        public Guid? OwnerRegistrationId { get; set; }
        public Guid? GateId { get; set; }
        public Guid[] CompoundUnitsIds { get; set; }
        public VisitStatus? Status { get; set; }
        public DateTime? Date { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public bool? WithFilterLists { get; set; }
    }
}
