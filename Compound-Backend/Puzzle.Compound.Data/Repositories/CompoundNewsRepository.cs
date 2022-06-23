using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{    public class CompoundNewsRepository : RepositoryBase<CompoundNews>, ICompoundNewsRepository
    {
        public CompoundNewsRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ICompoundNewsRepository : IRepository<CompoundNews>
    {
    }
}
