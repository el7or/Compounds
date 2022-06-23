using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{
    public class IssueRequestRepository : RepositoryBase<IssueRequest>, IIssueRequestRepository
    {
        public IssueRequestRepository(CompoundDbContext context) : base(context)
        {

        }
    }

    public interface IIssueRequestRepository : IRepository<IssueRequest>
    {

    }
}
