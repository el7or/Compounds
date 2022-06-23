using AutoMapper;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services {
	public interface IOwnerUnitRequestService {
		OperationState AddUnitRequest(OwnerAssignUnitRequest requestUnit);
		OperationState AddUnitsRequest(IEnumerable<OwnerAssignUnitRequest> requestUnits, Guid ownerRegistrationId);
		OperationState DeleteRequest(Guid ownerAssignUnitRequestId);
		OperationState CancelRequest(Guid ownerAssignUnitRequestId);
		OperationState ApproveRequest(Guid ownerAssignUnitRequestId);
		Task<OperationState> ApproveRequest(Guid compoundOwnerId, Guid ownerRegistrationId);
		OwnerAssignUnitRequest GetUnitRequestById(Guid ownerAssignUnitRequestId);
		OwnerAssignUnitRequest GetUnitRequest(Guid unitId, Guid ownerRegistrationId);
		IEnumerable<UnitRequestInfoMap> GetPendingUnits(Guid ownerRegistrationId, Guid? companyId);
	}

	public class OwnerUnitRequestService : BaseService, IOwnerUnitRequestService {
		private readonly IOwnerUnitRequestRepository ownerUnitRequestRepository;
		private readonly IOwnerUnitService ownerUnitService;
		private readonly ICompoundOwnerService compoundOwnerService;
		private readonly IOwnerRegistrationService ownerRegistrationService;

		public OwnerUnitRequestService(IOwnerUnitRequestRepository ownerUnitRequestRepository,
				IOwnerUnitService ownerUnitService,
				ICompoundOwnerService compoundOwnerService,
				IOwnerRegistrationService ownerRegistrationService,
				IUnitOfWork unitOfWork,
				IMapper mapper)
				: base(unitOfWork, mapper) {
			this.ownerUnitRequestRepository = ownerUnitRequestRepository;
			this.ownerUnitService = ownerUnitService;
			this.compoundOwnerService = compoundOwnerService;
			this.ownerRegistrationService = ownerRegistrationService;
		}

		public OperationState AddUnitRequest(OwnerAssignUnitRequest requestUnit) {
			requestUnit.RequestDate = DateTime.UtcNow;
			requestUnit.Status = 0; // Waiting for approval

			ownerUnitRequestRepository.Add(requestUnit);
			int result = unitOfWork.Commit();

			return result > 0 ? OperationState.Created : OperationState.None;
		}

		public IEnumerable<UnitRequestInfoMap> GetPendingUnits(Guid ownerRegistrationId, Guid? companyId) {
			var ownerRequests = ownerUnitRequestRepository.GetMany(c => c.OwnerRegistrationId == ownerRegistrationId
																															&& c.IsDeleted == false
																															&& (companyId == null || c.CompoundUnit.CompoundGroup.Compound.CompanyId == companyId)
																															&& c.Status == 0);

			var mappedUnits = new List<UnitRequestInfoMap>();
			foreach (var request in ownerRequests) {
				mappedUnits.Add(new UnitRequestInfoMap {
					CompanyId = request.CompoundUnit.CompoundGroup.Compound.CompanyId,
					CompoundUnitId = request.CompoundUnitId,
					CompoundGroupId = request.CompoundUnit.CompoundGroupId,
					CompoundUnitTypeId = request.CompoundUnit.CompoundUnitTypeId,
					Name = request.CompoundUnit.Name,
					OwnerRegistrationId = request.OwnerRegistrationId,
					RequestDate = request.RequestDate,
					UserConfirmed = request.OwnerRegistration.UserConfirmed,
					Status = request.Status
				});
			}
			return mappedUnits;
		}

		public OwnerAssignUnitRequest GetUnitRequestById(Guid ownerAssignUnitRequestId) {
			return ownerUnitRequestRepository.Get(c => c.OwnerAssignUnitRequestId == ownerAssignUnitRequestId && c.IsDeleted == false);
		}

		public OwnerAssignUnitRequest GetUnitRequest(Guid unitId, Guid ownerRegistrationId) {
			return ownerUnitRequestRepository.Get(c => c.CompoundUnitId == unitId
																								&& c.OwnerRegistrationId == ownerRegistrationId
																								&& c.IsDeleted == false);
		}

		public OperationState DeleteRequest(Guid ownerAssignUnitRequestId) {
			var unit = GetUnitRequestById(ownerAssignUnitRequestId);
			if (unit != null) {
				unit.IsDeleted = true;
				ownerUnitRequestRepository.Update(unit);
				if (unitOfWork.Commit() > 0) {
					return OperationState.Deleted;
				}
				return OperationState.None;
			} else {
				return OperationState.NotExists;
			}

		}

		private OperationState SetRequestStatus(OwnerAssignUnitRequest ownerAssignUnitRequest, int status) {
			if (ownerAssignUnitRequest != null) {
				ownerAssignUnitRequest.Status = status;
				ownerUnitRequestRepository.Update(ownerAssignUnitRequest);
				if (unitOfWork.Commit() > 0) {
					return OperationState.Updated;
				}
				return OperationState.None;
			} else {
				return OperationState.NotExists;
			}
		}

		public OperationState CancelRequest(Guid ownerAssignUnitRequestId) {
			var unit = GetUnitRequestById(ownerAssignUnitRequestId);
			return SetRequestStatus(unit, 1);
		}

		public OperationState ApproveRequest(Guid ownerAssignUnitRequestId) {
			var unit = GetUnitRequestById(ownerAssignUnitRequestId);
			if (unit == null)
				return OperationState.NotExists;

			if (unit.Status == 2) {
				return OperationState.UpdatedBefore;
			}

			var operationResult = SetRequestStatus(unit, 2);
			if (operationResult == OperationState.Updated) {
				// Add these requests to the owner
				var ownerId = unit.OwnerRegistration.CompoundOwners.FirstOrDefault(o => o.OwnerRegistrationId == unit.OwnerRegistrationId)?.CompoundOwnerId;
				var ownerRegistration = unit.OwnerRegistration;
				if (ownerId == null) {
					var mappedCompoundOwner = mapper.Map<OwnerRegistration, CompoundOwner>(unit.OwnerRegistration);

					operationResult = compoundOwnerService.AddCompoundOwner(mappedCompoundOwner);
				}

				if (ownerId != null) {
					operationResult = ownerUnitService.AddOwnerUnit(unit.CompoundUnitId, ownerId.Value);
					if (operationResult == OperationState.Created) {
						ownerRegistration.UserConfirmed = true;
						ownerRegistrationService.EditUser(ownerRegistration);
					}
				}
			}
			return operationResult;
		}

		public async Task<OperationState> ApproveRequest(Guid compoundOwnerId, Guid ownerRegistrationId) {
			var ownerRegistration = await ownerRegistrationService.GetUserById(ownerRegistrationId);
			if (ownerRegistration == null) {
				return OperationState.NotExists;
			}
			ownerRegistration.UserConfirmed = true;
			await ownerRegistrationService.EditUser(ownerRegistration);
			return compoundOwnerService.UpdateOwnerRegistration(compoundOwnerId, ownerRegistrationId);
		}

		public OperationState AddUnitsRequest(IEnumerable<OwnerAssignUnitRequest> requestUnits, Guid ownerRegistrationId) {
			OperationState operationState = OperationState.None;
			foreach (var item in requestUnits) {
				item.OwnerRegistrationId = ownerRegistrationId;
				operationState = AddUnitRequest(item);
			}
			return operationState;
		}
	}
}
