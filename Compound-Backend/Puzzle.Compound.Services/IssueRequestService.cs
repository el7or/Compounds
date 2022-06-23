using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Amazon;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Hubs;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.Issues;
using Puzzle.Compound.Models.PushNotifications;
using Puzzle.Compound.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{

    public interface IIssueRequestService
    {
        Task<CompoundIssueModel[]> GetIssueAssignment(Guid compoundId);
        Task<OperationState> UpdateIssueAssignment(Guid compoundId, CompoundIssueModel[] assignments);
        Task<PagedOutput<IssueRequestList>> GetIssueRequests(IssueRequestFilter filter);
        Task<OperationState> UpdateRequestStatus(Guid requestId, short status);
        Task<OperationState> AddRequestComment(Guid requestId, AddIssueCommentModel commentModel);
        Task<IssueRequestViewModel> GetIssueRequest(Guid requestId);
        Task<IssueTypeModel[]> GetCompoundIssues(Guid compoundId, string lang);
        Task<OperationState> CancelRequestByOwner(Guid requestId, CancelIssueRequestModel cancelModel);
        Task<OperationState> AddIssueRequest(Guid compoundId, IssueRequestModel model, IEnumerable<IssueAttachmentModel> attachments = null);
        Task<OperationState> UpdateIssueRequest(Guid requestId, Guid compoundId, IssueUpdateModel model, IEnumerable<IssueAttachmentModel> attachments = null);
        Task<OperationState> RateIssueByOwner(Guid requestId, IssueEvaluationModel rateModel);
        Task<PagedOutput<IssueRequestList>> GetOwnerRequests(Guid compoundId, OwnerIssueFilter filter);
        Task<OwnerIssueViewModel> GetOwnerRequest(Guid requestId, string lang);
        Task UpdateIssueIcon(Guid issueTypeId, string fileName, byte[] fileBytes);
        Task<CompoundIssueOutput[]> GetCompoundsIssues(CompoundIssueInput compounds);
    }

    public class IssueRequestService : BaseService, IIssueRequestService
    {

        private readonly IIssueTypeRepository _issueTypeRep;
        private readonly ICompoundIssueRepository _compIssueRep;
        private readonly IIssueRequestRepository _issueRequestRep;
        private readonly IOwnerRegistrationService _ownerRegistrationService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly UserIdentity _user;
        private readonly IS3Service _s3Service;
        private readonly IHubContext<CounterHub> _hub;
        // todo : get from configuration
        string s3Url = "http://circle360.s3.amazonaws.com/";

        public IssueRequestService(IUnitOfWork unitOfWork, IMapper mapper,
                IIssueTypeRepository issueTypeRep, ICompoundIssueRepository compIssueRep,
                IIssueRequestRepository issueRequestRep, UserIdentity user
                , IS3Service s3Service, IOwnerRegistrationService ownerRegistrationService, IHubContext<CounterHub> hub,
                IPushNotificationService pushNotificationService) : base(unitOfWork, mapper)
        {
            _issueTypeRep = issueTypeRep;
            _compIssueRep = compIssueRep;
            _issueRequestRep = issueRequestRep;
            _user = user;
            _s3Service = s3Service;
            _ownerRegistrationService = ownerRegistrationService;
            _hub = hub;
            _pushNotificationService = pushNotificationService;
        }

        #region Admin
        public async Task<CompoundIssueModel[]> GetIssueAssignment(Guid compoundId)
        {
            var assignments = await _issueTypeRep.Table.Include(x => x.CompoundIssues)
                    .Where(x => !x.IsFixed)
            .Select(z => new CompoundIssueModel
            {
                IssueTypeId = z.IssueTypeId,
                IssueNameArabic = z.ArabicName,
                IssueNameEnglish = z.EnglishName,
                IssueOrder = z.Order,
                IsFixed = z.IsFixed,
                Selected = z.CompoundIssues.Any(z => z.CompoundId == compoundId),
                AssignOrder = z.CompoundIssues.Any(z => z.CompoundId == compoundId) ?
                    z.CompoundIssues.First(z => z.CompoundId == compoundId).Order
                    : 0
            }).ToArrayAsync();
            return assignments;
        }

        public async Task<OperationState> UpdateIssueAssignment(Guid compoundId, CompoundIssueModel[] assignments)
        {
            var oldAssignments = _compIssueRep.GetMany(x => x.CompoundId == compoundId).ToList();
            var deletedAssigns = oldAssignments.Where(z => assignments.Any(y =>
            y.IssueTypeId == z.IssueTypeId && !y.Selected));
            foreach (var assign in deletedAssigns)
                _compIssueRep.Delete(assign);
            var addedAssigns = assignments.Where(z => z.Selected && oldAssignments.All(y =>
                    y.IssueTypeId != z.IssueTypeId));
            foreach (var assign in addedAssigns)
            {
                var newAssign = mapper.Map<Core.Models.CompoundIssue>(assign);
                newAssign.CompoundId = compoundId;
                _compIssueRep.Add(newAssign);
            }
            var updatedAssigns = assignments.Where(z => z.Selected && oldAssignments.Any(y =>
                    y.IssueTypeId == z.IssueTypeId));
            foreach (var assign in updatedAssigns)
            {
                var updateAssign = oldAssignments.FirstOrDefault(z => z.IssueTypeId == assign.IssueTypeId);
                if (updateAssign == null) continue;
                updateAssign.Order = updateAssign.Order;
                _compIssueRep.Update(updateAssign);
            }
            return (await unitOfWork.CommitAsync()) > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<PagedOutput<IssueRequestList>> GetIssueRequests(IssueRequestFilter filter)
        {
            var requests = _issueRequestRep.Table.Include(x => x.IssueType)
                    .Include(z => z.OwnerRegistration).Include(x => x.Compound)
                    .Where(y => y.Compound.CompoundId == filter.CompoundId);

            if (!string.IsNullOrEmpty(filter.SearchText))
                requests = requests.Where(z => z.IssueType.ArabicName.Contains(filter.SearchText) ||
                z.IssueType.EnglishName.Contains(filter.SearchText) ||
                z.Comment.Contains(filter.SearchText) || z.Note.Contains(filter.SearchText));

            requests = requests.Where(x => filter.Status == null || x.Status == filter.Status.Value)
                    .Where(x => !filter.IssueTypeIds.Any() || filter.IssueTypeIds.Contains(x.IssueTypeId));

            // apply sorting
            var columns = new Dictionary<string, Expression<Func<IssueRequest, object>>>()
            {
                ["requestedBy"] = v => v.OwnerRegistration.Name,
                ["status"] = v => v.Status,
                ["postDate"] = v => v.PostTime,
                ["postTime"] = v => v.PostTime,
                ["issueTypeEnglish"] = v => v.IssueType.EnglishName,
                ["issueTypeArabic"] = v => v.IssueType.ArabicName
            };
            requests = requests.ApplySorting(filter, columns);

            var filteredRequests = requests
                    .Select(z => new IssueRequestList()
                    {
                        IssueRequestId = z.IssueRequestId,
                        RequestNumber = z.RequestNumber,
                        IssueTypeArabic = z.IssueType.ArabicName,
                        IssueTypeEnglish = z.IssueType.EnglishName,
                        Status = z.Status,
                        PostTime = z.PostTime,
                        Rate = z.Rate,
                        RequestedBy = z.OwnerRegistration.Name,
                        Email = z.OwnerRegistration.Email,
                        Phone = z.OwnerRegistration.Phone,
                        Icon = s3Url + z.IssueType.Icon,
                        CompoundNameEnglish = z.Compound.NameEn,
                        CompoundNameArabic = z.Compound.NameAr
                    });

            var pageRequests = await filteredRequests.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize)
                    .ToListAsync();
            return new PagedOutput<IssueRequestList>
            {
                TotalCount = filteredRequests.Count(),
                Result = pageRequests
            };
        }

        public async Task<OperationState> UpdateRequestStatus(Guid requestId, short status)
        {
            var request = _issueRequestRep.Get(z => z.IssueRequestId == requestId);
            if (request == null) return OperationState.NotExists;
            request.Status = status;
            request.UpdateStatusTime = DateTime.Now;
            _issueRequestRep.Update(request);
            var result = (await unitOfWork.CommitAsync()) > 0 ? OperationState.Updated : OperationState.None;
            await _hub.Clients.All.SendAsync("UpdatePendingListCount", true);
            await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
            {
                NotificationType = status == 1 ? PushNotificationType.IssueAccepted : PushNotificationType.IssueCanceled,
                RecordId = request.IssueRequestId
            });
            return result;
        }

        public async Task<OperationState> AddRequestComment(Guid requestId, AddIssueCommentModel commentModel)
        {
            var request = _issueRequestRep.Get(z => z.IssueRequestId == requestId);
            if (request == null) return OperationState.NotExists;
            request.Comment = commentModel.Comment;
            if (commentModel.Status.HasValue)
            {
                request.Status = commentModel.Status.Value;
                request.UpdateStatusTime = DateTime.Now;
            }
            _issueRequestRep.Update(request);
            var result = (await unitOfWork.CommitAsync()) > 0 ? OperationState.Updated : OperationState.None;
            await _hub.Clients.All.SendAsync("UpdatePendingListCount", true);
            await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
            {
                NotificationType = commentModel.Status.HasValue ? PushNotificationType.IssueAcceptedWithComment : PushNotificationType.IssueComment,
                RecordId = request.IssueRequestId
            });
            return result;
        }

        public async Task<IssueRequestViewModel> GetIssueRequest(Guid requestId)
        {
            return await _issueRequestRep.Table.Include(z => z.IssueType)
                    .Include(z => z.OwnerRegistration).Include(z => z.Compound)
                    .Select(x => new IssueRequestViewModel
                    {
                        Comment = x.Comment,
                        IssueRequestId = x.IssueRequestId,
                        PostTime = x.PostTime,
                        OwnerComment = x.OwnerComment,
                        RequestNumber = x.RequestNumber,
                        Status = x.Status,
                        Note = x.Note,
                        Rate = x.Rate,
                        PresenterRate = x.PresenterRate,
                        IssueTypeArabic = x.IssueType.ArabicName,
                        IssueTypeEnglish = x.IssueType.EnglishName,
                        CompoundNameArabic = x.Compound.NameAr,
                        CompoundNameEnglish = x.Compound.NameEn,
                        OwnerName = x.OwnerRegistration.Name,
                        OwnerPhone = x.OwnerRegistration.Phone,
                        Icon = s3Url + x.IssueType.Icon,
                        Record = x.Record,
                        Attachments = x.IssueAttachments.Select(x => x.Path).ToList()
                    }).FirstOrDefaultAsync(z => z.IssueRequestId == requestId);
        }
        #endregion

        #region Owner
        public async Task<IssueTypeModel[]> GetCompoundIssues(Guid compoundId, string lang)
        {
            return await _issueTypeRep.Table.Include(z => z.CompoundIssues)
                    .Where(z => z.IsFixed || z.CompoundIssues.Any(x => x.CompoundId == compoundId))
                    .Select(z => new IssueTypeModel
                    {
                        IssueTypeId = z.IssueTypeId,
                        CompoundId = compoundId,
                        Name = lang == "en" ? z.EnglishName : z.ArabicName,
                        Order = z.Order,
                        IsFixed = z.IsFixed,
                        Icon = s3Url + z.Icon
                    }).ToArrayAsync();
        }

        public async Task<OperationState> CancelRequestByOwner(Guid requestId, CancelIssueRequestModel cancelModel)
        {
            var request = _issueRequestRep.Get(z => z.IssueRequestId == requestId);
            if (request == null) return OperationState.NotExists;
            if (request.OwnerRegistrationId != cancelModel.OwnerRegistrationId) return OperationState.NotAllowed;
            if (request.Status != 0) throw new BusinessException("Only pending requests can be cancelled");
            request.Status = 2;
            request.CancelType = 0;
            request.UpdateStatusTime = DateTime.UtcNow;
            request.UpdateStatusBy = cancelModel.OwnerRegistrationId;
            _issueRequestRep.Update(request);
            return (await unitOfWork.CommitAsync()) > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<OperationState> AddIssueRequest(Guid compoundId, IssueRequestModel request
                , IEnumerable<IssueAttachmentModel> attachments = null)
        {
            if (request == null) return OperationState.None;
            var model = mapper.Map<IssueRequest>(request);
            model.PostTime = DateTime.UtcNow;
            var maxRequest = _issueRequestRep.Table.Select(z => z.RequestNumber).DefaultIfEmpty().Max();
            model.RequestNumber = maxRequest + 1;
            model.CompoundId = compoundId;
            model.UpdateStatusTime = DateTime.UtcNow;
            model.UpdateStatusBy = _user.Id.Value;
            if (request.Record != null)
            {
                using var ms = new MemoryStream();
                await request.Record.CopyToAsync(ms);
                var fileUrl = _s3Service.UploadFile("Issues", request.Record.FileName, ms.ToArray());
                model.Record = fileUrl;
            }
            foreach (var attachment in attachments)
            {
                var attachUrl = _s3Service.UploadFile("Issues", attachment.FileName, attachment.File);
                model.IssueAttachments.Add(new IssueAttachment
                {
                    Path = attachUrl,
                    IsActive = true,
                    IsDeleted = false
                });
            }
            _issueRequestRep.Add(model);
            return (await unitOfWork.CommitAsync()) > 0 ? OperationState.Created : OperationState.None;
        }

        public async Task<OperationState> RateIssueByOwner(Guid requestId, IssueEvaluationModel rateModel)
        {
            var request = _issueRequestRep.Get(z => z.IssueRequestId == requestId);
            if (request == null) return OperationState.NotExists;
            if (request.OwnerRegistrationId != rateModel.OwnerRegistrationId) return OperationState.NotAllowed;
            if (request.Status != 1) throw new BusinessException("Only done requests can be rated");
            if (!string.IsNullOrEmpty(rateModel.Comment))
                request.OwnerComment = rateModel.Comment;
            request.Rate = rateModel.IssueRate;
            request.PresenterRate = rateModel.ProviderRate;
            request.UpdateStatusTime = DateTime.UtcNow;
            request.UpdateStatusBy = rateModel.OwnerRegistrationId;
            _issueRequestRep.Update(request);
            return (await unitOfWork.CommitAsync()) > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<PagedOutput<IssueRequestList>> GetOwnerRequests(Guid compoundId, OwnerIssueFilter filter)
        {
            var requests = _issueRequestRep.Table.Include(x => x.IssueType)
                    .Include(z => z.OwnerRegistration).Include(x => x.Compound)
                    .Where(y => y.Compound.CompoundId == compoundId);

            if (filter.IsUpcoming)
                requests = requests.Where(z => z.Status == 0);
            else
                requests = requests.Where(z => z.Status != 0);

            if (filter.IssueTypes != null)
                requests = requests.Where(z => filter.IssueTypes.Any(x => z.IssueTypeId == x));

            // filter by owner type
            var currentOwner = await _ownerRegistrationService.GetUserById(filter.OwnerRegistrationId);
            if (currentOwner.UserType == OwnerRegistrationType.Tenant)
            {
                requests = requests.Where(r => r.OwnerRegistrationId == filter.OwnerRegistrationId);
            }
            else
            {
                requests = requests.Where(r => (r.OwnerRegistrationId == filter.OwnerRegistrationId || r.OwnerRegistration.MainRegistrationId == filter.OwnerRegistrationId) && r.OwnerRegistration.UserType != OwnerRegistrationType.Tenant);
            }

            var filteredRequests = requests
                    .Select(z => new IssueRequestList()
                    {
                        IssueRequestId = z.IssueRequestId,
                        RequestNumber = z.RequestNumber,
                        IssueTypeArabic = z.IssueType.ArabicName,
                        IssueTypeEnglish = z.IssueType.EnglishName,
                        Status = z.Status,
                        PostTime = z.PostTime,
                        RequestedBy = z.OwnerRegistration.Name,
                        Icon = "http://circle360.s3.amazonaws.com/" + z.IssueType.Icon,
                        IssueTypeId = z.IssueTypeId,
                        CompoundNameEnglish = z.Compound.NameEn,
                        CompoundNameArabic = z.Compound.NameAr
                    });
            var pageRequests = await filteredRequests.Skip(filter.PageNumber * filter.PageSize).Take(filter.PageSize)
                    .ToListAsync();
            return new PagedOutput<IssueRequestList>
            {
                TotalCount = filteredRequests.Count(),
                Result = pageRequests
            };
        }

        public async Task<OwnerIssueViewModel> GetOwnerRequest(Guid requestId, string lang)
        {
            return await _issueRequestRep.Table.Include(z => z.IssueType)
                    .Include(z => z.Compound)
                    .Include(z => z.IssueAttachments)
                    .Where(z => z.IssueRequestId == requestId).Select(z =>
                                            new OwnerIssueViewModel
                                            {
                                                IssueType = lang == "en" ? z.IssueType.EnglishName : z.IssueType.ArabicName,
                                                Icon = z.IssueType.Icon,
                                                Attachments = z.IssueAttachments.Select(x => x.Path).ToList(),
                                                PostTime = z.PostTime,
                                                Note = z.Note,
                                                RequestNumber = z.RequestNumber,
                                                Record = z.Record,
                                                Rate = z.Rate,
                                                PresenterRate = z.PresenterRate,
                                                Comment = z.Comment,
                                                OwnerComment = z.OwnerComment,
                                                CanEdit = z.OwnerRegistrationId == _user.Id && z.Status == 0,
                                                CanRate = z.OwnerRegistrationId == _user.Id && z.Status == 1,
                                                CompoundNameArabic = z.Compound.NameAr,
                                                CompoundNameEnglish = z.Compound.NameEn,
                                                Status = z.Status
                                            }).FirstOrDefaultAsync();
        }

        public async Task<OperationState> UpdateIssueRequest(Guid requestId, Guid compoundId, IssueUpdateModel request,
                IEnumerable<IssueAttachmentModel> attachments = null)
        {
            if (request == null) return OperationState.None;
            var model = await _issueRequestRep.GetByIdAsync(requestId);
            mapper.Map(request, model);
            model.PostTime = DateTime.UtcNow;
            var maxRequest = _issueRequestRep.Table.Select(z => z.RequestNumber).DefaultIfEmpty().Max();
            model.UpdateStatusTime = DateTime.UtcNow;
            model.UpdateStatusBy = _user.Id.Value;
            if (request.Record != null)
            {
                using var ms = new MemoryStream();
                await request.Record.CopyToAsync(ms);
                var fileUrl = _s3Service.UploadFile("Issues", request.Record.FileName, ms.ToArray());
                model.Record = fileUrl;
            }
            if (model.IssueAttachments != null && model.IssueAttachments.Any())
                model.IssueAttachments.Clear();
            foreach (var attachment in attachments)
            {
                var attachUrl = _s3Service.UploadFile("Issues", attachment.FileName, attachment.File);
                model.IssueAttachments.Add(new IssueAttachment
                {
                    Path = attachUrl,
                    IsActive = true,
                    IsDeleted = false
                });
            }
            _issueRequestRep.Update(model);
            return (await unitOfWork.CommitAsync()) > 0 ? OperationState.Created : OperationState.None;
        }

        #endregion

        #region IssueType
        public async Task UpdateIssueIcon(Guid issueTypeId, string fileName, byte[] fileBytes)
        {
            var _issueType = await _issueTypeRep.GetByIdAsync(issueTypeId);
            if (_issueType == null) throw new BusinessException("Issue Type not found");
            string newFileName = "";
            var logoUrl = _s3Service.UploadFile("​issue", fileName, fileBytes, out newFileName);

            if (!string.IsNullOrEmpty(logoUrl))
            {
                _issueType.Icon = newFileName;
            }
            await unitOfWork.CommitAsync();
        }

        public async Task<CompoundIssueOutput[]> GetCompoundsIssues(CompoundIssueInput compounds)
        {
            var issues = await _issueTypeRep.Table.Include(x => x.CompoundIssues).Where(x => x.IsFixed ||
              x.CompoundIssues.Any(z => compounds.Compounds.Contains(z.CompoundId)))
                .ToListAsync();
            return mapper.Map<CompoundIssueOutput[]>(issues);
        }
        #endregion

    }
}
