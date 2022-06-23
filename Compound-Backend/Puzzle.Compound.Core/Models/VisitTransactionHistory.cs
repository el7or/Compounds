using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
    public class VisitTransactionHistory
    {
        public Guid VisitTransactionHistoryId { get; set; }
        public Guid VisitRequestId { get; set; }
        public Guid GateId { get; set; }
        public DateTime Date { get; set; }
        public Guid? CompanyUserId { get; set; }

        public virtual VisitRequest VisitRequest { get; set; }
        public virtual Gate Gate { get; set; }
        public virtual CompanyUser CompanyUser { get; set; }
    }
}
