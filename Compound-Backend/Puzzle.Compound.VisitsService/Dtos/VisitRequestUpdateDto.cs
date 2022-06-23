using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.VisitsService.Dtos
{
    public class VisitRequestUpdateDto : VisitRequestDto
    {
        public Guid Id { get; set; }
    }
}
