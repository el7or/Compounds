using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class SystemPageActionRepository : RepositoryBase<SystemPageAction>, ISystemPageActionRepository
    {
        public SystemPageActionRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ISystemPageActionRepository : IRepository<SystemPageAction>
    {
    }
}
