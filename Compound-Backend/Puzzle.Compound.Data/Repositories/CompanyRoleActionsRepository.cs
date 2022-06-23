using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class CompanyRoleActionsRepository : RepositoryBase<ActionsInCompanyRoles>, ICompanyRoleActionsRepository
    {
        public CompanyRoleActionsRepository(CompoundDbContext dbContext) : base(dbContext)
        {
        }
    }

    public interface ICompanyRoleActionsRepository : IRepository<ActionsInCompanyRoles>
    {
    }
}
