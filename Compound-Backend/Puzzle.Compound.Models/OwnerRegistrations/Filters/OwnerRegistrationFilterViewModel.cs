using Puzzle.Compound.Common.Models;

namespace Puzzle.Compound.Models.OwnerRegistrations.Filters
{
    public class OwnerRegistrationFilterViewModel : PagedInput
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public int UserType { get; set; }
        public bool? UserConfirmed { get; set; }
        public string Companies { get; set; }
        public string Compounds { get; set; }
    }
}
