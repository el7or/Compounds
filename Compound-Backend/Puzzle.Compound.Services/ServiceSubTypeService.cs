using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.Services;
using Puzzle.Compound.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface IServiceSubTypeService
    {
        Task<PagedOutput<ServiceSubTypeOutputViewModel>> GetSubTypesByTypeIdAsync(Guid compoundId, Guid serviceTypeId);
        Task<ServiceSubTypeOutputViewModel> AddAsync(ServiceSubTypeInputViewModel model);
        Task<OperationState> UpdateAsync(ServiceSubTypeInputViewModel model);
        Task<OperationState> DeleteAsync(Guid serviceSubTypeId);
    }
    public class ServiceSubTypeService : BaseService, IServiceSubTypeService
    {
        private readonly IServiceSubTypeRepository _subTypeRepository;
        private readonly ICompoundServiceRepository _compoundServiceRepository;

        public ServiceSubTypeService(IServiceSubTypeRepository subTypeRepository,
            ICompoundServiceRepository compoundServiceRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork, mapper)
        {
            _subTypeRepository = subTypeRepository;
            _compoundServiceRepository = compoundServiceRepository;
        }

        public async Task<PagedOutput<ServiceSubTypeOutputViewModel>> GetSubTypesByTypeIdAsync(Guid compoundId, Guid serviceTypeId)
        {
            if (serviceTypeId == null)
                throw new BusinessException("serviceTypeId is required");

            if (compoundId == null)
                throw new BusinessException("compoundId is required");

            var query = _subTypeRepository.Table.Include(c => c.CompoundService)
                .Where(s => s.CompoundService.CompoundId == compoundId && s.CompoundService.ServiceTypeId == serviceTypeId)
                .AsQueryable();

            return new PagedOutput<ServiceSubTypeOutputViewModel>()
            {
                TotalCount = await query.CountAsync(),
                Result = await query.Select(s => new ServiceSubTypeOutputViewModel
                {
                    ServiceSubTypeId = s.ServiceSubTypeId,
                    CompoundServiceId = s.CompoundServiceId,
                    ServiceTypeId = s.CompoundService.ServiceTypeId,
                    EnglishName = s.EnglishName,
                    ArabicName = s.ArabicName,
                    Cost = s.Cost,
                    Order = s.Order
                }).ToListAsync()
            };
        }

        public async Task<ServiceSubTypeOutputViewModel> AddAsync(ServiceSubTypeInputViewModel model)
        {
            ValidateModel(model);

            var parentCompoundService = _compoundServiceRepository.Table.FirstOrDefault(s => s.CompoundId == model.CompoundId && s.ServiceTypeId == model.ServiceTypeId);
            var serviceSubTypeToAdd = mapper.Map<ServiceSubType>(model);

            if (parentCompoundService == null)
            {
                _compoundServiceRepository.Add(new Core.Models.CompoundService
                {
                    CompoundId = model.CompoundId,
                    ServiceTypeId = model.ServiceTypeId,
                    IsActive = true,
                    IsDeleted = false,
                    ServiceSubTypes = new List<ServiceSubType> { serviceSubTypeToAdd }
                });
            }
            else
            {
                serviceSubTypeToAdd.CompoundServiceId = parentCompoundService.CompoundServiceId;
                _subTypeRepository.Add(serviceSubTypeToAdd);
            }

            await unitOfWork.CommitAsync();
            return mapper.Map<ServiceSubTypeOutputViewModel>(serviceSubTypeToAdd);
        }

        public async Task<OperationState> UpdateAsync(ServiceSubTypeInputViewModel model)
        {
            if (model.ServiceSubTypeId == null)
            {
                throw new BusinessException("ServiceSubTypeId is required");
            }

            ValidateModel(model);

            var entityToUpdate = _subTypeRepository.GetById(model.ServiceSubTypeId.Value);
            entityToUpdate.EnglishName = model.EnglishName;
            entityToUpdate.ArabicName = model.ArabicName;
            entityToUpdate.Cost = model.Cost;
            entityToUpdate.Order = model.Order;

            return await unitOfWork.CommitAsync() > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<OperationState> DeleteAsync(Guid serviceSubTypeId)
        {
            var entityToDelete = await _subTypeRepository.Table.Include(s => s.ServiceRequestSubTypes)
                .FirstOrDefaultAsync(s => s.ServiceSubTypeId == serviceSubTypeId);
            if (entityToDelete == null)
            {
                throw new BusinessException("Not found");
            }
            if(entityToDelete.ServiceRequestSubTypes.Count > 0)
            {
                throw new BusinessException("Cannot delete because it is already linked to Service Requests!");
            }
            entityToDelete.IsDeleted = true;
            entityToDelete.IsActive = false;
            return await unitOfWork.CommitAsync() > 0 ? OperationState.Deleted : OperationState.None;
        }

        private void ValidateModel(ServiceSubTypeInputViewModel model)
        {
            if (model.ServiceTypeId == null)
                throw new BusinessException("serviceTypeId is required");

            if (model.CompoundId == null)
                throw new BusinessException("compoundId is required");

            if (string.IsNullOrEmpty(model.EnglishName))
                throw new BusinessException("EnglishName is required");

            if (string.IsNullOrEmpty(model.ArabicName))
                throw new BusinessException("ArabicName is required");
        }
    }
}
