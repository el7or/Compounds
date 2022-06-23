using Puzzle.Compound.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Compounds
{
    public class AllPendingsFilterViewModel : PagedInput
    {
        public Guid[] CompoundsIds { get; set; }
        public bool? IsShowUsers { get; set; }
        public bool? IsShowVisits { get; set; }
        public bool? IsShowServices { get; set; }
        public bool? IsShowIssues { get; set; }
    }

    public class PendingOutputViewModel
    {
        public Guid Id { get; set; }
        public int PendingType { get; set; }
        public Guid CompoundId { get; set; }
        public string CompoundNameEn { get; set; }
        public string CompoundNameAr { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByName { get; set; }
        public string OwnerRegistrationPhone { get; set; }
    }

    public enum PendingType
    {
        User = 1,
        Visit,
        Service,
        Issue
    }
}
