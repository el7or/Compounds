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
using Puzzle.Compound.Models.News;
using Puzzle.Compound.Models.PushNotifications;
using Puzzle.Compound.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface ICompoundNewsService
    {
        Task<PagedOutput<NewsOutputViewModel>> GetAsync(NewsFilterViewModel model);
        Task<NewsOutputViewModel> GetByIdAsync(Guid compoundNewsId);
        Task<OperationState> AddAsync(NewsInputViewModel model);
        Task<OperationState> UpdateAsync(NewsInputViewModel model);
        Task<OperationState> DeleteAsync(Guid compoundNewsId);
        Task<PagedOutput<NewsMobileOutputViewModel>> GetMobileNewsAsync(NewsFilterViewModel model, string Language);
        Task<NewsMobileOutputViewModel> GetMobileNewsByIdAsync(Guid compoundNewsId, string Language);
    }

    public class CompoundNewsService : BaseService, ICompoundNewsService
    {
        private readonly ICompoundNewsRepository _compoundNewsRepository;
        private readonly IS3Service _s3Service;
        private readonly IPushNotificationService _pushNotificationService;

        public CompoundNewsService(ICompoundNewsRepository compoundNewsRepository,
            IS3Service s3Service,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPushNotificationService pushNotificationService) : base(unitOfWork, mapper)
        {
            _compoundNewsRepository = compoundNewsRepository;
            _s3Service = s3Service;
            _pushNotificationService = pushNotificationService;
        }

        public async Task<PagedOutput<NewsOutputViewModel>> GetAsync(NewsFilterViewModel model)
        {
            if (model.CompoundId == null)
                throw new BusinessException("Compound Id is required");

            var result = new PagedOutput<NewsOutputViewModel>();

            var query = _compoundNewsRepository.Table
                .Include(c => c.Images)
                .Include(c => c.Compound)
                .AsQueryable();

            // filtering
            query = FilterNews(query, model);

            // sorting
            var columnsMap = new Dictionary<string, Expression<Func<CompoundNews, object>>>()
            {
                ["englishTitle"] = v => v.EnglishTitle,
                ["arabicTitle"] = v => v.ArabicTitle,
                ["englishSummary"] = v => v.EnglishSummary,
                ["arabicSummary"] = v => v.ArabicSummary,
                ["publishDate"] = v => v.PublishDate,
                ["foregroundTillDate"] = v => v.ForegroundTillDate,
                ["status"] = v => v.IsActive,
            };
            query = query.ApplySorting(model, columnsMap);

            result.TotalCount = await query.CountAsync();

            // paging
            query = query.ApplyPaging(model);

            result.Result = mapper.Map<List<NewsOutputViewModel>>(query);

            return result;
        }

        public async Task<NewsOutputViewModel> GetByIdAsync(Guid compoundNewsId)
        {
            var compoundNews = await _compoundNewsRepository.Table
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.CompoundNewsId == compoundNewsId);
            if (compoundNews == null)
            {
                throw new BusinessException("Not found");
            }
            return mapper.Map<NewsOutputViewModel>(compoundNews);
        }

        public async Task<OperationState> AddAsync(NewsInputViewModel model)
        {
            ValidateNews(model);

            if (_compoundNewsRepository.GetMany(n => n.ArabicTitle.Trim() == model.ArabicTitle.Trim() || n.EnglishTitle.Trim() == model.EnglishTitle.Trim()).Count() > 0)
                throw new BusinessException("This Title has already been added before");

            var entityToAdd = mapper.Map<CompoundNews>(model);
            if (model.Images != null)
            {
                foreach (var image in model.Images)
                {
                    if (image != null)
                    {
                        var fileBytes = Convert.FromBase64String(image.FileBase64);
                        string imagePath = _s3Service.UploadFile("news", image.FileName, fileBytes);
                        entityToAdd.Images.Add(new CompoundNewsImage
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
            _compoundNewsRepository.Add(entityToAdd);
            var commitStatus = await unitOfWork.CommitAsync() > 0 ? OperationState.Created : OperationState.None;

            await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
            {
                NotificationType = PushNotificationType.News,
                RecordId = entityToAdd.CompoundNewsId
            });

            return commitStatus;
        }

        public async Task<OperationState> UpdateAsync(NewsInputViewModel model)
        {
            if (model.CompoundNewsId == null)
            {
                throw new BusinessException("Compound News Id is required");
            }

            ValidateNews(model);

            var entityToUpdate = await _compoundNewsRepository.Table
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.CompoundNewsId == model.CompoundNewsId);

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
                            imagePath = _s3Service.UploadFile("news", newImage.FileName, fileBytes);
                        }
                        else
                        {
                            imagePath = newImage.Path;
                        }
                        entityToUpdate.Images.Add(new CompoundNewsImage
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

            await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
            {
                NotificationType = PushNotificationType.News,
                RecordId = entityToUpdate.CompoundNewsId
            });

            return commitStatus;
        }

        public async Task<OperationState> DeleteAsync(Guid compoundNewsId)
        {
            var entityToDelete = await _compoundNewsRepository.GetByIdAsync(compoundNewsId);
            if (entityToDelete == null)
            {
                throw new BusinessException("Not found");
            }
            entityToDelete.IsDeleted = true;
            entityToDelete.IsActive = false;
            entityToDelete.ModificationDate = DateTime.UtcNow;
            return await unitOfWork.CommitAsync() > 0 ? OperationState.Deleted : OperationState.None;
        }

        public async Task<PagedOutput<NewsMobileOutputViewModel>> GetMobileNewsAsync(NewsFilterViewModel model, string lang)
        {
            if (model.CompoundId == null)
                throw new BusinessException("Compound Id is required");

            var result = new PagedOutput<NewsMobileOutputViewModel>();

            var query = _compoundNewsRepository.Table.Include(c => c.Images)
                .Where(n => n.CompoundId == model.CompoundId && n.IsActive == true && n.PublishDate.Date <= DateTime.Now.Date)
                .OrderByDescending(n => n.ForegroundTillDate.HasValue && n.ForegroundTillDate.Value.Date >= DateTime.Now.Date ? n.ForegroundTillDate : n.PublishDate)
                .AsQueryable();

            result.TotalCount = await query.CountAsync();

            query = query.ApplyPaging(model);

            //result.Result = mapper.Map<List<NewsMobileOutputViewModel>>(query);
            result.Result = query.Select(n => new NewsMobileOutputViewModel
            {
                CompoundNewsId = n.CompoundNewsId,
                CompoundId = n.CompoundId,
                IsActive = n.IsActive,
                PublishDate = n.PublishDate,
                ForegroundTillDate = n.ForegroundTillDate,
                Images = mapper.Map<List<PuzzleFileInfo>>(n.Images),
                Title = lang == "ar" ? n.ArabicTitle : n.EnglishTitle,
                Summary = lang == "ar" ? n.ArabicSummary : n.EnglishSummary,
                Details = lang == "ar" ? n.ArabicDetails : n.EnglishDetails
            }).ToList();

            return result;
        }

        public async Task<NewsMobileOutputViewModel> GetMobileNewsByIdAsync(Guid compoundNewsId, string lang)
        {
            var compoundNews = await _compoundNewsRepository.Table
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.CompoundNewsId == compoundNewsId);
            if (compoundNews == null)
            {
                throw new BusinessException("Not found");
            }
            return new NewsMobileOutputViewModel
            {
                CompoundNewsId = compoundNews.CompoundNewsId,
                CompoundId = compoundNews.CompoundId,
                IsActive = compoundNews.IsActive,
                PublishDate = compoundNews.PublishDate,
                ForegroundTillDate = compoundNews.ForegroundTillDate,
                Images = mapper.Map<List<PuzzleFileInfo>>(compoundNews.Images),
                Title = lang == "ar" ? compoundNews.ArabicTitle : compoundNews.EnglishTitle,
                Summary = lang == "ar" ? compoundNews.ArabicSummary : compoundNews.EnglishSummary,
                Details = lang == "ar" ? compoundNews.ArabicDetails : compoundNews.EnglishDetails
            };
        }

        private IQueryable<CompoundNews> FilterNews(IQueryable<CompoundNews> query, NewsFilterViewModel queryObject)
        {
            if (queryObject.CompoundId.HasValue)
            {
                query = query.Where(c => c.CompoundId == queryObject.CompoundId);
            }
            if (queryObject.CompanyId.HasValue)
            {
                query = query.Where(c => c.Compound.CompanyId == queryObject.CompanyId);
            }
            if (queryObject.PublishDateFrom.HasValue)
            {
                query = query.Where(p => p.PublishDate.Date >= queryObject.PublishDateFrom.Value.Date);
            }
            if (queryObject.PublishDateTo.HasValue)
            {
                query = query.Where(p => p.PublishDate.Date <= queryObject.PublishDateTo.Value.Date);
            }
            if (queryObject.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == queryObject.IsActive);
            }
            if (!string.IsNullOrEmpty(queryObject.SearchText))
            {
                query = query.Where(c => c.EnglishTitle.Contains(queryObject.SearchText)
                                      || c.ArabicTitle.Contains(queryObject.SearchText)
                                      || c.EnglishSummary.Contains(queryObject.SearchText)
                                      || c.ArabicSummary.Contains(queryObject.SearchText));
            }

            return query;
        }

        private void ValidateNews(NewsInputViewModel news)
        {
            if (string.IsNullOrEmpty(news.EnglishTitle))
                throw new BusinessException("English Title is required");

            if (string.IsNullOrEmpty(news.ArabicTitle))
                throw new BusinessException("Arabic Title is required");

            if (string.IsNullOrEmpty(news.EnglishSummary))
                throw new BusinessException("English Summary is required");

            if (string.IsNullOrEmpty(news.ArabicSummary))
                throw new BusinessException("Arabic Summary is required");

            if (string.IsNullOrEmpty(news.EnglishDetails))
                throw new BusinessException("English Details is required");

            if (string.IsNullOrEmpty(news.ArabicDetails))
                throw new BusinessException("Arabic Details is required");

            if (news.CompoundId == null)
                throw new BusinessException("Compound Id is required");

            if (news.PublishDate == null)
                throw new BusinessException("Publish Date is required");

            if (news.ForegroundTillDate != null && news.PublishDate > news.ForegroundTillDate)
                throw new BusinessException("Foreground Till Date must be after Publish Date");

            if (news.Images != null && news.Images.Any(i => i.SizeInBytes > 2097152))
                throw new BusinessException("File should be less than or equal 2 MB!");
        }
    }
}
