using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.VisitsService.Dtos
{
    public class PrintCardAddDto
    {
        [Required]
        public string Name { get; set; }
        public string Details { get; set; }
        [Required]
        public IFormFile Picture { get; set; }
        [Required]
        public Guid CompoundUnitId { get; set; }
    }
}
