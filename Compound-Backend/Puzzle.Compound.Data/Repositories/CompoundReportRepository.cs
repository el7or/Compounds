using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories {
	public class CompoundReportRepository : RepositoryBase<CompoundReport>, ICompoundReportRepository {
		public CompoundReportRepository(CompoundDbContext dbContext) : base(dbContext) {

		}
	}

	public interface ICompoundReportRepository : IRepository<CompoundReport> {

	}

}
