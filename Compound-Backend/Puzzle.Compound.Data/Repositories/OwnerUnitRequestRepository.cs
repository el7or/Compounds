using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class OwnerUnitRequestRepository : RepositoryBase<Core.Models.OwnerAssignUnitRequest>, IOwnerUnitRequestRepository
    {
        public OwnerUnitRequestRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IOwnerUnitRequestRepository : IRepository<Core.Models.OwnerAssignUnitRequest>
    {
    }
}
