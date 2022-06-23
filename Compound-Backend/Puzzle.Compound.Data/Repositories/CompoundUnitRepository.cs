using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class CompoundUnitRepository : RepositoryBase<Core.Models.CompoundUnit>, ICompoundUnitRepository
    {
        public CompoundUnitRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ICompoundUnitRepository : IRepository<Core.Models.CompoundUnit>
    {
    }
}
