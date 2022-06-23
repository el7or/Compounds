using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class IssueTypeRepository : RepositoryBase<IssueType>, IIssueTypeRepository
    {
        public IssueTypeRepository(CompoundDbContext dbContext) : base(dbContext)
        {

        }
    }

    public interface IIssueTypeRepository : IRepository<IssueType>
    {

    }
}
