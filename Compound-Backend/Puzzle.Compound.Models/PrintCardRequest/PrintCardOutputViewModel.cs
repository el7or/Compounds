using Puzzle.Compound.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.PrintCardRequest
{
    public class PrintCardOutputViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string Picture { get; set; }
        public PrintCardStatus Stauts { get; set; }
        public Guid OwnerRegisterationId { get; set; }
        public Guid CompoundUnitId { get; set; }
        public string OwnerName { get; set; }
        public string UnitName { get; set; }
        public string QrCode { get; set; }
        public string Code { get; set; }
    }
}
