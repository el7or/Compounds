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

namespace Puzzle.Compound.Services {
	public interface IOwnerAssignedUnitService {
		OperationState AddUnit(OwnerAssignedUnit unitInfo);
		OperationState AddUnits(IEnumerable<OwnerAssignedUnit> units, Guid ownerRegistrationId);
		OperationState DeleteUnit(Guid ownerAssignedUnitId);
		OperationState DeleteByUnitId(Guid compoundUnitId);
		OwnerAssignedUnit GetUnitById(Guid OwnerAssignedUnitId);
		IEnumerable<UnitInfoMap> GetUnits(Guid ownerRegistrationId, Guid? companyId, string lang = null);
		IEnumerable<CompoundInfo> GetAssignedCompounds(Guid ownerRegistrationId, Guid? companyId, string lang = null);
		OperationState UpdateUnits(IEnumerable<OwnerAssignedUnit> units, Guid ownerRegistrationId);
	}

	public class OwnerAssignedUnitService : BaseService, IOwnerAssignedUnitService {
		private readonly IOwnerAssignedUnitRepository ownerAssignedUnitRepository;

		public OwnerAssignedUnitService(IOwnerAssignedUnitRepository ownerAssignedUnitRepository, IUnitOfWork unitOfWork,
				IMapper mapper)
				: base(unitOfWork, mapper) {
			this.ownerAssignedUnitRepository = ownerAssignedUnitRepository;
		}

		public OperationState AddUnit(OwnerAssignedUnit unitInfo) {
			unitInfo.AssignedDate = DateTime.UtcNow;

			ownerAssignedUnitRepository.Add(unitInfo);
			int result = unitOfWork.Commit();

			return result > 0 ? OperationState.Created : OperationState.None;
		}

		public IEnumerable<UnitInfoMap> GetUnits(Guid ownerRegistrationId, Guid? companyId, string lang = null) {
			var ownerRequests = ownerAssignedUnitRepository.GetMany(c => c.OwnerRegistrationId == ownerRegistrationId
																															&& (companyId == null || c.CompoundUnit.CompoundGroup.Compound.CompanyId == companyId)
																															&& c.IsDeleted == false);

			var mappedUnits = new List<UnitInfoMap>();
			foreach (var request in ownerRequests) {
				mappedUnits.Add(new UnitInfoMap {
					CompanyId = request.CompoundUnit.CompoundGroup.Compound.CompanyId,
					CompoundUnitId = request.CompoundUnitId,
					Name = request.CompoundUnit.Name,
					CompoundGroupId = request.CompoundUnit.CompoundGroupId,
					CompoundUnitTypeId = request.CompoundUnit.CompoundUnitTypeId,
					CompoundId = request.CompoundUnit.CompoundGroup.CompoundId,
					CompoundName = lang == "en" ? request.CompoundUnit.CompoundGroup.Compound.NameEn
					: request.CompoundUnit.CompoundGroup.Compound.NameAr,
					CompoundTimeZone = request.CompoundUnit.CompoundGroup.Compound.TimeZoneText,
					CompoundTimeOffset = request.CompoundUnit.CompoundGroup.Compound.TimeZoneOffset
				});
			}
			return mappedUnits;
		}

		public IEnumerable<CompoundInfo> GetAssignedCompounds(Guid ownerRegistrationId, Guid? companyId, string lang = null) {
			var ownerRequests = ownerAssignedUnitRepository.GetMany(c => c.OwnerRegistrationId == ownerRegistrationId
																															&& (companyId == null || c.CompoundUnit.CompoundGroup.Compound.CompanyId == companyId)
																															&& c.IsDeleted == false);

			var compounds = ownerRequests.Select(z => new {
				CompoundId = z.CompoundUnit.CompoundGroup.Compound.CompoundId,
				CompoundName = lang == "en" ? z.CompoundUnit.CompoundGroup.Compound.NameEn :
				z.CompoundUnit.CompoundGroup.Compound.NameAr,
				TimeZoneText = z.CompoundUnit.CompoundGroup.Compound.TimeZoneText,
				TimeOffset = z.CompoundUnit.CompoundGroup.Compound.TimeZoneOffset,
				TimezoneValue = z.CompoundUnit.CompoundGroup.Compound.TimeZoneValue
			}).Distinct()
			.Select(x => new CompoundInfo {
				CompoundId = x.CompoundId,
				CompoundName = x.CompoundName,
				TimeZoneText = x.TimeZoneText,
				TimeOffset = x.TimeOffset,
				TimeZoneValue = x.TimezoneValue
			})
			.ToList();
			return compounds;
		}

		public OwnerAssignedUnit GetUnitById(Guid OwnerAssignedUnitId) {
			return ownerAssignedUnitRepository.Get(c => c.OwnerAssignedUnitId == OwnerAssignedUnitId && c.IsDeleted == false);
		}

		public OperationState DeleteUnit(Guid ownerAssignedUnitId) {
			var unit = GetUnitById(ownerAssignedUnitId);
			if (unit != null) {
				unit.IsDeleted = true;
				unit.IsActive = false;
				ownerAssignedUnitRepository.Update(unit);
				if (unitOfWork.Commit() > 0) {
					return OperationState.Deleted;
				}
				return OperationState.None;
			} else {
				return OperationState.NotExists;
			}

		}

		public OperationState DeleteByUnitId(Guid compoundUnitId) {
			var assignedOwners = ownerAssignedUnitRepository.GetMany(c => c.CompoundUnitId == compoundUnitId);

			assignedOwners.ToList().ForEach((item) => {
				item.IsDeleted = true;
				item.IsActive = false;
				ownerAssignedUnitRepository.Update(item);

			});

			if (unitOfWork.Commit() > 0) {
				return OperationState.Deleted;
			}
			return OperationState.None;
		}

		public OperationState AddUnits(IEnumerable<OwnerAssignedUnit> units, Guid ownerRegistrationId) {
			foreach (var unit in units) {
				ownerAssignedUnitRepository.Add(new OwnerAssignedUnit {
					CompoundUnitId = unit.CompoundUnitId,
					OwnerRegistrationId = ownerRegistrationId,
					AssignedDate = DateTime.UtcNow
				});
			}

			int result = unitOfWork.Commit();
			return result > 0 ? OperationState.Created : OperationState.None;
		}

		public OperationState UpdateUnits(IEnumerable<OwnerAssignedUnit> units, Guid ownerRegistrationId) {
			var oldUnits = ownerAssignedUnitRepository.GetMany(x => x.OwnerRegistrationId == ownerRegistrationId);
			foreach (var unit in oldUnits) {
				unit.IsDeleted = true;
				unit.IsActive = false;
				ownerAssignedUnitRepository.Update(unit);
			}
			foreach (var unit in units) {
				ownerAssignedUnitRepository.Add(new OwnerAssignedUnit {
					CompoundUnitId = unit.CompoundUnitId,
					OwnerRegistrationId = ownerRegistrationId,
					AssignedDate = DateTime.UtcNow
				});
			}
			int result = unitOfWork.Commit();
			return result > 0 ? OperationState.Created : OperationState.None;
		}
	}
}
