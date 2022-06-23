using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
    }
}
