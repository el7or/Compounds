using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.Units;
using Puzzle.Compound.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Puzzle.Compound.Common.Hubs;

namespace Puzzle.Compound.Services
{
    public interface ICompoundUnitService
    {
        OperationState AddUnit(CompoundUnit unit);
        OperationState EditUnit(CompoundUnit unit);
        OperationState DeleteUnit(Guid id);
        CompoundUnit GetUnitById(Guid id);
        CompoundUnit GetUnitByName(string name);
        IEnumerable<CompoundUnit> GetUnits();
        Task<PagedOutput<UnitInfoViewModel>> FilterUnitsAsync(UnitFilterByTextInputViewModel model);
        IEnumerable<UnitInfoViewModel> GetUnitsByGroupId(Guid groupId);
        UnitInfoMap GetUnitsByCompanyId(Guid companyId, string unitName);
        UnitInfoMap GetUnitsByCompoundId(Guid compoundId, string unitName);
        Task<OperationState> ImportUnitsOwnersAsync(AddUnitsOwnersViewModel model);
    }

    public class CompoundUnitService : BaseService, ICompoundUnitService
    {
        private readonly ICompoundUnitRepository _unitRepository;
        private readonly ICompoundRepository _compoundRepository;
        private readonly ICompoundOwnerRepository _compoundOwnerRepository;
        private readonly IHubContext<CounterHub> _hub;

        public CompoundUnitService(ICompoundUnitRepository unitRepository,
            IUnitOfWork unitOfWork,
                IMapper mapper,
                ICompoundRepository compoundRepository,
                ICompoundOwnerRepository compoundOwnerRepository,
                IHubContext<CounterHub> hub)
                : base(unitOfWork, mapper)
        {
            _unitRepository = unitRepository;
            _compoundRepository = compoundRepository;
            _compoundOwnerRepository = compoundOwnerRepository;
            _hub = hub;
        }

        public OperationState AddUnit(CompoundUnit unit)
        {
            var existingUnit = GetUnitByGroupId(unit.CompoundGroupId, unit.Name);
            if (existingUnit != null)
            {
                return OperationState.Exists;
            }

            _unitRepository.Add(unit);
            int result = unitOfWork.Commit();

            return (result > 0) ? OperationState.Created : OperationState.None;
        }

        public IEnumerable<CompoundUnit> GetUnits()
        {
            return _unitRepository.GetMany(c => (c.IsDeleted == null || c.IsDeleted == false) && (c.IsActive == null || c.IsActive == true));
        }

        public async Task<PagedOutput<UnitInfoViewModel>> FilterUnitsAsync(UnitFilterByTextInputViewModel model)
        {
            var query = GetUnitQuery();
            query = query.Where(u => u.Name.StartsWith(model.Text) && u.CompoundGroup.CompoundId == model.CompoundId);

            var output = new PagedOutput<UnitInfoViewModel>();
            output.TotalCount = await query.CountAsync();
            var result = query.ApplyPaging(model);
            output.Result = mapper.Map<List<UnitInfoViewModel>>(result.ToList());

            //var result = await query.Skip(model.PageNumber * model.PageSize).Take(model.PageSize).ToListAsync();
            //var output = new PagedOutput<UnitInfoViewModel> {
            //	TotalCount = query.Count(),
            //	Result = mapper.Map<List<UnitInfoViewModel>>(result)
            //};
            return output;
        }

        private IQueryable<CompoundUnit> GetUnitQuery()
        {
            var query = _unitRepository.Table
                .Include(c => c.CompoundGroup)
                .Where(c => (c.IsDeleted == null || c.IsDeleted == false)
                                                                            && (c.IsActive == null || c.IsActive == true));
            return query;
        }

        public CompoundUnit GetUnitById(Guid id)
        {
            return _unitRepository.Get(c => c.CompoundUnitId == id && (c.IsDeleted == null || c.IsDeleted == false) && (c.IsActive == null || c.IsActive == true));
        }

        public CompoundUnit GetUnitByName(string name)
        {
            return _unitRepository.Get(c => c.Name.ToLower().Contains(name.ToLower()) && (c.IsDeleted == null || c.IsDeleted == false) && (c.IsActive == null || c.IsActive == true));
        }

        public OperationState EditUnit(CompoundUnit unit)
        {
            var existingUnit = GetUnitByGroupId(unit.CompoundGroupId, unit.Name);
            if (existingUnit != null && existingUnit.CompoundUnitId != unit.CompoundUnitId)
            {
                return OperationState.Exists;
            }

            existingUnit = GetUnitById(unit.CompoundUnitId);
            existingUnit.Name = unit.Name;
            existingUnit.CompoundGroupId = unit.CompoundGroupId;
            existingUnit.CompoundUnitTypeId = unit.CompoundUnitTypeId;

            _unitRepository.Update(existingUnit);
            int result = unitOfWork.Commit();

            return (result > 0) ? OperationState.Updated : OperationState.None;
        }

        public OperationState DeleteUnit(Guid id)
        {
            var unit = GetUnitById(id);
            if (unit == null)
            {
                return OperationState.NotExists;
            }
            else
            {
                if (unit.OwnerUnits?.Count > 0)
                {
                    return OperationState.None;
                }
                unit.IsDeleted = true;
                unit.IsActive = false;
                _unitRepository.Update(unit);
                return (unitOfWork.Commit() > 0) ? OperationState.Deleted : OperationState.None;
            }
        }

        public CompoundUnit GetUnitByGroupId(Guid groupId, string name)
        {
            return _unitRepository.Get(c => c.CompoundGroupId == groupId && (c.IsDeleted == null || c.IsDeleted == false) && (c.IsActive == null || c.IsActive == true) && c.Name.ToLower().Contains(name.ToLower()));
        }

