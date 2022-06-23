using Puzzle.Compound.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Core.Models
{
    public class PrintCardRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string Picture { get; set; }
        public PrintCardStatus Status { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Guid OwnerRegisterationId { get; set; }
        public Guid CompoundUnitId { get; set; }
        public Guid? VisitRequestId { get; set; }
        public virtual OwnerRegistration OwnerRegistration { get; set; }
        public virtual CompoundUnit CompoundUnit { get; set; }
        public virtual VisitRequest VisitRequest { get; set; }

    }
}
