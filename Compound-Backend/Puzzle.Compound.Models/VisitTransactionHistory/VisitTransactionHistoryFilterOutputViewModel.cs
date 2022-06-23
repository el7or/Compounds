using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.VisitTransactionHistory
{
    public class VisitTransactionHistoryFilterOutputViewModel
    {
        public Guid? VisitTransactionHistoryId { get; set; }
        public Guid VisitRequestId { get; set; }
        public Guid OwnerRegistrationId { get; set; }
        public DateTime? Date { get; set; }
        public string GateName { get; set; }
        public string OwnerName { get; set; }
        public string UnitName { get; set; }
        public VisitType Type { get; set; }
        public VisitStatus Status { get; set; }
        public string VisitorName { get; set; }
    }
    public class VisitTransactionHistoryPagedOutput : PagedOutput<VisitTransactionHistoryFilterOutputViewModel>
    {
        public List<VisitsCompoundOwnersViewModel> CompoundOwners { get; set; }
        public List<VisitsCompoundUnitsViewModel> CompoundUnits { get; set; }
        public List<VisitsCompoundGatesViewModel> CompoundGates{ get; set; }
    }

    public class VisitsCompoundOwnersViewModel
    {
        public Guid OwnerRegistrationId { get; set; }
        public string Name { get; set; }
    }

    public class VisitsCompoundUnitsViewModel
    {
        public Guid CompoundUnitId { get; set; }
        public string Name { get; set; }
    }

    public class VisitsCompoundGatesViewModel
    {
        public Guid GateId { get; set; }
        public string GateName { get; set; }
    }

    public class VisitTransactionHistoryFilterByUserOutputViewModel
    {
        public DateTime DateTime { get; set; }
        public string GateName { get; set; }
        public string GateType { get; set; }
        public string UserName { get; set; }
        public string VisitorName { get; set; }
    }
}
