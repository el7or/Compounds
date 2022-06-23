using Newtonsoft.Json.Linq;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Extensions;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.Data.Repositories
{
    public class OwnerRegistrationRepository : RepositoryBase<OwnerRegistration>, IOwnerRegistrationRepository
    {
        private readonly CompoundDbContext dbContext;

        public OwnerRegistrationRepository(CompoundDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<PagedOutput<OwnerRegistrationFullInfo>> GetOwnerRegistrationsAsync(string companies, string compounds, string phone, string name, bool? userConfirmed, int userType, int? pageNumber, int? pageSize)
        {
            object userConfirmedVal = DBNull.Value;
            if (userConfirmed.HasValue)
            {
                userConfirmedVal = userConfirmed.Value ? 1 : 0;
            }

            var ownerRegistrations = await dbContext.LoadStoredProc("sp_getRegisteredOwners")
                                    .WithSqlParam("@companies", companies)
                                    .WithSqlParam("@compounds", compounds)
                                    .WithSqlParam("@phone", phone)
                                    .WithSqlParam("@name", name)
                                    .WithSqlParam("@userConfirmed", userConfirmedVal)
                                    .WithSqlParam("@userType", userType)
                                    .ExecuteStoredProc<OwnerRegistrationFullInfo>();
            if (pageNumber != null && pageSize != null)
            {
                return new PagedOutput<OwnerRegistrationFullInfo>()
                {
                    Result = ownerRegistrations.AsQueryable().ApplyPaging(new PagedInput { PageNumber = (int)pageNumber, PageSize = (int)pageSize }).ToList(),
                    TotalCount = ownerRegistrations.Count
                };
            }
            else
            {
                return new PagedOutput<OwnerRegistrationFullInfo>()
                {
                    Result = ownerRegistrations,
                    TotalCount = ownerRegistrations.Count
                };
            }
        }
    }

    public interface IOwnerRegistrationRepository : IRepository<OwnerRegistration>
    {
        Task<PagedOutput<OwnerRegistrationFullInfo>> GetOwnerRegistrationsAsync(string companies, string compounds, string phone, string name, bool? userConfirmed, int userType, int? pageNumber, int? pageSize);
    }
}
