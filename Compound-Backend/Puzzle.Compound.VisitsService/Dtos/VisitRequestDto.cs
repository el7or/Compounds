using Microsoft.AspNetCore.Http;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Models.VisitRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.VisitsService.Dtos
{
    public class VisitRequestDto
    {
        public string VisitorName { get; set; }
        public string Details { get; set; }
        public string CarNo { get; set; }
        public VisitType Type { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int GroupNo { get; set; }
        public List<Day> Days { get; set; }
        public Guid CompoundUnitId { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}
