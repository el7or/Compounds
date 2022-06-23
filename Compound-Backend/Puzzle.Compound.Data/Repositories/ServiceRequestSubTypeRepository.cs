using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Data.Repositories
{
    public class ServiceRequestSubTypeRepository : RepositoryBase<ServiceRequestSubType>, IServiceRequestSubTypeRepository
    {
        public ServiceRequestSubTypeRepository(CompoundDbContext dbContext) : base(dbContext)
        {

        }
    }
    public interface IServiceRequestSubTypeRepository : IRepository<ServiceRequestSubType>
    {

    }
}
