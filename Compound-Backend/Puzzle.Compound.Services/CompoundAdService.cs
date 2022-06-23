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
using Puzzle.Compound.Models.Ads;
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
    public interface ICompoundAdService
    {
        Task<PagedOutput<AdOutputViewModel>> GetAsync(AdFilterViewModel model);
        Task<AdOutputViewModel> GetByIdAsync(Guid compoundAdId);
        Task<OperationState> AddAsync(AdInputViewModel model);
        Task<OperationState> UpdateAsync(AdInputViewModel model);
        Task<OperationState> DeleteAsync(Guid compoundAdId);
        Task<PagedOutput<AdMobileOutputViewModel>> GetMobileAdsAsync(AdFilterViewModel model, string Language);
        Task<AdMobileOutputViewModel> GetMobileAdByIdAsync(Guid compoundAdId, Guid ownerRegistrationId, string Language);
        Task<OperationState> PostAdActionAsync(Guid compoundAdId, Guid ownerRegistrationId, ActionType actionType);
    }

    public class CompoundAdService : BaseService, ICompoundAdService
    {
        private readonly ICompoundAdRepository _compoundAdRepository;
        private readonly ICompoundAdHistoryRepository _compoundAdHistoryRepository;
        private readonly IS3Service _s3Service;
        private readonly IPushNotificationService _pushNotificationService;

        public CompoundAdService(ICompoundAdRepository compoundAdRepository,
            ICompoundAdHistoryRepository compoundAdHistoryRepository,
            IS3Service s3Service,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPushNotificationService pushNotificationService) : base(unitOfWork, mapper)
        {
            _compoundAdRepository = compoundAdRepository;
            _compoundAdHistoryRepository = compoundAdHistoryRepository;
            _s3Service = s3Service;
            _pushNotificationService = pushNotificationService;
        }

        public async Task<PagedOutput<AdOutputViewModel>> GetAsync(AdFilterViewModel model)
        {
            var result = new PagedOutput<AdOutputViewModel>();

            var query = _compoundAdRepository.Table
                .Include(c => c.Images)
                .Include(c => c.CompoundAdHistories)
                .AsQueryable();

            // filtering
            query = FilterAd(query, model);

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<CompoundAd, object>>>()
            {
                ["englishTitle"] = v => v.EnglishTitle,
                ["arabicTitle"] = v => v.ArabicTitle,
                ["englishDescription"] = v => v.EnglishDescription,
                ["arabicDescription"] = v => v.ArabicDescription,
                ["startDate"] = v => v.StartDate,
                ["endDate"] = v => v.EndDate
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<AdOutputViewModel>>(query);

            return result;
        }

        public async Task<AdOutputViewModel> GetByIdAsync(Guid compoundAdId)
        {
            var compoundAd = await _compoundAdRepository.Table
                .Include(c => c.Images)
                .Include(c => c.CompoundAdHistories)
                .FirstOrDefaultAsync(c => c.CompoundAdId == compoundAdId);
            if (compoundAd == null)
            {
                throw new BusinessException("Not found");
            }
            return mapper.Map<AdOutputViewModel>(compoundAd);
        }

        public async Task<OperationState> AddAsync(AdInputViewModel model)
        {
            ValidateAd(model);

            var entityToAdd = mapper.Map<CompoundAd>(model);
            if (model.Images != null)
            {
                foreach (var image in model.Images)
                {
                    if (image != null)
                    {
                        var fileBytes = Convert.FromBase64String(image.FileBase64);
                        string imagePath = _s3Service.UploadFile("ads", image.FileName, fileBytes);
                        entityToAdd.Images.Add(new CompoundAdImage
                        {
                            IsActive = true,
                            IsDeleted = false,
                            Path = imagePath
                        });
                    }
                }
            }
            entityToAdd.IsActive = true;
            entityToAdd.IsDeleted = false;
            entityToAdd.CreationDate = DateTime.UtcNow;
            entityToAdd.ModificationDate = DateTime.UtcNow;
            _compoundAdRepository.Add(entityToAdd);
            var commitStatus = await unitOfWork.CommitAsync() > 0 ? OperationState.Created : OperationState.None;

            if (!string.IsNullOrEmpty(entityToAdd.ArabicTitle))
                await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
                {
                    NotificationType = PushNotificationType.Advertise,
                    RecordId = entityToAdd.CompoundAdId
                });

            return commitStatus;
        }

        public async Task<OperationState> UpdateAsync(AdInputViewModel model)
        {
            if (model.CompoundAdId == null)
            {
                throw new BusinessException("Compound Ad Id is required");
            }

            ValidateAd(model);

            var entityToUpdate = await _compoundAdRepository.Table
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.CompoundAdId == model.CompoundAdId);

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
                            imagePath = _s3Service.UploadFile("ads", newImage.FileName, fileBytes);
                        }
                        else
                        {
                            imagePath = newImage.Path;
                        }
                        entityToUpdate.Images.Add(new CompoundAdImage
                        {
                            IsActive = true,
                            IsDeleted = false,
                            Path = imagePath
                        });
                    }
                }
            }
            entityToUpdate.ModificationDate = DateTime.UtcNow;

            var commitStatus = await unitOfWork.CommitAsync() > 0 ? OperationState.Updated : OperationState.None;
            if (!string.IsNullOrEmpty(entityToUpdate.ArabicTitle))
                await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
                {
                    NotificationType = PushNotificationType.Advertise,
                    RecordId = entityToUpdate.CompoundAdId
                });

            return commitStatus;
        }

        public async Task<OperationState> DeleteAsync(Guid compoundAdId)
        {
            var entityToDelete = await _compoundAdRepository.GetByIdAsync(compoundAdId);
            if (entityToDelete == null)
            {
                throw new BusinessException("Not found");
            }
            entityToDelete.IsDeleted = true;
            entityToDelete.IsActive = false;
            entityToDelete.ModificationDate = DateTime.UtcNow;
            return await unitOfWork.CommitAsync() > 0 ? OperationState.Deleted : OperationState.None;
        }

        public async Task<PagedOutput<AdMobileOutputViewModel>> GetMobileAdsAsync(AdFilterViewModel model, string lang)
        {
            if (model.CompoundId == null)
                throw new BusinessException("CompoundId is required");

            if (model.OwnerRegistrationId == null)
                throw new BusinessException("OwnerRegistrationId is required");

            var result = new PagedOutput<AdMobileOutputViewModel>();

            var query = _compoundAdRepository.Table
                .Include(c => c.Images)
                .Include(c => c.CompoundAdHistories)
                .OrderByDescending(n => n.CreationDate)
                .Where(c => c.CompoundId == model.CompoundId)
                .AsQueryable();

            if (query.Count() > 0)
            {
                query.FirstOrDefault().CompoundAdHistories.Add(new CompoundAdHistory
                {
                    OwnerRegistrationId = (Guid)model.OwnerRegistrationId,
                    ActionType = ActionType.Show,
                    ActionDate = DateTime.UtcNow
                });
                await unitOfWork.CommitAsync();
            }

            result.TotalCount = await query.CountAsync();

            query = query.ApplyPaging(model);

            result.Result = query.Select(ad => new AdMobileOutputViewModel
            {
                CompoundAdId = ad.CompoundAdId,
                StartDate = ad.StartDate,
                EndDate = ad.EndDate,
                IsUrl = !string.IsNullOrEmpty(ad.AdUrl),
                AdUrl = ad.AdUrl,
                Title = lang == "ar" ? ad.ArabicTitle : ad.EnglishTitle,
                Description = lang == "ar" ? ad.ArabicDescription : ad.EnglishDescription,
                Images = mapper.Map<List<PuzzleFileInfo>>(ad.Images)
            }).ToList();

            return result;
        }

        public async Task<AdMobileOutputViewModel> GetMobileAdByIdAsync(Guid compoundAdId, Guid ownerRegistrationId, string lang)
        {
            var compoundAd = await _compoundAdRepository.Table
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.CompoundAdId == compoundAdId);

            if (compoundAd == null)
            {
                throw new BusinessException("Not found");
            }

            compoundAd.CompoundAdHistories.Add(new CompoundAdHistory
            {
                OwnerRegistrationId = ownerRegistrationId,
                ActionType = ActionType.Click,
                ActionDate = DateTime.UtcNow
            });
            await unitOfWork.CommitAsync();

            return new AdMobileOutputViewModel
            {
                CompoundAdId = compoundAd.CompoundAdId,
                StartDate = compoundAd.StartDate,
                EndDate = compoundAd.EndDate,
                IsUrl = !string.IsNullOrEmpty(compoundAd.AdUrl),
                AdUrl = compoundAd.AdUrl,
                Title = lang == "ar" ? compoundAd.ArabicTitle : compoundAd.EnglishTitle,
                Description = lang == "ar" ? compoundAd.ArabicDescription : compoundAd.EnglishDescription,
                Images = mapper.Map<List<PuzzleFileInfo>>(compoundAd.Images)
            };
        }

        public async Task<OperationState> PostAdActionAsync(Guid compoundAdId, Guid ownerRegistrationId, ActionType actionType)
        {
            if (compoundAdId == null)
                throw new BusinessException("CompoundAdId is required");

            if (ownerRegistrationId == null)
                throw new BusinessException("OwnerRegistrationId is required");

            _compoundAdHistoryRepository.Add(new CompoundAdHistory
            {
                CompoundAdId = compoundAdId,
                OwnerRegistrationId = ownerRegistrationId,
                ActionType = actionType,
                ActionDate = DateTime.UtcNow
            });

            return await unitOfWork.CommitAsync() > 0 ? OperationState.Created : OperationState.None;
        }

        private IQueryable<CompoundAd> FilterAd(IQueryable<CompoundAd> query, AdFilterViewModel queryObject)
        {
            if (queryObject.CompoundId.HasValue)
            {
                query = query.Where(c => c.CompoundId == queryObject.CompoundId);
            }
            if (!string.IsNullOrEmpty(queryObject.SearchText))
            {
                query = query.Where(c => c.EnglishTitle.Contains(queryObject.SearchText)
                                      || c.ArabicTitle.Contains(queryObject.SearchText)
                                      || c.EnglishDescription.Contains(queryObject.SearchText)
                                      || c.ArabicDescription.Contains(queryObject.SearchText));
            }

            return query;
        }

        private void ValidateAd(AdInputViewModel ad)
        {
            if (ad.CompoundId == null)
                throw new BusinessException("Compound Id is required");

            if (ad.StartDate == null)
                throw new BusinessException("Start Date is required");

            if (ad.EndDate == null)
                throw new BusinessException("End Date is required");

            if (ad.Images != null && ad.Images.Any(i => i.SizeInBytes > 2097152))
                throw new BusinessException("File should be less than or equal 2 MB!");
        }
    }
}
