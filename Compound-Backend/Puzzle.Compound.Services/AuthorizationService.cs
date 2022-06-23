using Puzzle.Compound.Core.Extensions;
using Puzzle.Compound.Core.Models;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface IAuthorizationService
    {
        Task<bool> Validate(string companyId, string companyUserId, string actionName);
    }

    public class AuthorizationService : IAuthorizationService
    {
        private readonly CompoundDbContext dbContext;

        public AuthorizationService(CompoundDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<bool> Validate(string companyId, string companyUserId, string actionName)
        {
            var result = await dbContext.LoadStoredProc("sp_validateCompanyUserAuth")
                                    .WithSqlParam("@companyId", companyId)
                                    .WithSqlParam("@companyUserId", companyUserId)
                                    .WithSqlParam("@actionName", actionName)
                                    .ExecuteStoredProcedure<string>();

            return bool.Parse(result);
        }
    }
}
