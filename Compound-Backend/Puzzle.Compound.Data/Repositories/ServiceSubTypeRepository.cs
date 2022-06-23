using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Data.Repositories
{
    public class ServiceSubTypeRepository : RepositoryBase<ServiceSubType>, IServiceSubTypeRepository
    {
        public ServiceSubTypeRepository(CompoundDbContext dbContext) : base(dbContext)
        {

        }
    }
    public interface IServiceSubTypeRepository : IRepository<ServiceSubType>
    {

    }
}
