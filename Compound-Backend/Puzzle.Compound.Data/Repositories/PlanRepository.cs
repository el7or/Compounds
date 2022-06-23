using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class PlanRepository : RepositoryBase<Plan>, IPlanRepository
    {
        public PlanRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IPlanRepository : IRepository<Plan>
    {
    }
}
