using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.PushNotifications;
using Puzzle.Compound.Services.Exceptions;

namespace Puzzle.Compound.Services
{
    public interface IRegistrationForUsersService
    {
        Task<RegistrationForUsersOutputViewModel> AddRegistration(RegistrationForUsersAddViewModel model);
        Task DeleteByUserId(Guid userId);
        Task RemoveRegistrationId(string registerId);
    }
    public class RegistrationForUsersService : BaseService, IRegistrationForUsersService
    {
        private readonly IRegistrationForUserRepository _registrationForUserRepository;
        private readonly UserIdentity _user;

        public RegistrationForUsersService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IRegistrationForUserRepository registrationForUserRepository,
            UserIdentity user) : base(unitOfWork,
            mapper)
        {
            _registrationForUserRepository = registrationForUserRepository;
            _user = user;
        }

        public async Task<RegistrationForUsersOutputViewModel> AddRegistration(RegistrationForUsersAddViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.RegisterId))
                throw new BusinessException("RegisterId is Invalid");
            if (model.UserId == Guid.Empty)
                throw new BusinessException("UserId is Invalid");
            if (model.RegisterType != "android" && model.RegisterType != "ios")
                throw new BusinessException("RegisterType is Invalid");
            if (_registrationForUserRepository.GetMany(x => x.RegisterId == model.RegisterId && x.UserId == model.UserId).Any())
                throw new BusinessException("RegisterId exist before for this user");

            var register = new RegistrationForUser()
            {
                Id = Guid.NewGuid(),
                RegisterId = model.RegisterId,
                IsActive = true,
                CreatedBy = (_user.Id ?? Guid.Empty).ToString(),
                CreatedDate = DateTime.Now,
                UserId = model.UserId,
                RegisterType = model.RegisterType
            };
            _registrationForUserRepository.Add(register);
            await unitOfWork.CommitAsync();

            return mapper.Map<RegistrationForUsersOutputViewModel>(register);
        }

        public async Task DeleteByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new BusinessException("UserId is Invalid");

            var registrationForDelete = _registrationForUserRepository.GetMany(x => x.UserId == userId);
            if (!registrationForDelete.Any())
                throw new BusinessException("No registrations found");

            _registrationForUserRepository.Delete(x => x.UserId == userId);
            await unitOfWork.CommitAsync();
        }

        public async Task RemoveRegistrationId(string registerId)
        {
            var registration = _registrationForUserRepository.Get(x => x.RegisterId == registerId);
            if (registration == null)
                throw new BusinessException("no registrations found");

            _registrationForUserRepository.Delete(x => x.RegisterId == registerId);

            await unitOfWork.CommitAsync();

        }
    }
}