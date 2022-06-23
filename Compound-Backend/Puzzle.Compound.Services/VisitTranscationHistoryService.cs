using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.VisitTransactionHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface IVisitTranscationHistoryService
    {
        Task<VisitTransactionHistoryPagedOutput> GetAsync(VisitTransactionHistoryFilterViewModel model);
        Task<VisitTransactionHistoryPagedOutput> GetPendingVisitsAsync(VisitTransactionHistoryFilterViewModel model);
        public Task<PagedOutput<VisitTransactionHistoryFilterByUserOutputViewModel>> FilterByUserCardAsync(PagedInput model);
    }
    public class VisitTranscationHistoryService : BaseService, IVisitTranscationHistoryService
    {
        private readonly UserIdentity _user;
        private readonly IVisitTransactionHistoryRepository _visitTransactionHistoryRepository;
        private readonly IVisitorRequestRepository _visitRequestRepository;
        public VisitTranscationHistoryService(IUnitOfWork unitOfWork, IMapper mapper, UserIdentity user, IVisitTransactionHistoryRepository visitTransactionHistoryRepository, IVisitorRequestRepository visitRequestRepository) : base(unitOfWork, mapper)
        {
            _user = user;
            _visitTransactionHistoryRepository = visitTransactionHistoryRepository;
            _visitRequestRepository = visitRequestRepository;
        }

        public async Task<VisitTransactionHistoryPagedOutput> GetAsync(VisitTransactionHistoryFilterViewModel model)
        {
            var result = new VisitTransactionHistoryPagedOutput();

            var query = _visitTransactionHistoryRepository.Table
                .Include(v => v.Gate)
                .Include(v => v.VisitRequest)
                    .ThenInclude(v => v.CompoundUnit)
                        .ThenInclude(v => v.CompoundGroup)
                .Include(v => v.VisitRequest)
                    .ThenInclude(v => v.OwnerRegistration)
                .AsQueryable();

            // filtering
            if (model.CompoundId != null)
                query = query.Where(x => model.CompoundId == x.VisitRequest.CompoundUnit.CompoundGroup.CompoundId);

            if (model.WithFilterLists == true)
            {
                var visitOwners = query.Select(v => v.VisitRequest.OwnerRegistration).Distinct();
                result.CompoundOwners = mapper.Map<List<VisitsCompoundOwnersViewModel>>(visitOwners);
                var visitUnits = query.Select(v => v.VisitRequest.CompoundUnit).Distinct();
                result.CompoundUnits = mapper.Map<List<VisitsCompoundUnitsViewModel>>(visitUnits);
                var visitGates = query.Select(v => v.Gate).Distinct();
                result.CompoundGates = mapper.Map<List<VisitsCompoundGatesViewModel>>(visitGates);
            }

            if (model.OwnerRegistrationId != null)
                query = query.Where(v => model.OwnerRegistrationId == v.VisitRequest.OwnerRegistrationId);

            if (model.CompoundUnitsIds != null)
                query = query.Where(v => model.CompoundUnitsIds.Contains(v.VisitRequest.CompoundUnitId));

            if (model.GateId != null)
                query = query.Where(v => model.GateId == v.GateId);

            if (model.Date.HasValue)
                query = query.Where(v => model.Date.Value.Date == v.Date.Date);

            if (!string.IsNullOrEmpty(model.DateFrom))
                query = query.Where(v => v.Date >= DateTime.Parse(model.DateFrom, null, System.Globalization.DateTimeStyles.RoundtripKind));

            if (!string.IsNullOrEmpty(model.DateTo))
                query = query.Where(v => v.Date <= DateTime.Parse(model.DateTo, null, System.Globalization.DateTimeStyles.RoundtripKind));

            if (model.Status.HasValue)
            {
                switch (model.Status)
                {
                    case VisitStatus.Consumed:
                        query = query.Where(x => x.VisitRequest.IsConsumed == true);
                        break;
                    case VisitStatus.Confirmed:
                        query = query.Where(x => x.VisitRequest.IsConfirmed == true && x.VisitRequest.IsConsumed != true
                                                    && x.VisitRequest.DateTo.HasValue && x.VisitRequest.DateTo.Value.Date >= DateTime.Now.Date);
                        break;
                    case VisitStatus.NotConfirmed:
                        query = query.Where(x => x.VisitRequest.IsConfirmed == false && x.VisitRequest.IsConsumed != true
                                                    && x.VisitRequest.DateTo.HasValue && x.VisitRequest.DateTo.Value.Date >= DateTime.Now.Date);
                        break;
                    case VisitStatus.Canceled:
                        query = query.Where(x => x.VisitRequest.IsCanceled == true);
                        break;
                    case VisitStatus.Expired:
                        query = query.Where(x => x.VisitRequest.DateTo.HasValue && x.VisitRequest.DateTo.Value.Date < DateTime.Now.Date);
                        break;
                    case VisitStatus.Pending:
                        query = query.Where(x => x.VisitRequest.IsConsumed != true && x.VisitRequest.IsConfirmed == null && x.VisitRequest.IsCanceled != true
                                                    && x.VisitRequest.DateTo.HasValue && x.VisitRequest.DateTo.Value.Date >= DateTime.Now.Date);
                        break;
                }
            }

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<VisitTransactionHistory, object>>>()
            {
                ["ownerName"] = v => v.VisitRequest.OwnerRegistration.Name,
                ["unitName"] = v => v.VisitRequest.CompoundUnit.Name,
                ["gateName"] = v => v.Gate.GateName,
                ["date"] = v => v.Date,
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<VisitTransactionHistoryFilterOutputViewModel>>(query);

            return result;
        }

        public async Task<VisitTransactionHistoryPagedOutput> GetPendingVisitsAsync(VisitTransactionHistoryFilterViewModel model)
        {
            var result = new VisitTransactionHistoryPagedOutput();

            var query = _visitRequestRepository.Table
                    .Where(x => x.IsConsumed != true && x.IsConfirmed == null && x.IsCanceled != true
                                   && x.DateTo.HasValue && x.DateTo.Value.Date >= DateTime.Now.Date)
                    .Include(v => v.OwnerRegistration)
                    .Include(v => v.CompoundUnit)
                        .ThenInclude(v => v.CompoundGroup)
                .AsQueryable();

            // filtering
            if (model.CompoundId != null)
                query = query.Where(x => model.CompoundId == x.CompoundUnit.CompoundGroup.CompoundId);

            if (model.WithFilterLists == true)
            {
                var visitOwners = query.Select(v => v.OwnerRegistration).Distinct();
                result.CompoundOwners = mapper.Map<List<VisitsCompoundOwnersViewModel>>(visitOwners);
                var visitUnits = query.Select(v => v.CompoundUnit).Distinct();
                result.CompoundUnits = mapper.Map<List<VisitsCompoundUnitsViewModel>>(visitUnits);
                //var visitGates = query.Select(v => v.Gate).Distinct();
                //result.CompoundGates = mapper.Map<List<VisitsCompoundGatesViewModel>>(visitGates);
            }

            if (model.OwnerRegistrationId != null)
                query = query.Where(v => model.OwnerRegistrationId == v.OwnerRegistrationId);

            if (model.CompoundUnitsIds != null)
                query = query.Where(v => model.CompoundUnitsIds.Contains(v.CompoundUnitId));         

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<VisitRequest, object>>>()
            {
                ["ownerName"] = v => v.OwnerRegistration.Name,
                ["unitName"] = v => v.CompoundUnit.Name
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<VisitTransactionHistoryFilterOutputViewModel>>(query);

            return result;
        }

        public async Task<PagedOutput<VisitTransactionHistoryFilterByUserOutputViewModel>> FilterByUserCardAsync(PagedInput model)
        {
            var query = _visitTransactionHistoryRepository.Table.Where(x => x.VisitRequest.OwnerRegistrationId == _user.Id &&
                x.VisitRequest.PrintCardRequests.Any())
                .Include(v => v.VisitRequest)
                .Select(x => new VisitTransactionHistoryFilterByUserOutputViewModel
                {
                    DateTime = x.Date,
                    GateName = x.Gate.GateName,
                    GateType = x.Gate.EntryType.ToString(),
                    UserName = x.VisitRequest.OwnerRegistration.Name,
                    VisitorName = x.VisitRequest.VisitorName
                });
            var result = new PagedOutput<VisitTransactionHistoryFilterByUserOutputViewModel>
            {
                TotalCount = await query.CountAsync(),
                Result = await query.OrderByDescending(x => x.DateTime).Skip(model.PageNumber * model.PageSize).Take(model.PageSize).ToListAsync()
            };
            return result;
        }
    }
}
