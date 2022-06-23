using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.CompanyUsers
{
    public class CompanyUserValidationViewModel
    {
        public Guid CompanyId { get; set; }
        public Guid CompanyUserId { get; set; }
        public string ActionName { get; set; }


    }
}
