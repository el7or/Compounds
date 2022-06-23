using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class SystemPageRepository : RepositoryBase<SystemPage>, ISystemPageRepository
    {
        public SystemPageRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ISystemPageRepository : IRepository<SystemPage>
    {
    }
}
