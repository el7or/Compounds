using Puzzle.Compound.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.PrintCardRequest
{
    public class PrintCardUpdateStatusInputViewModel
    {
        public Guid Id { get; set; }
        public PrintCardStatus Status { get; set; }
    }
}
