using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ICompanyRepository : IRepository<Company>
    {
    }
}
