using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class CompoundUnitTypeRepository : RepositoryBase<Core.Models.CompoundUnitType>, ICompoundUnitTypeRepository
    {
        public CompoundUnitTypeRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ICompoundUnitTypeRepository : IRepository<Core.Models.CompoundUnitType>
    {
    }
}
