using System;
using System.ComponentModel.DataAnnotations;

namespace Puzzle.Compound.Models.Owners
{
    public class OwnerLoginViewModel
    {
        [Required]
        public string Phone { get; set; }
        public string CompanyId { get; set; } = null;
    }
}
