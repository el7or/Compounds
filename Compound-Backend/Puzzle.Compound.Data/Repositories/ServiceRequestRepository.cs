using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories {
	public class ServiceRequestRepository : RepositoryBase<ServiceRequest>, IServiceRequestRepository {
		public ServiceRequestRepository(CompoundDbContext context) : base(context) {

		}
	}

	public interface IServiceRequestRepository : IRepository<ServiceRequest> {

	}
}
