using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Common.Enums
{
    public enum PrintCardStatus
    {
        Request = 1,
        BeingPrinted,
        Printed,
        PrintCanceled,
        Rejected,
        CardCanceled,
    }
}
