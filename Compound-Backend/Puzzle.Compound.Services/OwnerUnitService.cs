using AutoMapper;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.Compounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services {
	public interface IOwnerUnitService {
		IEnumerable<CompoundUnit> GetOwnerUnits(Guid ownerId);
		OperationState AddOwnerUnits(Guid ownerId, Guid[] units);
		OperationState AddOwnerUnit(Guid unitId, Guid ownerId);
		OperationState UpdateOwnerUnit(Guid oldUnitId, Guid newUnitId, Guid ownerId);
		IEnumerable<UnitInfoMap> GetUnitsByPhone(string phone, Guid? companyId, string lang = null);
		IEnumerable<UnitInfoMap> GetUnitsByRegistrationId(Guid ownerRegistrationId, Guid? companyId, string lang = null);
		Task<bool> IsUnitExists(Guid ownerRegistrationId, Guid unitId);
		IEnumerable<UnitInfoMap> GetUnitsByOwnerId(Guid compoundOwnerId, Guid? companyId);
		OperationState DeleteOwnerUnit(Guid unitId, Guid ownerId);
		OperationState DeleteOwnerUnits(Guid ownerId);
		Task<List<MainOwnerUnitInfo>> GetUnitOwnersAsync(Guid unitId);
		Task<List<OwnerUnitInfo>> GetUnitSubOwnersAsync(Guid unitId, Guid mainOwnerRegistrationId);
		Task<List<MainOwnerUnitInfo>> GetUnitMainOwnersAsync(Guid unitId, Guid? ownerRegistrationId);
		IEnumerable<CompoundInfo> GetUnitsOwnerCompounds(Guid ownerRegistrationId, Guid? companyId, string lang = null);
		OperationState UpdateUnits(IEnumerable<OwnerUnit> units, Guid ownerRegistrationId);
	}

	public class OwnerUnitService : BaseService, IOwnerUnitService {
		private readonly IOwnerUnitRepository ownerUnitRepository;
		private readonly IOwnerAssignedUnitService assignedUnitService;
		private readonly ICompoundUnitService compoundUnitService;

		public OwnerUnitService(IOwnerUnitRepository ownerUnitRepository,
				IOwnerAssignedUnitService assignedUnitService,
				ICompoundUnitService compoundUnitService,
				IUnitOfWork unitOfWork,
				IMapper mapper)
				: base(unitOfWork, mapper) {
			this.ownerUnitRepository = ownerUnitRepository;
			this.assignedUnitService = assignedUnitService;
			this.compoundUnitService = compoundUnitService;
		}

		public IEnumerable<UnitInfoMap> GetUnitsByPhone(string phone, Guid? companyId, string lang = null) {
			var groupUnits = ownerUnitRepository.GetMany(c => c.CompoundOwner.Phone.Contains(phone) && c.IsDeleted == false
																											&& (companyId == null || c.CompoundUnit.CompoundGroup.Compound.CompanyId == companyId));

			return GetMappedUnits(companyId, groupUnits, lang);
		}
		public IEnumerable<UnitInfoMap> GetUnitsByRegistrationId(Guid ownerRegistrationId, Guid? companyId, string lang = null) {
			var ownerUnits = ownerUnitRepository.GetMany(c => c.CompoundOwner.OwnerRegistrationId == ownerRegistrationId && c.IsDeleted == false
																											&& (companyId == null || c.CompoundUnit.CompoundGroup.Compound.CompanyId == companyId));

			return GetMappedUnits(companyId, ownerUnits, lang);
		}

		public IEnumerable<CompoundInfo> GetUnitsOwnerCompounds(Guid ownerRegistrationId, Guid? companyId, string lang = null) {
			var ownerUnits = ownerUnitRepository.GetMany(c => c.CompoundOwner.OwnerRegistrationId == ownerRegistrationId && c.IsDeleted == false
																											&& (companyId == null || c.CompoundUnit.CompoundGroup.Compound.CompanyId == companyId));

			var compounds = ownerUnits.Select(z => new {
				z.CompoundUnit.CompoundGroup.Compound.CompoundId,
				CompoundName = lang == "en" ? z.CompoundUnit.CompoundGroup.Compound.NameEn :
				z.CompoundUnit.CompoundGroup.Compound.NameAr,
				z.CompoundUnit.CompoundGroup.Compound.TimeZoneText,
				TimeOffset = z.CompoundUnit.CompoundGroup.Compound.TimeZoneOffset,
				TimeZoneValue = z.CompoundUnit.CompoundGroup.Compound.TimeZoneValue
			}).Distinct()
			.Select(x => new CompoundInfo {
				CompoundId = x.CompoundId,
				CompoundName = x.CompoundName,
				TimeZoneText = x.TimeZoneText,
				TimeOffset = x.TimeOffset,
				TimeZoneValue=x.TimeZoneValue
			})
			.ToList();

			return compounds;
		}

		public IEnumerable<UnitInfoMap> GetUnitsByOwnerId(Guid compoundOwnerId, Guid? companyId) {
			var groupUnits = ownerUnitRepository.GetMany(c => c.CompoundOwnerId == compoundOwnerId
																											&& (c.IsDeleted == null || c.IsDeleted == false)
																											&& (c.IsActive == null || c.IsActive == true)
																											&& (companyId == null || c.CompoundUnit.CompoundGroup.Compound.CompanyId == companyId));

			return GetMappedUnits(companyId, groupUnits);
		}

		public IEnumerable<CompoundUnit> GetOwnerUnits(Guid ownerId) {
			return ownerUnitRepository.GetMany(c => c.CompoundOwnerId == ownerId
																							&& (c.IsDeleted == null || c.IsDeleted == false)
																							&& (c.IsActive == null || c.IsActive == true))?.Select(g => g.CompoundUnit);
		}

		public OperationState AddOwnerUnits(Guid ownerId, Guid[] units) {
			OperationState operationResult = OperationState.None;

			foreach (var unitId in units) {
				var existingUnit = compoundUnitService.GetUnitById(unitId);
				if (existingUnit == null) {
					operationResult = OperationState.NotExists;
					continue;
				}
				operationResult = AddOwnerUnit(unitId, ownerId);
			}

			return operationResult;
		}

		public OperationState AddOwnerUnit(Guid unitId, Guid ownerId) {
			ownerUnitRepository.Add(new OwnerUnit {
				CompoundOwnerId = ownerId,
				CompoundUnitId = unitId,
				IsDeleted = false,
				IsActive = true
			});

			var added = unitOfWork.Commit();

			return added > 0 ? OperationState.Created : OperationState.None;
		}

		public OperationState UpdateOwnerUnit(Guid oldUnitId, Guid newUnitId, Guid ownerId) {
			var oldOwnerUnit = GetUnit(oldUnitId, ownerId);
			if (oldOwnerUnit == null) {
				return OperationState.NotExists;
			}

			var newOwnerUnit = GetUnit(newUnitId, ownerId);
			if (newOwnerUnit != null) {
				return OperationState.Exists;
			}

			oldOwnerUnit.CompoundUnitId = newUnitId;
			ownerUnitRepository.Update(oldOwnerUnit);

			var updated = unitOfWork.Commit();

			return updated > 0 ? OperationState.Updated : OperationState.None;
		}

		public OperationState DeleteOwnerUnit(Guid unitId, Guid ownerId) {
			var ownerUnit = ownerUnitRepository.Get(u => u.CompoundOwnerId == ownerId
																											&& u.CompoundUnitId == unitId
																											&& (u.IsDeleted == null || u.IsDeleted == false)
																											&& (u.IsActive == null || u.IsActive == true));
			if (ownerUnit == null) {
				return OperationState.NotExists;
			}

			ownerUnit.IsDeleted = true;
			ownerUnit.IsActive = false;

			ownerUnitRepository.Update(ownerUnit);

			var deleted = unitOfWork.Commit();
			if (deleted > 0) {
				return assignedUnitService.DeleteByUnitId(unitId);
			}

			return OperationState.None;
		}

		public OperationState DeleteOwnerUnits(Guid ownerId) {
			var ownerUnits = ownerUnitRepository.GetMany(u => u.CompoundOwnerId == ownerId
																											&& (u.IsDeleted == null || u.IsDeleted == false)
																											&& (u.IsActive == null || u.IsActive == true));

			OperationState operationState = OperationState.None;
			ownerUnits.ToList().ForEach((unit) => {
				unit.IsDeleted = true;
				unit.IsActive = false;

				ownerUnitRepository.Update(unit);

				unitOfWork.Commit();
			});

			return operationState;
		}

		private OwnerUnit GetUnit(Guid unitId, Guid ownerId) {
			return ownerUnitRepository.Get(u => u.CompoundOwnerId == ownerId
																											&& u.CompoundUnitId == unitId
																											&& (u.IsDeleted == null || u.IsDeleted == false)
																											&& (u.IsActive == null || u.IsActive == true));
		}

		private List<UnitInfoMap> GetMappedUnits(Guid? companyId, IEnumerable<OwnerUnit> compoundGroupUnits, string lang = null) {
			var units = new List<UnitInfoMap>();

			foreach (var item in compoundGroupUnits) {
				units.Add(
						new UnitInfoMap {
							CompanyId = companyId != null ? companyId.Value : item.CompoundUnit.CompoundGroup.Compound.CompanyId,
							CompoundUnitId = item.CompoundUnitId,
							Name = item.CompoundUnit.Name,
							CompoundUnitTypeId = item.CompoundUnit.CompoundUnitTypeId,
							CompoundGroupId = item.CompoundUnit.CompoundGroupId,
							CompoundId = item.CompoundUnit.CompoundGroup.Compound.CompoundId,
							CompoundName = lang == "en" ? item.CompoundUnit.CompoundGroup.Compound.NameEn :
							item.CompoundUnit.CompoundGroup.Compound.NameAr,
							CompoundTimeZone = item.CompoundUnit.CompoundGroup.Compound.TimeZoneText,
							CompoundTimeOffset = item.CompoundUnit.CompoundGroup.Compound.TimeZoneOffset
						});
			}

			return units.OrderBy(u => u.CompanyId).ToList();
		}

		public async Task<bool> IsUnitExists(Guid ownerRegistrationId, Guid unitId) {
			var results = await ownerUnitRepository.GetOwnerUnits(ownerRegistrationId: ownerRegistrationId, unitId: unitId);

			return results.Any();
		}

		public async Task<List<MainOwnerUnitInfo>> GetUnitOwnersAsync(Guid unitId) {
			return await ownerUnitRepository.GetOwnerUnits(ownerRegistrationId: null, unitId: unitId);
		}

		public async Task<List<OwnerUnitInfo>> GetUnitSubOwnersAsync(Guid unitId, Guid mainOwnerRegistrationId) {
			return await ownerUnitRepository.GetSubOwnerUnits(mainOwnerRegistrationId, subOwnerRegistrationId: null, unitId: unitId);
		}

		public async Task<List<MainOwnerUnitInfo>> GetUnitMainOwnersAsync(Guid unitId, Guid? ownerRegistrationId) {
			return await ownerUnitRepository.GetMainOwnerUnits(ownerRegistrationId, unitId);
		}

		public OperationState UpdateUnits(IEnumerable<OwnerUnit> units, Guid compoundOwnerId)
		{
			var oldUnits = ownerUnitRepository.GetMany(x => x.CompoundOwnerId == compoundOwnerId);
			foreach (var unit in oldUnits)
			{
				unit.IsDeleted = true;
				unit.IsActive = false;
				ownerUnitRepository.Update(unit);
			}
			foreach (var unit in units)
			{
				unit.CompoundOwnerId = compoundOwnerId;
				ownerUnitRepository.Add(unit);
			}
			int result = unitOfWork.Commit();
			return result > 0 ? OperationState.Created : OperationState.None;
		}
	}
}
