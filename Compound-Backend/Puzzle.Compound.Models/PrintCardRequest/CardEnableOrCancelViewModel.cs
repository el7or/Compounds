using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.PrintCardRequest
{
    public class CardEnableOrCancelViewModel
    {
        public Guid Id { get; set; }
        public bool IsCancel { get; set; }
    }
}
