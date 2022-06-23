using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories {
	public class ServiceTypeRepository : RepositoryBase<ServiceType>, IServiceTypeRepository {
		public ServiceTypeRepository(CompoundDbContext dbContext) : base(dbContext) {

		}
	}

	public interface IServiceTypeRepository : IRepository<ServiceType> {

	}
}
