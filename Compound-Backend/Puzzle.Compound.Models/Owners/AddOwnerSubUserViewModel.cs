using Microsoft.AspNetCore.Http;
using Puzzle.Compound.Models.Units;
using System;
using System.Collections.Generic;

namespace Puzzle.Compound.Models.Owners
{
    public class AddOwnerSubUserViewModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CompanyId { get; set; }       
        public int UserType { get; set; }  // 2 => subowner , 3 => tenant
        public Guid? MainRegistrationId { get; set; }
        public IFormFile Image { get; set; }
        public List<SubUserAssignUnitViewModel> Units { get; set; }
        public string Gender { get; set; }
        public string WhatsAppNum { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
