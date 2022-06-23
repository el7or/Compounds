using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories {
	public class CompoundIssueRepository : RepositoryBase<CompoundIssue>, ICompoundIssueRepository {
		public CompoundIssueRepository(CompoundDbContext context) : base(context) {

		}
	}

	public interface ICompoundIssueRepository : IRepository<CompoundIssue> {

	}
}
