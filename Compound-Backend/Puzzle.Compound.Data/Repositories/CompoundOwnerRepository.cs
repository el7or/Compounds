using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class CompoundOwnerRepository : RepositoryBase<Core.Models.CompoundOwner>, ICompoundOwnerRepository
    {
        public CompoundOwnerRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ICompoundOwnerRepository : IRepository<Core.Models.CompoundOwner>
    {
    }
}
