using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories {
	public class ReportTypeRepository : RepositoryBase<ReportType>, IReportTypeRepository {
		public ReportTypeRepository(CompoundDbContext dbContext) : base(dbContext) {

		}
	}

	public interface IReportTypeRepository : IRepository<ReportType> {

	}

}
