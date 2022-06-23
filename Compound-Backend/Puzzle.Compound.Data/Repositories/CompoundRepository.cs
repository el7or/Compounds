using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Extensions;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.Compounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.Data.Repositories
{
    public class CompoundRepository : RepositoryBase<Core.Models.Compound>, ICompoundRepository
    {
        private readonly CompoundDbContext dbContext;
        public CompoundRepository(CompoundDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<CompoundViewModel>> GetUserCompoundsAsync(Guid companyUserId)
        {
            var compounds = await dbContext.LoadStoredProc("sp_getUserCompounds")
                                    .WithSqlParam("@companyUserId", companyUserId)
                                    .ExecuteStoredProc<CompoundViewModel>();
            //foreach (var compound in compounds)
            //{
            //    compound.ServicesCount = dbContext.CompoundServices
            //        .Count(s => s.CompoundId == compound.CompoundId && s.IsActive == true && s.IsDeleted == false);
            //    compound.UnitsCount = dbContext.CompoundUnits
            //        .Include(c => c.CompoundGroup)
            //        .Count(u => u.CompoundGroup.CompoundId == compound.CompoundId && u.IsActive == true && u.IsDeleted == false);
            //    compound.OwnersCount = dbContext.CompoundOwners
            //        .Include(ou => ou.OwnerUnits)
            //        .ThenInclude(u => u.CompoundUnit)
            //        .ThenInclude(g => g.CompoundGroup)
            //        .Count(o => o.OwnerUnits.Any(g => g.CompoundUnit.CompoundGroup.CompoundId == compound.CompoundId)
            //                            && o.IsActive == true  && o.IsDeleted == false);
            //}
            return compounds;
        }

        public async Task<DashboardInfoViewModel> GetDashboardInfoAsync(DashboardFilterViewModel model)
        {
            var allUsers = await dbContext.LoadStoredProc("sp_getRegisteredOwners")
                .WithSqlParam("@companies", null)
                .WithSqlParam("@compounds", model.CompoundId.ToString())
                .WithSqlParam("@phone", null)
                .WithSqlParam("@name", null)
                .WithSqlParam("@userConfirmed", null)
                .WithSqlParam("@userType", 1)
                .ExecuteStoredProc<OwnerRegistrationFullInfo>();
            var allVisits = dbContext.VisitRequests
                .Where(v => v.CompoundId == model.CompoundId)
                .Include(v => v.VisitTransactionHistories)
                .AsQueryable();
            var allServices = dbContext.ServiceRequests
                .Where(v => v.CompoundId == model.CompoundId && model.ServiceTypesIds.Contains(v.ServiceTypeId))
                .AsQueryable();
            var allIssues = dbContext.IssueRequests
                .Where(v => v.CompoundId == model.CompoundId && model.IssueTypesIds.Contains(v.IssueTypeId))
                .AsQueryable();
            DashboardInfoViewModel info = new DashboardInfoViewModel
            {
                VisitsHistoryInScope = dbContext.VisitTransactionHistory.Include(v => v.VisitRequest)
                .Where(v => v.VisitRequest.CompoundId == model.CompoundId)
                .AsEnumerable().GroupBy(v => new { v.Date.Year, v.Date.Month, v.Date.Day })
                        .Select(k => new VisitsHistoryInScope
                        {
                            Scope = k.Key.Year + "/" + k.Key.Month + "/" + k.Key.Day,
                            TypeCounts = new int[7]
                            {
                                k.Count(t => t.VisitRequest.VisitType == VisitType.None),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Once),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Periodic),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Labor),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Group),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Taxi),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Delivery),
                            }
                        }).OrderBy(m => m.Scope).Take(30).ToList(),
                ServicesTypesCounts = dbContext.ServiceRequests.Include(v => v.ServiceType)
                .Where(s => s.CompoundId == model.CompoundId && model.ServiceTypesIds.Contains(s.ServiceTypeId))
                .AsEnumerable().GroupBy(s => new { s.ServiceType })
                .Select(k => new ServiceTypeStatuses
                {
                    TypeArabicName = k.Key.ServiceType.ArabicName,
                    TypeEnglishName = k.Key.ServiceType.EnglishName,
                    StatusCounts = new int[3]
                    {
                        k.Count(t => t.Status == 0),
                        k.Count(t => t.Status == 1),
                        k.Count(t => t.Status == 2),
                    },
                    RatesCounts = new int[5]
                    {
                        k.Count(t => t.Rate == 1),
                        k.Count(t => t.Rate == 2),
                        k.Count(t => t.Rate == 3),
                        k.Count(t => t.Rate == 4),
                        k.Count(t => t.Rate == 5)
                    }
                }).ToList(),
                ServicesSubTypesCounts = dbContext.CompoundServices
                .Include(t => t.ServiceType).ThenInclude(r => r.ServiceRequests)
                .Include(s => s.ServiceSubTypes).ThenInclude(s => s.ServiceRequestSubTypes)
                .Where(s => s.CompoundId == model.CompoundId && model.ServiceTypesIds.Contains(s.ServiceTypeId) && s.ServiceType.ServiceRequests.Count > 0)
                .Select(c => new ServiceTypeSubType
                {
                    TypeEnglishName = c.ServiceType.EnglishName,
                    TypeArabicName = c.ServiceType.ArabicName,
                    ServiceSubTypesCounts = c.ServiceSubTypes.Where(s => s.ServiceRequestSubTypes.Count > 0)
                    .Select(s => new ServiceSubTypeCount
                    {
                        SubTypeEnglishName = s.EnglishName,
                        SubTypeArabicName = s.ArabicName,
                        SubTypeCount = s.ServiceRequestSubTypes.Count()
                    }).ToList()
                }).ToList(),
                IssuesTypesCounts = dbContext.IssueRequests.Include(v => v.IssueType)
                .Where(s => s.CompoundId == model.CompoundId && model.IssueTypesIds.Contains(s.IssueTypeId))
                .AsEnumerable().GroupBy(s => new { s.IssueType })
                .Select(k => new IssueTypeStatuses
                {
                    TypeArabicName = k.Key.IssueType.ArabicName,
                    TypeEnglishName = k.Key.IssueType.EnglishName,
                    StatusCounts = new int[3]
                    {
                        k.Count(t => t.Status == 0),
                        k.Count(t => t.Status == 1),
                        k.Count(t => t.Status == 2),
                    }
                }).ToList(),
                AllUsersCount = allUsers.Count(),
                PendingUsersCount = allUsers.Where(u => u.UserConfirmed != true).Count(),
                AllVisitsCount = allVisits.SelectMany(v => v.VisitTransactionHistories).Count(),
                PendingVisitsCount = allVisits.Where(x => x.IsConsumed != true && x.IsConfirmed == null && x.IsCanceled != true
                                                    && x.DateTo.HasValue && x.DateTo.Value.Date >= DateTime.Now.Date).Count(),
                AllServicesCount = allServices.Count(),
                PendingServicesCount = await allServices.Where(s => s.Status == 0).CountAsync(),
                AllIssuesCount = allIssues.Count(),
                PendingIssuesCount = await allIssues.Where(s => s.Status == 0).CountAsync(),
            };
            return info;
        }

        public IEnumerable<VisitsHistoryInScope> GetChartVisitsInfo(Guid compoundId, ChartScope chartScope)
        {
            var visitsHistory = dbContext.VisitTransactionHistory.Include(v => v.VisitRequest)
                .Where(v => v.VisitRequest.CompoundId == compoundId)
                .AsEnumerable();
            var visitsHistoryInScope = new List<VisitsHistoryInScope>();
            switch (chartScope)
            {
                case ChartScope.Year:
                    visitsHistoryInScope = visitsHistory.GroupBy(v => new { v.Date.Year })
                        .Select(k => new VisitsHistoryInScope
                        {
                            Scope = k.Key.Year.ToString(),
                            TypeCounts = new int[7]
                            {
                                k.Count(t => t.VisitRequest.VisitType == VisitType.None),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Once),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Periodic),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Labor),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Group),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Taxi),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Delivery),
                            }
                        }).OrderBy(m => m.Scope).Take(5).ToList();
                    break;
                case ChartScope.Month:
                    visitsHistoryInScope = visitsHistory.GroupBy(v => new { v.Date.Year, v.Date.Month })
                        .Select(k => new VisitsHistoryInScope
                        {
                            Scope = k.Key.Year + "/" + k.Key.Month,
                            TypeCounts = new int[7]
                            {
                                k.Count(t => t.VisitRequest.VisitType == VisitType.None),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Once),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Periodic),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Labor),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Group),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Taxi),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Delivery),
                            }
                        }).OrderBy(m => m.Scope).Take(12).ToList();
                    break;
                case ChartScope.Day:
                    visitsHistoryInScope = visitsHistory.GroupBy(v => new { v.Date.Year, v.Date.Month, v.Date.Day })
                        .Select(k => new VisitsHistoryInScope
                        {
                            Scope = k.Key.Year + "/" + k.Key.Month + "/" + k.Key.Day,
                            TypeCounts = new int[7]
                            {
                                k.Count(t => t.VisitRequest.VisitType == VisitType.None),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Once),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Periodic),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Labor),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Group),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Taxi),
                                k.Count(t => t.VisitRequest.VisitType == VisitType.Delivery),
                            }
                        }).OrderBy(m => m.Scope).Take(30).ToList();
                    break;
            }

            return visitsHistoryInScope;
        }

        public async Task<PagedOutput<PendingOutputViewModel>> GetAllPendingsAsync(AllPendingsFilterViewModel model)
        {
            var pendingVisits = model.IsShowVisits != true ? null : dbContext.VisitRequests
                .Where(x => model.CompoundsIds.Contains(x.CompoundId)
                && x.IsConsumed != true && x.IsConfirmed == null && x.IsCanceled != true
                && x.DateTo.HasValue && x.DateTo.Value.Date >= DateTime.Now.Date)
                .Include(c => c.Compound)
                .Select(v => new PendingOutputViewModel
                {
                    Id = v.VisitRequestId,
                    PendingType = (int)PendingType.Visit,
                    CompoundId = v.CompoundId,
                    CompoundNameEn = v.Compound.NameEn,
                    CompoundNameAr = v.Compound.NameAr,
                    CreatedDate = v.CreationDate,
                    CreatedByName = v.VisitorName
                }).ToList();

            var pendingServices = model.IsShowServices != true ? null : dbContext.ServiceRequests
                .Where(v => model.CompoundsIds.Contains(v.CompoundId) && v.Status == 0)
                .Include(o => o.OwnerRegistration)
                .Include(o => o.Compound)
                .Select(s => new PendingOutputViewModel
                {
                    Id = s.ServiceRequestId,
                    PendingType = (int)PendingType.Service,
                    CompoundId = s.CompoundId,
                    CompoundNameEn = s.Compound.NameEn,
                    CompoundNameAr = s.Compound.NameAr,
                    CreatedDate = s.Date,
                    CreatedByName = s.OwnerRegistration.Name
                }).ToList();

            var pendingIssues = model.IsShowIssues != true ? null : dbContext.IssueRequests
                .Where(v => model.CompoundsIds.Contains(v.CompoundId) && v.Status == 0)
                .Include(o => o.OwnerRegistration)
                .Include(o => o.Compound)
                .Select(i => new PendingOutputViewModel
                {
                    Id = i.IssueRequestId,
                    PendingType = (int)PendingType.Issue,
                    CompoundId = i.CompoundId,
                    CompoundNameEn = i.Compound.NameEn,
                    CompoundNameAr = i.Compound.NameAr,
                    CreatedDate = i.PostTime,
                    CreatedByName = i.OwnerRegistration.Name
                }).ToList();

            var pendingUsers = model.IsShowUsers != true ? null : await dbContext.LoadStoredProc("sp_getRegisteredOwners")
                .WithSqlParam("@companies", null)
                .WithSqlParam("@compounds", model.CompoundsIds.ToString())
                .WithSqlParam("@phone", null)
                .WithSqlParam("@name", null)
                .WithSqlParam("@userConfirmed", 0)
                .WithSqlParam("@userType", 1)
                .ExecuteStoredProc<OwnerRegistrationFullInfo>();

            var allPendings = new List<PendingOutputViewModel>();

            if (pendingVisits != null)
                allPendings = allPendings.Union(pendingVisits).ToList();

            if (pendingServices != null)
                allPendings = allPendings.Union(pendingServices).ToList();

            if (pendingIssues != null)
                allPendings = allPendings.Union(pendingIssues).ToList();

            if (pendingUsers != null)
                allPendings = allPendings.Union(pendingUsers.Select(u => new PendingOutputViewModel
                {
                    Id = u.OwnerRegistrationId,
                    PendingType = (int)PendingType.User,
                    CompoundId = u.CompoundId,
                    CompoundNameEn = u.CompoundNameEn,
                    CompoundNameAr = u.CompoundNameAr,
                    CreatedDate = u.RegisterDate.Value,
                    CreatedByName = u.Name,
                    OwnerRegistrationPhone = u.Phone
                })).ToList();

            allPendings = allPendings.OrderByDescending(p => p.CreatedDate).ToList();

            return new PagedOutput<PendingOutputViewModel>()
            {
                TotalCount = allPendings.Count(),
                Result = allPendings.AsQueryable().ApplyPaging(model).ToList()
            };
        }

        public int GetAllPendingsCount(AllPendingsFilterViewModel model)
        {
            var pendingVisitsCount = model.IsShowVisits != true ? 0 : dbContext.VisitRequests
                .Where(x => model.CompoundsIds.Contains(x.CompoundId)
                && x.IsConsumed != true && x.IsConfirmed == null && x.IsCanceled != true
                && x.DateTo.HasValue && x.DateTo.Value.Date >= DateTime.Now.Date)
                .Count();

            var pendingServicesCount = model.IsShowServices != true ? 0 : dbContext.ServiceRequests
                .Where(v => model.CompoundsIds.Contains(v.CompoundId) && v.Status == 0)
                .Count();

            var pendingIssuesCount = model.IsShowIssues != true ? 0 : dbContext.IssueRequests
                .Where(v => model.CompoundsIds.Contains(v.CompoundId) && v.Status == 0)
                .Count();

            var pendingUsersCount = model.IsShowUsers != true ? 0 : dbContext.LoadStoredProc("sp_getRegisteredOwners")
                .WithSqlParam("@companies", null)
                .WithSqlParam("@compounds", model.CompoundsIds.ToString())
                .WithSqlParam("@phone", null)
                .WithSqlParam("@name", null)
                .WithSqlParam("@userConfirmed", 0)
                .WithSqlParam("@userType", 1)
                .ExecuteStoredProc<OwnerRegistrationFullInfo>().Result.Count();

            return pendingVisitsCount + pendingServicesCount + pendingIssuesCount + pendingUsersCount;            
        }
    }

    public interface ICompoundRepository : IRepository<Core.Models.Compound>
    {
        Task<IEnumerable<CompoundViewModel>> GetUserCompoundsAsync(Guid companyUserId);
        Task<DashboardInfoViewModel> GetDashboardInfoAsync(DashboardFilterViewModel model);
        IEnumerable<VisitsHistoryInScope> GetChartVisitsInfo(Guid compoundId, ChartScope chartScope);
        Task<PagedOutput<PendingOutputViewModel>> GetAllPendingsAsync(AllPendingsFilterViewModel model);
        int GetAllPendingsCount(AllPendingsFilterViewModel model);
    }
}
