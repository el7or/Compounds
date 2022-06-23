using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class OwnerAssignedUnitRepository : RepositoryBase<OwnerAssignedUnit>, IOwnerAssignedUnitRepository
    {
        public OwnerAssignedUnitRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IOwnerAssignedUnitRepository : IRepository<OwnerAssignedUnit>
    {
    }
}
