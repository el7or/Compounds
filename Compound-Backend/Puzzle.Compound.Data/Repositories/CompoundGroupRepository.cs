using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class CompoundGroupRepository : RepositoryBase<Core.Models.CompoundGroup>, ICompoundGroupRepository
    {
        public CompoundGroupRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ICompoundGroupRepository : IRepository<Core.Models.CompoundGroup>
    {
    }
}
