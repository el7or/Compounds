using AutoMapper;
using Puzzle.Compound.Amazon;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Mappings;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.OwnerRegistrations.Filters;
using Puzzle.Compound.Models.PushNotifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface IOwnerRegistrationService
    {
        Task<OwnerRegistration> GetUserById(Guid id);
        Task<OperationState> AddUser(OwnerRegistration user, byte[] image = null, string imageName = null);
        Task<OperationState> EditUser(OwnerRegistration ownerRegistration, byte[] image = null, string imageName = null);
        OwnerRegistration GetByPhone(string phone);
        Task<PagedOutput<OwnerRegistrationFullInfo>> GetOwnerRegistrationsAsync(OwnerRegistrationFilterViewModel ownerRegistrationFilter);
        IEnumerable<OwnerRegistration> GetUsersByMainRegistrationId(Guid mainRegistrationId);
        Task<OperationState> DeleteSubUser(Guid userId);
        Task<OperationState> ActivateSubUser(Guid userId);
        Task<OperationState> DisActivateSubUser(Guid userId);
    }

    public class OwnerRegistrationService : BaseService, IOwnerRegistrationService
    {
        private readonly IOwnerRegistrationRepository ownerRegistrationRepository;
        private readonly IS3Service _s3Service;
        private readonly IPushNotificationService _pushNotificationService;

        public OwnerRegistrationService(IOwnerRegistrationRepository ownerRegistrationRepository,
                IUnitOfWork unitOfWork,
                IMapper mapper,
                IS3Service s3Service,
                IPushNotificationService pushNotificationService)
                : base(unitOfWork, mapper)
        {
            this.ownerRegistrationRepository = ownerRegistrationRepository;
            _s3Service = s3Service;
            _pushNotificationService = pushNotificationService;
        }

        public async Task<OwnerRegistration> GetUserById(Guid id)
        {
            return await ownerRegistrationRepository.GetByIdAsync(id);
        }

        public async Task<OperationState> AddUser(OwnerRegistration ownerRegistration, byte[] image = null, string imageName = null)
        {
            var existsUser = GetByPhone(ownerRegistration.Phone);
            if (existsUser != null)
            {
                return OperationState.Exists;
            }
            ownerRegistration.RegisterDate = DateTime.UtcNow;

            if (image != null)
            {
                var path = _s3Service.UploadFile("SubOwner", imageName, image);
                ownerRegistration.Image = path;
            }

            ownerRegistrationRepository.Add(ownerRegistration);
            int result = await unitOfWork.CommitAsync();

            if (result > 0)
                return OperationState.Created;
            return OperationState.None;
        }

        public async Task<OperationState> EditUser(OwnerRegistration OwnerRegistration, byte[] image = null, string imageName = null)
        {
            ownerRegistrationRepository.Update(OwnerRegistration);
            if (image != null)
            {
                var path = _s3Service.UploadFile("SubOwner", imageName, image);
                OwnerRegistration.Image = path;
            }
            else OwnerRegistration.Image = null;
            int result = await unitOfWork.CommitAsync();
            if (result > 0)
            {
                await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
                {
                    NotificationType = PushNotificationType.RegisteredUserApproved,
                    RecordId = OwnerRegistration.OwnerRegistrationId
                });
                return OperationState.Updated;
            }
            return OperationState.None;
        }

        public OwnerRegistration GetByPhone(string phone)
        {
            return ownerRegistrationRepository.Get(c => c.Phone == phone && c.IsDeleted == false && c.IsActive == true);
        }

        public async Task<PagedOutput<OwnerRegistrationFullInfo>> GetOwnerRegistrationsAsync(OwnerRegistrationFilterViewModel ownerRegistrationFilter)
        {
            //string companies = null;
            //for (int i = 0; i < ownerRegistrationFilter.Companies?.Length; i++) {
            //	companies += $"{ownerRegistrationFilter.Companies[i]},";
            //}

            //if (companies != null)
            //	companies = companies.TrimEnd(',');

            //string compounds = null;
            //for (int i = 0; i < ownerRegistrationFilter.Compounds?.Length; i++) {
            //	compounds += $"{ownerRegistrationFilter.Compounds[i]},";
            //}
            //if (compounds != null)
            //	compounds = compounds.TrimEnd(',');

            return await ownerRegistrationRepository.GetOwnerRegistrationsAsync(ownerRegistrationFilter.Companies, ownerRegistrationFilter.Compounds, ownerRegistrationFilter.Phone, ownerRegistrationFilter.Name,
                    ownerRegistrationFilter.UserConfirmed, ownerRegistrationFilter.UserType, ownerRegistrationFilter.PageNumber, ownerRegistrationFilter.PageSize);
        }

        public IEnumerable<OwnerRegistration> GetUsersByMainRegistrationId(Guid mainRegistrationId)
        {
            return ownerRegistrationRepository.GetMany(u => u.MainRegistrationId == mainRegistrationId && u.UserType != OwnerRegistrationType.Owner && u.IsDeleted == false);
        }

        public async Task<OperationState> DeleteSubUser(Guid userId)
        {
            var user = await ownerRegistrationRepository.GetByIdAsync(userId);
            if (user == null) return OperationState.NotExists;
            user.IsDeleted = true;
            ownerRegistrationRepository.Update(user);
            var status = (await unitOfWork.CommitAsync()) > 0 ? OperationState.Deleted : OperationState.None;

            if (user.MainRegistrationId != null && user.MainRegistrationId != Guid.Empty)
                await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
                {
                    NotificationType = PushNotificationType.SubAccountDeleted,
                    RecordId = user.MainRegistrationId.Value
                });
            return status;
        }

        public async Task<OperationState> ActivateSubUser(Guid userId)
        {
            var user = await ownerRegistrationRepository.GetByIdAsync(userId);
            if (user == null) return OperationState.NotExists;
            user.IsActive = true;
            ownerRegistrationRepository.Update(user);
            var status = (await unitOfWork.CommitAsync()) > 0 ? OperationState.Updated : OperationState.None;

            if (user.MainRegistrationId != null && user.MainRegistrationId != Guid.Empty)
                await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
                {
                    NotificationType = PushNotificationType.SubAccountActive,
                    RecordId = user.MainRegistrationId.Value
                });
            return status;
        }

        public async Task<OperationState> DisActivateSubUser(Guid userId)
        {
            var user = await ownerRegistrationRepository.GetByIdAsync(userId);
            if (user == null) return OperationState.NotExists;
            user.IsActive = false;
            ownerRegistrationRepository.Update(user);
            var status = (await unitOfWork.CommitAsync()) > 0 ? OperationState.Updated : OperationState.None;

            if (user.MainRegistrationId != null && user.MainRegistrationId != Guid.Empty)
                await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
                {
                    NotificationType = PushNotificationType.SubAccountCanceled,
                    RecordId = user.MainRegistrationId.Value
                });
            return status;
        }
    }
}
