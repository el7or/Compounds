using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.Compounds;
using Puzzle.Compound.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface ICompoundService
    {
        OperationState AddCompound(Core.Models.Compound compound);
        OperationState EditCompound(Core.Models.Compound compound);
        OperationState DeleteCompound(Guid compoundId);
        Core.Models.Compound GetCompoundById(Guid id);
        Core.Models.Compound GetCompoundByName(string name);
        Task<IEnumerable<CompoundViewModel>> GetCompoundsAsync();
        Task<IEnumerable<CompoundInfo>> GetCompounds(string name, string lang);
        Task<DashboardInfoViewModel> GetDashboardInfoAsync(DashboardFilterViewModel model);
        Task<PagedOutput<PendingOutputViewModel>> GetAllPendingsAsync(AllPendingsFilterViewModel model);
        int GetAllPendingsCount(AllPendingsFilterViewModel model);
        IEnumerable<VisitsHistoryInScope> GetChartVisitsInfo(Guid compoundId, ChartScope chartScope);
        Task<string> PostEmergencyCall(EmergencyCallViewModel model);
    }

    public class CompoundService : BaseService, ICompoundService
    {
        private readonly ICompoundRepository _compoundRepository;
        private readonly UserIdentity _user;

        public CompoundService(ICompoundRepository compoundRepository, IUnitOfWork unitOfWork,
        UserIdentity user,
        IMapper mapper)
                        : base(unitOfWork, mapper)
        {
            _compoundRepository = compoundRepository;
            _user = user;
        }

        public OperationState AddCompound(Core.Models.Compound compound)
        {
            compound.CreationDate = DateTime.UtcNow;
            _compoundRepository.Add(compound);
            return unitOfWork.Commit() > 0 ? OperationState.Created : OperationState.None;
        }

        public async Task<IEnumerable<CompoundViewModel>> GetCompoundsAsync()
        {
            Guid companyUserId = _user.Id.Value;
            return await _compoundRepository.GetUserCompoundsAsync(companyUserId);
            //return _compoundRepository.GetMany(c => c.IsDeleted == false);
        }

        public Core.Models.Compound GetCompoundById(Guid id)
        {
            return _compoundRepository.Get(c => c.CompoundId == id && c.IsDeleted == false);
        }

        public Core.Models.Compound GetCompoundByName(string name)
        {
            return _compoundRepository.Get(c => (c.NameEn.Contains(name) || c.NameAr.Contains(name)) && c.IsDeleted == false);
        }

        public async Task<IEnumerable<CompoundInfo>> GetCompounds(string name, string lang)
        {
            return await _compoundRepository.Table.Where(c => c.IsDeleted == false)
                    .Where(c => lang == "en" || c.NameEn.Contains(name))
                    .Where(c => lang == "ar" || c.NameAr.Contains(name))
                    .Select(c => new CompoundInfo()
                    {
                        CompoundId = c.CompoundId,
                        CompoundName = lang == "en" ? c.NameEn : c.NameAr,
                        TimeZoneText = c.TimeZoneText,
                        TimeOffset = c.TimeZoneOffset,
                        TimeZoneValue = c.TimeZoneValue
                    }).ToListAsync();
        }

        public OperationState EditCompound(Core.Models.Compound compound)
        {
            var existingCompound = GetCompoundById(compound.CompoundId);
            existingCompound.NameAr = compound.NameAr;
            existingCompound.NameEn = compound.NameEn;
            existingCompound.Address = compound.Address;
            existingCompound.Phone = compound.Phone;
            existingCompound.EmergencyPhone = compound.EmergencyPhone;
            existingCompound.Email = compound.Email;
            existingCompound.Location = compound.Location;
            existingCompound.TimeZoneOffset = compound.TimeZoneOffset;
            existingCompound.TimeZoneText = compound.TimeZoneText;
            existingCompound.TimeZoneValue = compound.TimeZoneValue;
            if (compound.Image != null)
                existingCompound.Image = compound.Image;

            _compoundRepository.Update(existingCompound);
            return unitOfWork.Commit() > 0 ? OperationState.Updated : OperationState.None;
        }

        public OperationState DeleteCompound(Guid compoundId)
        {
            var compound = GetCompoundById(compoundId);
            if (compound != null)
            {
                if (compound.CompoundGroups?.Count > 0)
                {
                    return OperationState.None;
                }

                compound.IsDeleted = true;
                compound.IsActive = false;
                _compoundRepository.Update(compound);
                return unitOfWork.Commit() > 0 ? OperationState.Deleted : OperationState.None;
            }
            return OperationState.NotExists;
        }

        public async Task<DashboardInfoViewModel> GetDashboardInfoAsync(DashboardFilterViewModel model)
        {
            return await _compoundRepository.GetDashboardInfoAsync(model);
        }

        public IEnumerable<VisitsHistoryInScope> GetChartVisitsInfo(Guid compoundId, ChartScope chartScope)
        {
            return _compoundRepository.GetChartVisitsInfo(compoundId, chartScope);
        }

        public async Task<string> PostEmergencyCall(EmergencyCallViewModel model)
        {
            var compound = _compoundRepository.Get(c => c.CompoundId == model.CompoundId);
            if (string.IsNullOrEmpty(compound.EmergencyPhone))
            {
                throw new BusinessException("Compound not has EmergencyPhone");
            }
            compound.CompoundCalls.Add(new CompoundCall
            {
                OwnerRegistrationId = model.OwnerRegistrationId,
                EmergencyPhone = compound.EmergencyPhone,
                CallDate = DateTime.UtcNow,
                IsDeleted = false
            });

            await unitOfWork.CommitAsync();

            return compound.EmergencyPhone;
        }

        public async Task<PagedOutput<PendingOutputViewModel>> GetAllPendingsAsync(AllPendingsFilterViewModel model)
        {
            return await _compoundRepository.GetAllPendingsAsync(model);
        }

        public int GetAllPendingsCount(AllPendingsFilterViewModel model)
        {
            return _compoundRepository.GetAllPendingsCount(model);
        }
    }
}
