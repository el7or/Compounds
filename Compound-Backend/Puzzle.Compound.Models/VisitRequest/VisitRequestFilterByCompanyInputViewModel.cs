﻿using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.VisitRequest
{
    public class VisitRequestFilterByCompanyInputViewModel : PagedInput
    {
        public Guid Id { get; set; }
    }
}
