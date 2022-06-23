using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Amazon;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models;
using Puzzle.Compound.Models.Notifications;
using Puzzle.Compound.Models.PushNotifications;
using Puzzle.Compound.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface ICompoundNotificationService
    {
        Task<PagedOutput<NotificationOutputViewModel>> GetAsync(NotificationFilterViewModel model);
        Task<NotificationOutputViewModel> GetByIdAsync(Guid compoundNotificationId, Guid? ownerRegistrationId);
        Task<OperationState> AddAsync(NotificationInputViewModel model);
        Task<OperationState> UpdateAsync(NotificationInputViewModel model);
        Task<OperationState> DeleteAsync(Guid compoundNotificationId);
        Task<PagedOutput<NotificationMobileOutputViewModel>> GetMobileNotificationsAsync(NotificationFilterViewModel model, string Language);
        Task<NotificationMobileOutputViewModel> GetMobileNotificationByIdAsync(Guid compoundNotificationId, Guid? ownerRegistrationId, string Language);
        Task<int> GetUnreadNotificationsCount(Guid compoundId, Guid ownerRegistrationId);
    }

    public class CompoundNotificationService : BaseService, ICompoundNotificationService
    {
        private readonly ICompoundNotificationRepository _compoundNotificationRepository;
        private readonly IS3Service _s3Service;
        private readonly IPushNotificationService _pushNotificationService;

        public CompoundNotificationService(ICompoundNotificationRepository compoundNotificationRepository,
            IS3Service s3Service,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPushNotificationService pushNotificationService) : base(unitOfWork, mapper)
        {
            _compoundNotificationRepository = compoundNotificationRepository;
            _s3Service = s3Service;
            _pushNotificationService = pushNotificationService;
        }

        public async Task<PagedOutput<NotificationOutputViewModel>> GetAsync(NotificationFilterViewModel model)
        {
            var result = new PagedOutput<NotificationOutputViewModel>();

            var query = _compoundNotificationRepository.Table
                .Include(c => c.Images)
                .Include(c => c.Compound)
                .Include(c => c.OwnerNotifications)
                    .ThenInclude(c => c.OwnerRegistration)
                .Include(c => c.NotificationUnits)
                    .ThenInclude(c => c.CompoundUnit)
                        .ThenInclude(c => c.OwnerAssignedUnits)
                .Include(c => c.NotificationUnits)
                    .ThenInclude(c => c.CompoundUnit)
                        .ThenInclude(c => c.OwnerUnits)
                            .ThenInclude(c => c.CompoundOwner)
                .AsQueryable();

            // filtering
            query = FilterNotification(query, model);

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<CompoundNotification, object>>>()
            {
                ["englishTitle"] = v => v.EnglishTitle,
                ["arabicTitle"] = v => v.ArabicTitle,
                ["englishMessage"] = v => v.EnglishMessage,
                ["arabicMessage"] = v => v.ArabicMessage,
                ["isOwnerOnly"] = v => v.IsOwnerOnly,
                ["creationDate"] = v => v.CreationDate
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<NotificationOutputViewModel>>(query);

            return result;
        }

        public async Task<NotificationOutputViewModel> GetByIdAsync(Guid compoundNotificationId, Guid? ownerRegistrationId)
        {
            var compoundNotification = await _compoundNotificationRepository.Table
                .Include(c => c.Images)
                .Include(c => c.NotificationUnits)
                    .ThenInclude(c => c.CompoundUnit)
                .Include(c => c.OwnerNotifications)
                .FirstOrDefaultAsync(c => c.CompoundNotificationId == compoundNotificationId);
            if (compoundNotification == null)
            {
                throw new BusinessException("Not found");
            }
            if (ownerRegistrationId != null && compoundNotification.OwnerNotifications.Count == 0)
            {
                compoundNotification.OwnerNotifications.Add(new OwnerNotification
                {
                    OwnerRegistrationId = ownerRegistrationId.Value,
                    CreationDate = DateTime.UtcNow
                });
                await unitOfWork.CommitAsync();
            }
            return mapper.Map<NotificationOutputViewModel>(compoundNotification);
        }

        public async Task<OperationState> AddAsync(NotificationInputViewModel model)
        {
            ValidateNotification(model);

            if (_compoundNotificationRepository.GetMany(n => n.ArabicTitle.Trim() == model.ArabicTitle.Trim() || n.EnglishTitle.Trim() == model.EnglishTitle.Trim()).Count() > 0)
                throw new BusinessException("This Title has already been added before");

            var entityToAdd = mapper.Map<CompoundNotification>(model);
            if (model.Images != null)
            {
                foreach (var image in model.Images)
                {
                    if (image != null)
                    {
                        var fileBytes = Convert.FromBase64String(image.FileBase64);
                        string imagePath = _s3Service.UploadFile("news", image.FileName, fileBytes);
                        entityToAdd.Images.Add(new CompoundNotificationImage
                        {
                            IsActive = true,
                            IsDeleted = false,
                            Path = imagePath
                        });
                    }
                }
            }
            if (model.ToUnitsIds != null)
            {
                foreach (var unitId in model.ToUnitsIds)
                {
                    entityToAdd.NotificationUnits.Add(new NotificationUnit
                    {
                        CompoundUnitId = unitId
                    });
                }
            }
            entityToAdd.IsActive = true;
            entityToAdd.IsDeleted = false;
            entityToAdd.CreationDate = DateTime.UtcNow;
            entityToAdd.ModificationDate = DateTime.UtcNow;
            _compoundNotificationRepository.Add(entityToAdd);
            var commitStatus = await unitOfWork.CommitAsync() > 0 ? OperationState.Created : OperationState.None;

            await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
            {
                NotificationType = PushNotificationType.Notification,
                RecordId = entityToAdd.CompoundNotificationId
            });
            return commitStatus;
        }

        public async Task<OperationState> UpdateAsync(NotificationInputViewModel model)
        {
            if (model.CompoundNotificationId == null)
            {
                throw new BusinessException("Compound Notification Id is required");
            }

            ValidateNotification(model);

            var entityToUpdate = await _compoundNotificationRepository.Table
                .Include(c => c.Images)
                .Include(c => c.NotificationUnits)
                .FirstOrDefaultAsync(c => c.CompoundNotificationId == model.CompoundNotificationId);

            if (entityToUpdate == null)
            {
                throw new BusinessException("Not found");
            }

            mapper.Map(model, entityToUpdate);

            if (entityToUpdate.Images != null)
            {
                foreach (var oldImage in entityToUpdate.Images)
                {
                    oldImage.IsDeleted = true;
                    oldImage.IsActive = false;
                }
            }
            if (entityToUpdate.NotificationUnits != null)
            {
                foreach (var oldUnit in entityToUpdate.NotificationUnits)
                {
                    oldUnit.IsDeleted = true;
                    oldUnit.IsActive = false;
                }
            }
            if (model.Images != null)
            {
                foreach (var newImage in model.Images)
                {
                    if (newImage != null)
                    {
                        string imagePath;
                        if (newImage.Path == null)
                        {
                            var fileBytes = Convert.FromBase64String(newImage.FileBase64);
                            imagePath = _s3Service.UploadFile("news", newImage.FileName, fileBytes);
                        }
                        else
                        {
                            imagePath = newImage.Path;
                        }
                        entityToUpdate.Images.Add(new CompoundNotificationImage
                        {
                            IsActive = true,
                            IsDeleted = false,
                            Path = imagePath
                        });
                    }
                }
            }
            if (model.ToUnitsIds != null)
            {
                foreach (var newUnitId in model.ToUnitsIds)
                {
                    entityToUpdate.NotificationUnits.Add(new NotificationUnit
                    {
                        IsActive = true,
                        IsDeleted = false,
                        CompoundUnitId = newUnitId
                    });

                }
            }
            entityToUpdate.ModificationDate = DateTime.UtcNow;

            var commitStatus = await unitOfWork.CommitAsync() > 0 ? OperationState.Updated : OperationState.None;

            await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
            {
                NotificationType = PushNotificationType.Notification,
                RecordId = entityToUpdate.CompoundNotificationId
            });
            return commitStatus;
        }

        public async Task<OperationState> DeleteAsync(Guid compoundNotificationId)
        {
            var entityToDelete = await _compoundNotificationRepository.GetByIdAsync(compoundNotificationId);
            if (entityToDelete == null)
            {
                throw new BusinessException("Not found");
            }
            entityToDelete.IsDeleted = true;
            entityToDelete.IsActive = false;
            entityToDelete.ModificationDate = DateTime.UtcNow;
            return await unitOfWork.CommitAsync() > 0 ? OperationState.Deleted : OperationState.None;
        }

        public async Task<PagedOutput<NotificationMobileOutputViewModel>> GetMobileNotificationsAsync(NotificationFilterViewModel model, string lang)
        {
            if (model.OwnerRegistrationId == null)
                throw new BusinessException("OwnerRegistrationId is required");

            var result = new PagedOutput<NotificationMobileOutputViewModel>();

            var query = _compoundNotificationRepository.Table
                .Include(c => c.Images)
                .Include(c => c.Compound)
                .Include(c => c.OwnerNotifications)
                    .ThenInclude(c => c.OwnerRegistration)
                .Include(c => c.NotificationUnits)
                    .ThenInclude(c => c.CompoundUnit)
                        .ThenInclude(c => c.OwnerAssignedUnits)
                .Include(c => c.NotificationUnits)
                    .ThenInclude(c => c.CompoundUnit)
                        .ThenInclude(c => c.OwnerUnits)
                            .ThenInclude(c => c.CompoundOwner)
                .Where(c => c.CompoundId == model.CompoundId
                              && (c.NotificationUnits.Any(u => u.CompoundUnit.OwnerAssignedUnits.Any(o => o.OwnerRegistrationId == model.OwnerRegistrationId))
                                 || c.NotificationUnits.Any(u => u.CompoundUnit.OwnerUnits.Any(o => o.CompoundOwner.OwnerRegistrationId == model.OwnerRegistrationId))))
                .OrderByDescending(n => n.CreationDate)
                .AsQueryable();

            result.TotalCount = await query.CountAsync();

            query = query.ApplyPaging(model);

            result.Result = query.Select(n => new NotificationMobileOutputViewModel
            {
                CompoundNotificationId = n.CompoundNotificationId,
                Title = lang == "ar" ? n.ArabicTitle : n.EnglishTitle,
                Message = lang == "ar" ? n.ArabicMessage : n.EnglishMessage,
                IsOwnerOnly = n.IsOwnerOnly,
                IsOwnerRead = n.OwnerNotifications.Count > 0 ? true : false,
                OwnerReadOn = n.OwnerNotifications.FirstOrDefault().CreationDate,
                Images = mapper.Map<List<PuzzleFileInfo>>(n.Images)
            }).ToList();

            return result;
        }

        public async Task<NotificationMobileOutputViewModel> GetMobileNotificationByIdAsync(Guid compoundNotificationId, Guid? ownerRegistrationId, string lang)
        {
            var compoundNotification = await _compoundNotificationRepository.Table
                .Include(c => c.Images)
                .Include(c => c.NotificationUnits)
                .Include(c => c.OwnerNotifications)
                .FirstOrDefaultAsync(c => c.CompoundNotificationId == compoundNotificationId);
            if (compoundNotification == null)
            {
                throw new BusinessException("Not found");
            }
            if (ownerRegistrationId != null && compoundNotification.OwnerNotifications.Count == 0)
            {
                compoundNotification.OwnerNotifications.Add(new OwnerNotification
                {
                    OwnerRegistrationId = ownerRegistrationId.Value,
                    CreationDate = DateTime.UtcNow
                });
                await unitOfWork.CommitAsync();
            }
            return new NotificationMobileOutputViewModel
            {
                CompoundNotificationId = compoundNotification.CompoundNotificationId,
                Title = lang == "ar" ? compoundNotification.ArabicTitle : compoundNotification.EnglishTitle,
                Message = lang == "ar" ? compoundNotification.ArabicMessage : compoundNotification.EnglishMessage,
                IsOwnerOnly = compoundNotification.IsOwnerOnly,
                IsOwnerRead = compoundNotification.OwnerNotifications.Count > 0 ? true : false,
                OwnerReadOn = compoundNotification.OwnerNotifications.FirstOrDefault().CreationDate,
                Images = mapper.Map<List<PuzzleFileInfo>>(compoundNotification.Images)
            };
        }

        private IQueryable<CompoundNotification> FilterNotification(IQueryable<CompoundNotification> query, NotificationFilterViewModel queryObject)
        {
            if (queryObject.CompoundId.HasValue)
            {
                query = query.Where(c => c.CompoundId == queryObject.CompoundId);
            }
            if (queryObject.OwnerRegistrationId.HasValue)
            {
                query = query.Where(c => c.NotificationUnits.Any(u => u.CompoundUnit.OwnerAssignedUnits.Any(o => o.OwnerRegistrationId == queryObject.OwnerRegistrationId))
                                                      || c.NotificationUnits.Any(u => u.CompoundUnit.OwnerUnits.Any(o => o.CompoundOwner.OwnerRegistrationId == queryObject.OwnerRegistrationId)));
            }
            if (!string.IsNullOrEmpty(queryObject.SearchText))
            {
                query = query.Where(c => c.EnglishTitle.Contains(queryObject.SearchText)
                                      || c.ArabicTitle.Contains(queryObject.SearchText)
                                      || c.EnglishMessage.Contains(queryObject.SearchText)
                                      || c.ArabicMessage.Contains(queryObject.SearchText));
            }

            return query;
        }

        private void ValidateNotification(NotificationInputViewModel news)
        {
            if (string.IsNullOrEmpty(news.EnglishTitle))
                throw new BusinessException("English Title is required");

            if (string.IsNullOrEmpty(news.ArabicTitle))
                throw new BusinessException("Arabic Title is required");

            if (string.IsNullOrEmpty(news.EnglishMessage))
                throw new BusinessException("English Message is required");

            if (string.IsNullOrEmpty(news.ArabicMessage))
                throw new BusinessException("Arabic Message is required");

            if (news.CompoundId == null)
                throw new BusinessException("Compound Id is required");

            if (news.Images != null && news.Images.Any(i => i.SizeInBytes > 2097152))
                throw new BusinessException("File should be less than or equal 2 MB!");
        }

        public async Task<int> GetUnreadNotificationsCount(Guid compoundId, Guid ownerRegistrationId)
        {
            var query = _compoundNotificationRepository.Table
                .Include(c => c.OwnerNotifications)
                    .ThenInclude(c => c.OwnerRegistration)
                .Include(c => c.NotificationUnits)
                    .ThenInclude(c => c.CompoundUnit)
                        .ThenInclude(c => c.OwnerAssignedUnits)
                .Include(c => c.NotificationUnits)
                    .ThenInclude(c => c.CompoundUnit)
                        .ThenInclude(c => c.OwnerUnits)
                            .ThenInclude(c => c.CompoundOwner)
                .Where(c => c.CompoundId == compoundId
                            && (c.NotificationUnits.Any(u => u.CompoundUnit.OwnerAssignedUnits.Any(o => o.OwnerRegistrationId == ownerRegistrationId))
                            || c.NotificationUnits.Any(u => u.CompoundUnit.OwnerUnits.Any(o => o.CompoundOwner.OwnerRegistrationId == ownerRegistrationId)))
                            && c.OwnerNotifications.Count == 0);

            return await query.CountAsync();
        }
    }
}
