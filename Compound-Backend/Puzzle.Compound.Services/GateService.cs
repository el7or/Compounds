using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.Gates;
using Puzzle.Compound.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services {
	public interface IGateService {
		Task<GateOutputViewModel> GetByIdAsync(Guid id);
		Task<GateOutputViewModel> AddAsync(GateAddViewModel model);
		Task UpdateAsync(GateUpdateViewModel model);
		Task DeleteAsync(Guid id, Guid compoundId);
		Task<PagedOutput<GateFilterOutputViewModel>> FilterAsync(GateFilterInputViewModel model);
		Task<GateListModel[]> GetForCompound(Guid compoundId);
		Task<List<GateListModel>> GetAll();
		Task<GateLoginModel> AuthenticateGate(string username, string password);
	}

	public class GateService : BaseService, IGateService {
		private readonly IGateRepository _gateRepository;
		private readonly ICompanyUserRepository _compUserRep;
		private readonly ICompoundRepository _compRep;
		public GateService(IUnitOfWork unitOfWork, IGateRepository gateRepository, IMapper mapper,
			ICompanyUserRepository compUserRep, ICompoundRepository compRep) : base(unitOfWork, mapper) {
			_gateRepository = gateRepository;
			_compUserRep = compUserRep;
			_compRep = compRep;
		}

		public async Task<GateOutputViewModel> AddAsync(GateAddViewModel model) {
			if (string.IsNullOrEmpty(model.GateName))
				throw new BusinessException("Name is required");
			if (model.CompoundIds == null || model.CompoundIds.Count == 0)
				throw new BusinessException("Compounds is required");
			if (string.IsNullOrEmpty(model.UserName))
				throw new BusinessException("UserName is required");
			if (string.IsNullOrEmpty(model.Password))
				throw new BusinessException("Password is required");
			Gate gate;
			if (model.GateId != null && model.GateId != Guid.Empty)
				gate = await _gateRepository.GetByIdAsync(model.GateId.Value);
			else {
				var user = new CompanyUser {
					Username = model.UserName,
					Password = model.Password,
					UserType = 3,
					IsDeleted = false,
					IsActive = true,
					CreationDate = DateTime.UtcNow,
					NameAr = model.UserName,
					NameEn = model.UserName,
					Email = model.UserName
				};
				_compUserRep.Add(user);
				gate = new Gate {
					EntryType = model.EntryType,
					GateName = model.GateName,
					IsActive = true,
					IsDeleted = false,
					User = user
				};
			}
			foreach (var item in model.CompoundIds) {
				gate.CompoundGates.Add(new CompoundGate {
					CompoundId = item,
					IsActive = true,
					IsDeleted = false
				});
			}
			if (model.GateId != null && model.GateId != Guid.Empty)
				_gateRepository.Update(gate);
			else
				_gateRepository.Add(gate);
			await unitOfWork.CommitAsync();
			return mapper.Map<GateOutputViewModel>(gate);
		}

		public async Task DeleteAsync(Guid id, Guid compoundId) {
			var gate = await _gateRepository.Table.Include(x => x.CompoundGates)
				.Include(x => x.User)
				.Where(x => x.GateId == id && !x.IsDeleted.Value).FirstOrDefaultAsync();
			if (gate == null)
				throw new BusinessException("Gate not found");
			var compoundGate = gate.CompoundGates.FirstOrDefault(x => x.CompoundId == compoundId);
			if (compoundGate == null)
				throw new BusinessException("gate not found for this compound");
			compoundGate.IsDeleted = true;
			compoundGate.IsActive = false;
			if (gate.CompoundGates.Where(z => z.CompoundId != compoundId && z.IsDeleted != true).Count() == 0) {
				gate.IsActive = false;
				gate.IsDeleted = true;
				if (gate.User != null) {
					gate.User.IsDeleted = true;
					gate.IsActive = false;
				}
			}
			await unitOfWork.CommitAsync();
		}

		public async Task<PagedOutput<GateFilterOutputViewModel>> FilterAsync(GateFilterInputViewModel model) {
			var query = _gateRepository.Table.Where(x => !x.IsDeleted.Value);
			if (!string.IsNullOrEmpty(model.Name))
				query = query.Where(x => x.GateName.StartsWith(model.Name));
			var pagedQuery = query.OrderBy(x => x.GateName).Skip(model.PageNumber * model.PageSize).Take(model.PageSize);
			var list = await pagedQuery.ToListAsync();

			var result = new PagedOutput<GateFilterOutputViewModel>() {
				TotalCount = await query.CountAsync(),
				Result = mapper.Map<List<GateFilterOutputViewModel>>(list)
			};
			return result;
		}

		public async Task<GateOutputViewModel> GetByIdAsync(Guid id) {
			var gate = await _gateRepository.Table.Include(x => x.CompoundGates)
				.Include(x => x.User).Where(x => x.GateId == id && x.IsDeleted.Value != true).FirstOrDefaultAsync();
			if (gate == null)
				throw new BusinessException("Gate not found");
			return mapper.Map<GateOutputViewModel>(gate);
		}

		public async Task UpdateAsync(GateUpdateViewModel model) {
			var gate = await _gateRepository.Table
				.Include(x => x.User)
				.Include(x => x.CompoundGates).Where(x => x.GateId == model.GateId && x.IsDeleted.Value != true).FirstOrDefaultAsync();
			if (gate == null)
				throw new BusinessException("Gate not found");
			gate.GateName = model.GateName;
			gate.EntryType = model.EntryType;
			if (gate.User == null) {
				var user = new CompanyUser {
					Username = model.UserName,
					Password = model.Password,
					UserType = 3,
					IsDeleted = false,
					IsActive = true,
					CreationDate = DateTime.UtcNow,
					NameEn = model.UserName,
					NameAr = model.UserName,
					Email = model.UserName
				};
				gate.User = user;
			} else {
				gate.User.Username = model.UserName;
				gate.User.Password = model.Password;
				gate.User.NameAr = model.UserName;
				gate.User.NameEn = model.UserName;
				gate.User.Email = model.UserName;
			}
			foreach (var item in gate.CompoundGates) {
				item.IsActive = false;
				item.IsDeleted = true;
			}
			foreach (var item in model.CompoundIds) {
				gate.CompoundGates.Add(new CompoundGate {
					CompoundId = item,
					IsActive = true,
					IsDeleted = false
				});
			}
			_gateRepository.Update(gate);
			await unitOfWork.CommitAsync();
		}

		public async Task<GateListModel[]> GetForCompound(Guid compoundId) {
			var compound = await _compRep.GetByIdAsync(compoundId);
			if (compoundId == null)
				throw new BusinessException("Compound Not Found");
			var gates = await _gateRepository.Table.Include(x => x.CompoundGates)
				.Where(x => x.IsDeleted != true && x.CompoundGates.Any(z => z.CompoundId == compoundId && z.IsDeleted != true))
				.ToArrayAsync();
			return mapper.Map<GateListModel[]>(gates);
		}

		public async Task<List<GateListModel>> GetAll() {
			var gates = await _gateRepository.Table.Include(x => x.CompoundGates)
				.Where(x => x.IsDeleted != true)
				.ToListAsync();
			return mapper.Map<List<GateListModel>>(gates);
		}

		public async Task<GateLoginModel> AuthenticateGate(string username, string password) {
			var gate = await _gateRepository.Table.Include(x => x.User).Include(x => x.CompoundGates)
				.ThenInclude(x => x.Compound).FirstOrDefaultAsync(
				x => x.User != null && x.User.Username == username && x.User.Password == password);
			if (gate == null) return null;
			return mapper.Map<GateLoginModel>(gate);
		}
	}
}
