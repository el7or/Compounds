using Puzzle.Compound.Core.Extensions;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puzzle.Compound.Data.Repositories
{
    public class OwnerUnitRepository : RepositoryBase<OwnerUnit>, IOwnerUnitRepository
    {
        private readonly CompoundDbContext dbContext;

        public OwnerUnitRepository(CompoundDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<MainOwnerUnitInfo>> GetOwnerUnits(Guid? ownerRegistrationId, Guid? unitId)
        {
            return await dbContext.LoadStoredProc("sp_checkOwnerUnits")
                                    .WithSqlParam("@ownerRegistrationId", ownerRegistrationId)
                                    .WithSqlParam("@unitId", unitId)
                                    .ExecuteStoredProc<MainOwnerUnitInfo>();
        }

        public async Task<List<MainOwnerUnitInfo>> GetMainOwnerUnits(Guid? ownerRegistrationId, Guid? unitId)
        {
            return await dbContext.LoadStoredProc("sp_checkMainOwnerUnits")
                                    .WithSqlParam("@ownerRegistrationId", ownerRegistrationId)
                                    .WithSqlParam("@unitId", unitId)
                                    .ExecuteStoredProc<MainOwnerUnitInfo>();
        }

        public async Task<List<OwnerUnitInfo>> GetSubOwnerUnits(Guid? mainOwnerRegistrationId, Guid? subOwnerRegistrationId, Guid? unitId)
        {
            return await dbContext.LoadStoredProc("sp_checkSubOwnerUnits")
                                    .WithSqlParam("@mainOwnerRegistrationId", mainOwnerRegistrationId)
                                    .WithSqlParam("@subOwnerRegistrationId", subOwnerRegistrationId)
                                    .WithSqlParam("@unitId", unitId)
                                    .ExecuteStoredProc<OwnerUnitInfo>();
        }
    }

    public interface IOwnerUnitRepository : IRepository<OwnerUnit>
    {
        Task<List<MainOwnerUnitInfo>> GetOwnerUnits(Guid? ownerRegistrationId = null, Guid? unitId= null);
        Task<List<MainOwnerUnitInfo>> GetMainOwnerUnits(Guid? ownerRegistrationId = null, Guid? unitId = null);
        Task<List<OwnerUnitInfo>> GetSubOwnerUnits(Guid? mainOwnerRegistrationId, Guid? subOwnerRegistrationId, Guid? unitId);
    }
}
