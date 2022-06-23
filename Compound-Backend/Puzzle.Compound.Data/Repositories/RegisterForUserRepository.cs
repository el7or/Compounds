using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class RegistrationForUserRepository : RepositoryBase<RegistrationForUser>, IRegistrationForUserRepository
    {
        public RegistrationForUserRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface IRegistrationForUserRepository : IRepository<RegistrationForUser>
    {
    }
}