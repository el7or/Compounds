using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Owners;
using Puzzle.Compound.Models.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.OwnerRegistrations
{
    public class OwnerApprovalInfoViewModel
    {
        public OwnerRegistrationApproveViewModel OwnerRegistration { get; set; }
        public OwnerApproveViewModel Owner { get; set; }

    }

    public class OwnerRegistrationApproveViewModel
    {
        public OwnerInfoViewModel Info { get; set; }
        public List<UnitRequestInfoMap> Units { get; set; } = new List<UnitRequestInfoMap>();
    }

    public class OwnerApproveViewModel
    {
        public OwnerInfoViewModel Info { get; set; }
        public List<UnitInfoViewModel> Units { get; set; } = new List<UnitInfoViewModel>();
    }
}