        public IEnumerable<UnitInfoViewModel> GetUnitsByGroupId(Guid groupId)
        {
            var units = _unitRepository.GetMany(c => c.CompoundGroupId == groupId && (c.IsDeleted == null || c.IsDeleted == false) && (c.IsActive == null || c.IsActive == true));
            return mapper.Map<IEnumerable<CompoundUnit>, IEnumerable<UnitInfoViewModel>>(units);
        }

        public UnitInfoMap GetUnitsByCompanyId(Guid companyId, string unitName)
        {
            var unit = _unitRepository.Get(c => c.Name.ToLower().Contains(unitName) && (c.IsDeleted == null || c.IsDeleted == false) && (c.IsActive == null || c.IsActive == true)
                                                                                                            && c.CompoundGroup.Compound.CompanyId == companyId);

            if (unit == null)
                return null;

            return new UnitInfoMap
            {
                CompanyId = companyId,
                CompoundUnitId = unit.CompoundUnitId,
                CompoundGroupId = unit.CompoundGroupId,
                CompoundUnitTypeId = unit.CompoundUnitTypeId,
                Name = unit.Name
            };
        }

        public UnitInfoMap GetUnitsByCompoundId(Guid compoundId, string unitName)
        {
            var unit = _unitRepository.Get(c => c.Name.ToLower().Contains(unitName) && (c.IsDeleted == null || c.IsDeleted == false) && (c.IsActive == null || c.IsActive == true)
                                                                                                            && c.CompoundGroup.CompoundId == compoundId);
            if (unit == null)
                return null;

            return new UnitInfoMap
            {
                CompanyId = unit.CompoundGroup.Compound.CompanyId,
                CompoundUnitId = unit.CompoundUnitId,
                CompoundGroupId = unit.CompoundGroupId,
                CompoundUnitTypeId = unit.CompoundUnitTypeId,
                Name = unit.Name,
                CompoundName = unit.CompoundGroup.Compound.NameEn,
                CompoundId = unit.CompoundGroup.CompoundId,
                CompoundTimeZone = unit.CompoundGroup.Compound.TimeZoneText,
                CompoundTimeOffset = unit.CompoundGroup.Compound.TimeZoneOffset

            };
        }

        public async Task<OperationState> ImportUnitsOwnersAsync(AddUnitsOwnersViewModel model)
        {
            // get compound groups and units
            var compound = _compoundRepository.Table.Where(c => c.CompoundId == model.CompoundId)
                .Include(g => g.CompoundGroups)
                    .ThenInclude(u => u.CompoundUnits)
                        .ThenInclude(ou => ou.OwnerUnits)
                            .ThenInclude(o => o.CompoundOwner)
                .FirstOrDefault();
            if (compound == null)
                throw new BusinessException("Compound not found!");

            int counter = 1;
            // add groups, units, owners in compound
            foreach (var unitOwner in model.UnitsOwners)
            {
                string[] groups = unitOwner.Group.Trim().Split("-");
                for (int i = 0; i < groups.Length; i++)
                {
                    // add group if not exists
                    var group = compound.CompoundGroups.FirstOrDefault(g => g.NameEn.Trim().ToLower() == groups[i].Trim().ToLower());
                    if (group == null)
                    {
                        group = new CompoundGroup
                        {
                            NameEn = groups[i].Trim(),
                            NameAr = groups[i].Trim(),
                            CreationDate = DateTime.UtcNow,
                            IsActive = true,
                            IsDeleted = false,
                            // add parent group
                            Group = i > 0 ? compound.CompoundGroups.FirstOrDefault(g => g.NameEn.Trim() == groups[i - 1].Trim()) : null
                        };
                        compound.CompoundGroups.Add(group);
                    }
                }

                // add unit if not exists
                var groupParentUnit = compound.CompoundGroups.FirstOrDefault(g => g.NameEn.Trim() == groups[groups.Length - 1].Trim());
                var unit = groupParentUnit.CompoundUnits.FirstOrDefault(u => u.Name.Trim().ToLower() == unitOwner.Unit.Trim().ToLower());
                if (unit == null)
                {
                    unit = new CompoundUnit
                    {
                        Name = unitOwner.Unit.Trim(),
                        CompoundUnitTypeId = unitOwner.UnitType,
                        IsActive = true,
                        IsDeleted = false
                    };
                    groupParentUnit.CompoundUnits.Add(unit);
                }

                // add owner if not exists
                var owner = _compoundOwnerRepository.Table
                    .Where(o => o.Phone.Trim().ToLower() == unitOwner.Mobile.Replace("+", "").TrimStart('0').Trim().ToLower())
                    .Include(u => u.OwnerUnits)
                     .ThenInclude(u => u.CompoundUnit)
                    .FirstOrDefault();
                if (owner == null)
                {
                    owner = new CompoundOwner
                    {
                        Name = unitOwner.OwnerName,
                        Phone = unitOwner.Mobile,
                        Email = unitOwner.Email,
                        CreationDate = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false
                    };
                    _compoundOwnerRepository.Add(owner);
                }

                // assign unit to owner if not assign before
                if (!owner.OwnerUnits.Any(u => u.CompoundUnit == unit))
                {
                    owner.OwnerUnits.Add(new OwnerUnit
                    {
                        CompoundUnit = unit,
                        IsActive = true,
                        IsDeleted = false
                    });
                }
                await _hub.Clients.Client(model.ConnectionId).SendAsync("UpdateAddedExcelOwnersCount", counter);
                counter++;
            }
            _compoundRepository.Update(compound);
            return await unitOfWork.CommitAsync() > 0 ? OperationState.Created : OperationState.None;
        }
    }
}
