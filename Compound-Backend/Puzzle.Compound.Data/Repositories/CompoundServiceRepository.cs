using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories {
	public class CompoundServiceRepository : RepositoryBase<CompoundService>, ICompoundServiceRepository {
		public CompoundServiceRepository(CompoundDbContext context) : base(context) {

		}
	}

	public interface ICompoundServiceRepository : IRepository<CompoundService> {

	}
}
