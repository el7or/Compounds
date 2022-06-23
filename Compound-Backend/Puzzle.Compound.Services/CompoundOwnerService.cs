using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.Owners;
using Puzzle.Compound.Models.Owners.Filters;
using Puzzle.Compound.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface ICompoundOwnerService
    {
        OperationState AddCompoundOwner(CompoundOwner compoundOwner);
        OperationState UpdateOwnerRegistration(Guid compoundOwnerId, Guid ownerRegistrationId);
        OperationState EditCompoundOwner(CompoundOwner compoundOwner);
        OperationState DeleteCompoundOwner(Guid compoundOwnerId);
        CompoundOwner GetCompoundOwnerById(Guid id);
        OwnerInfoViewModel GetCompoundOwnerByIdWithSubOwnersCount(Guid id);
        IEnumerable<CompoundOwner> GetCompoundOwnersByPhone(string phone, Guid? companyId);
        CompoundOwner GetOwner(string phone, Guid? ownerRegistrationId);
        CompoundOwner GetCompoundOwnerByName(string name);
        IEnumerable<CompoundOwner> GetCompoundOwners(OwnerFilterViewModel ownerFilter);
        Task<PagedOutput<OwnerInfoViewModel>> GetFilteredOwnersAsync(OwnerFilterViewModel ownerFilter);

        OperationState AssignRegistrationId(Guid ownerRegistrationId, IEnumerable<CompoundOwner> owners);

    }

    public class CompoundOwnerService : BaseService, ICompoundOwnerService
    {
        private readonly ICompoundOwnerRepository compoundOwnerRepository;
        private readonly IOwnerRegistrationRepository _ownerRegisterRep;
        public CompoundOwnerService(ICompoundOwnerRepository compoundOwnerRepository, IUnitOfWork unitOfWork,
                        IMapper mapper, IOwnerRegistrationRepository ownerRegisterRep)
                        : base(unitOfWork, mapper)
        {
            this.compoundOwnerRepository = compoundOwnerRepository;
            _ownerRegisterRep = ownerRegisterRep;
        }

        public OperationState AddCompoundOwner(CompoundOwner compoundOwner)
        {
            compoundOwner.CreationDate = DateTime.UtcNow;

            compoundOwnerRepository.Add(compoundOwner);
            int result = unitOfWork.Commit();

            if (result > 0)
            {
                return OperationState.Created;
            }
            return OperationState.None;
        }



        public async Task<PagedOutput<OwnerInfoViewModel>> GetFilteredOwnersAsync(OwnerFilterViewModel model)
        {
            if (model.CompoundId == null)
                throw new BusinessException("Compound Id is required");

            var result = new PagedOutput<OwnerInfoViewModel>();

            var query = compoundOwnerRepository.Table
                .Include(c => c.OwnerUnits)
                    .ThenInclude(c => c.CompoundUnit)
                        .ThenInclude(c => c.CompoundGroup)
                            .ThenInclude(c => c.Compound)
                .AsQueryable();

            // filtering
            if (model.CompoundId.HasValue)
            {
                query = query.Where(c => c.OwnerUnits.Any(u => u.CompoundUnit.CompoundGroup.CompoundId == model.CompoundId));
            }
            if (model.CompanyId.HasValue)
            {
                query = query.Where(c => c.OwnerUnits.Any(u => u.CompoundUnit.CompoundGroup.Compound.CompanyId == model.CompanyId));
            }
            if (!string.IsNullOrEmpty(model.SearchText))
            {
                query = query.Where(c => c.Name.Contains(model.SearchText)
                                                      || c.Phone.Contains(model.SearchText)
                                                      || c.Email.Contains(model.SearchText)
                                                      || c.OwnerUnits.Any(u => u.CompoundUnit.Name.Contains(model.SearchText)));
            }
            if (!string.IsNullOrEmpty(model.Phone))
            {
                query = query.Where(c => c.Phone.Contains(model.Phone));
            }
            if (!string.IsNullOrEmpty(model.Name))
            {
                query = query.Where(c => c.Name.Contains(model.Name));
            }
            if (!string.IsNullOrEmpty(model.Gender))
            {
                query = query.Where(c => c.Gender.Contains(model.Gender));
            }
            if (!string.IsNullOrEmpty(model.Email))
            {
                query = query.Where(c => c.Email.Contains(model.Email));
            }
            if (!string.IsNullOrEmpty(model.Address))
            {
                query = query.Where(c => c.Address.Contains(model.Address));
            }
            //query = query.Where(c => (string.IsNullOrEmpty(model.Phone) || c.Phone.Contains(model.Phone))
            //                                    && (string.IsNullOrEmpty(model.Name) || c.Name.Contains(model.Name))
            //                                    && (string.IsNullOrEmpty(model.Gender) || c.Gender.Contains(model.Gender))
            //                                    && (string.IsNullOrEmpty(model.Email) || c.Email.Contains(model.Email))
            //                                    && (string.IsNullOrEmpty(model.Address) || c.Address.Contains(model.Address))
            //                                    && (string.IsNullOrEmpty(model.SearchText) || c.Name.Contains(model.SearchText) || c.Phone.Contains(model.SearchText) || c.Email.Contains(model.SearchText) || c.OwnerUnits.Any(u => u.CompoundUnit.Name.Contains(model.SearchText)))
            //                                    && (model.CompanyId == null || c.OwnerUnits.Any(u => u.CompoundUnit.CompoundGroup.Compound.CompanyId == model.CompanyId))
            //                                    && (model.CompoundId == null || c.OwnerUnits.Any(u => u.CompoundUnit.CompoundGroup.CompoundId == model.CompoundId)));

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<CompoundOwner, object>>>()
            {
                ["name"] = v => v.Name,
                ["phone"] = v => v.Phone,
                ["creationDate"] = v => v.CreationDate,
                ["status"] = v => v.IsActive,
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<OwnerInfoViewModel>>(query);

            return result;
        }

        public IEnumerable<CompoundOwner> GetCompoundOwners(OwnerFilterViewModel ownerFilter)
        {
            return compoundOwnerRepository.GetMany(c => (string.IsNullOrEmpty(ownerFilter.Phone) || c.Phone.Contains(ownerFilter.Phone))
                                                                                && (string.IsNullOrEmpty(ownerFilter.Name) || c.Name.Contains(ownerFilter.Name))
                                                                                && (string.IsNullOrEmpty(ownerFilter.Gender) || c.Gender.Contains(ownerFilter.Gender))
                                                                                && (string.IsNullOrEmpty(ownerFilter.Email) || c.Email.Contains(ownerFilter.Email))
                                                                                && (string.IsNullOrEmpty(ownerFilter.Address) || c.Address.Contains(ownerFilter.Address))
                                                                                && (ownerFilter.CompanyId == null || c.OwnerUnits.Any(u => u.CompoundUnit.CompoundGroup.Compound.CompanyId == ownerFilter.CompanyId))
                                                                                && (ownerFilter.CompoundId == null || c.OwnerUnits.Any(u => u.CompoundUnit.CompoundGroup.CompoundId == ownerFilter.CompoundId)));
        }

        public CompoundOwner GetCompoundOwnerById(Guid id)
        {
            return compoundOwnerRepository.Get(c => c.CompoundOwnerId == id && (c.IsDeleted == null || c.IsDeleted == false));
        }

        public OwnerInfoViewModel GetCompoundOwnerByIdWithSubOwnersCount(Guid id)
        {
            var owner = compoundOwnerRepository.Table
                .Include(o => o.OwnerUnits)
                    .ThenInclude(u => u.CompoundUnit)
                        .ThenInclude(a => a.OwnerAssignedUnits)
                            .ThenInclude(r => r.OwnerRegistration)
                .FirstOrDefault(c => c.CompoundOwnerId == id);

            int? subOwners = 0;
            if (owner != null)
            {
                owner.OwnerUnits.ToList().ForEach(ownerUnit =>
                {
                    subOwners += ownerUnit?.CompoundUnit?.OwnerAssignedUnits?.Count;
                });
            }
            var mappedOwner = mapper.Map<CompoundOwner, OwnerInfoViewModel>(owner);
            mappedOwner.SubOwnersCount = subOwners;

            return mappedOwner;
        }

        public CompoundOwner GetCompoundOwnerByName(string name)
        {
            return compoundOwnerRepository.Get(c => c.Name.Contains(name) && (c.IsDeleted == null || c.IsDeleted == false) && (c.IsActive == null || c.IsActive == true));
        }

        public OperationState EditCompoundOwner(CompoundOwner owner)
        {
            var existingOwner = GetCompoundOwnerById(owner.CompoundOwnerId);
            if (existingOwner == null)
            {
                return OperationState.NotExists;
            }

            existingOwner.Name = owner.Name;
            existingOwner.Phone = owner.Phone;
            existingOwner.Address = owner.Address;
            existingOwner.BirthDate = owner.BirthDate;
            existingOwner.Email = owner.Email;
            existingOwner.Gender = owner.Gender;
            existingOwner.WhatsAppNum = owner.WhatsAppNum;
            existingOwner.IsActive = owner.IsActive;

            compoundOwnerRepository.Update(existingOwner);

            var updated = unitOfWork.Commit();

            return updated > 0 ? OperationState.Updated : OperationState.None;
        }

        public OperationState UpdateOwnerRegistration(Guid compoundOwnerId, Guid ownerRegistrationId)
        {
            var existingOwner = GetCompoundOwnerById(compoundOwnerId);
            if (existingOwner == null)
            {
                return OperationState.NotExists;
            }

            existingOwner.OwnerRegistrationId = ownerRegistrationId;

            compoundOwnerRepository.Update(existingOwner);

            var updated = unitOfWork.Commit();

            return updated > 0 ? OperationState.Updated : OperationState.None;
        }

        public OperationState DeleteCompoundOwner(Guid CompoundOwnerId)
        {
            var owner = GetCompoundOwnerById(CompoundOwnerId);
            if (owner != null)
            {
                owner.IsDeleted = true;
                owner.IsActive = false;

                compoundOwnerRepository.Update(owner);
                return (unitOfWork.Commit() > 0) ? OperationState.Deleted : OperationState.None;
            }
            return OperationState.NotExists;
        }

        public IEnumerable<CompoundOwner> GetCompoundOwnersByPhone(string phone, Guid? companyId)
        {
            return compoundOwnerRepository.GetMany(c => c.Phone.Contains(phone) && (c.IsDeleted == null || c.IsDeleted == false) && (c.IsActive == null || c.IsActive == true)
                                                                            && (companyId == null || c.OwnerUnits.Any(g => g.CompoundUnit.CompoundGroup.Compound.CompanyId == companyId)));
        }

        public OperationState AssignRegistrationId(Guid ownerRegistrationId, IEnumerable<CompoundOwner> owners)
        {
            foreach (var owner in owners)
            {
                owner.OwnerRegistrationId = ownerRegistrationId;
                compoundOwnerRepository.Update(owner);
                owner.OwnerRegistration.UserType = OwnerRegistrationType.Owner;
                if (owner.OwnerUnits.Any())
                    owner.OwnerRegistration.UserConfirmed = true;
                _ownerRegisterRep.Update(owner.OwnerRegistration);
            }

            int result = unitOfWork.Commit();
            return result > 0 ? OperationState.Updated : OperationState.None;
        }

        public CompoundOwner GetOwner(string phone, Guid? ownerRegistrationId)
        {
            return compoundOwnerRepository.Get(c => (c.Phone.Contains(phone) || c.OwnerRegistrationId == ownerRegistrationId)
                                                                                                                                                            && (c.IsDeleted == null || c.IsDeleted == false) && (c.IsActive == null || c.IsActive == true));
        }
    }
}
