using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class CompanyRoleRepository : RepositoryBase<CompanyRole>, ICompanyRoleRepository
    {
        public CompanyRoleRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ICompanyRoleRepository : IRepository<CompanyRole>
    {
    }
}
