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
using Puzzle.Compound.Models.PushNotifications;
using Puzzle.Compound.Models.Services;
using Puzzle.Compound.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{

    public interface IServiceRequestService
    {
        Task<CompoundServiceModel[]> GetServiceAssignment(Guid compoundId);
        Task<OperationState> UpdateServiceAssignment(Guid compoundId, CompoundServiceModel[] assignments);
        Task<PagedOutput<ServiceRequestList>> GetServiceRequests(ServiceRequestFilter filter);
        Task<OperationState> UpdateRequestStatus(Guid requestId, short status);
        Task<OperationState> AddRequestComment(Guid requestId, AddCommentModel commentModel);
        Task<ServiceRequestViewModel> GetServiceRequest(Guid requestId);
        Task<ServiceTypeModel[]> GetCompoundServices(Guid compoundId, string lang);
        Task<OperationState> CancelRequestByOwner(Guid requestId, CancelRequestModel cancelModel);
        Task<OperationState> AddServiceRequest(Guid compoundId, ServiceRequestModel model, IEnumerable<ServiceAttachmentModel> attachments = null);
        Task<OperationState> UpdateServiceRequest(Guid requestId, Guid compoundId, ServiceUpdateModel model, IEnumerable<ServiceAttachmentModel> attachments = null);
        Task<OperationState> RateServiceByOwner(Guid requestId, ServiceEvaluationModel rateModel);
        Task<PagedOutput<ServiceRequestList>> GetOwnerRequests(Guid compoundId, OwnerServiceFilter filter);
        Task<OwnerServiceViewModel> GetOwnerRequest(Guid requestId, string lang);
        Task UpdateServiceIcon(Guid serviceTypeId, string fileName, byte[] fileBytes);
        Task<CompoundServiceOutput[]> GetCompoundsServices(CompoundServiceInput compounds);
    }

    public class ServiceRequestService : BaseService, IServiceRequestService
    {

        private readonly IServiceTypeRepository _servTypeRep;
        private readonly ICompoundServiceRepository _compServRep;
        private readonly IServiceRequestRepository _serReqRep;
        private readonly IServiceRequestSubTypeRepository _serReqSubTypeRep;
        private readonly UserIdentity _user;
        private readonly IS3Service _s3Service;
        private readonly IHubContext<CounterHub> _hub;
        private readonly IPushNotificationService _pushNotificationService;

        // todo : get from configuration
        string s3Url = "http://circle360.s3.amazonaws.com/";

        public ServiceRequestService(IUnitOfWork unitOfWork, IMapper mapper,
                IServiceTypeRepository servTypeRep, ICompoundServiceRepository compServRep,
                IServiceRequestRepository serReqRep, UserIdentity user
                , IS3Service s3Service, IHubContext<CounterHub> hub, IServiceRequestSubTypeRepository serReqSubTypeRep,
                IPushNotificationService pushNotificationService) : base(unitOfWork, mapper)
        {
            _servTypeRep = servTypeRep;
            _compServRep = compServRep;
            _serReqRep = serReqRep;
            _user = user;
            _s3Service = s3Service;
            _hub = hub;
            _serReqSubTypeRep = serReqSubTypeRep;
            _pushNotificationService = pushNotificationService;
        }

        #region Admin
        public async Task<CompoundServiceModel[]> GetServiceAssignment(Guid compoundId)
        {
            var assignments = await _servTypeRep.Table.Include(x => x.CompoundServices).ThenInclude(s => s.ServiceSubTypes)
                    .Where(x => !x.IsFixed)
                    .Select(z => new CompoundServiceModel
                    {
                        ServiceTypeId = z.ServiceTypeId,
                        ServiceNameArabic = z.ArabicName,
                        ServiceNameEnglish = z.EnglishName,
                        ServiceOrder = z.Order,
                        IsFixed = z.IsFixed,
                        Selected = z.CompoundServices.Any(z => z.CompoundId == compoundId),
                        AssignOrder = z.CompoundServices.Any(z => z.CompoundId == compoundId) ?
                    z.CompoundServices.First(z => z.CompoundId == compoundId).Order
                    : 0,
                        ServiceSubTypes = z.CompoundServices.First(z => z.CompoundId == compoundId).ServiceSubTypes.Where(s => s.CompoundService.CompoundId == compoundId)
                        .Select(s => new ServiceSubTypeOutputViewModel
                        {
                            ServiceSubTypeId = s.ServiceSubTypeId,
                            CompoundServiceId = s.CompoundServiceId,
                            ServiceTypeId = z.ServiceTypeId,
                            EnglishName = s.EnglishName,
                            ArabicName = s.ArabicName,
                            Cost = s.Cost,
                            Order = s.Order
                        }).ToList()
                    }).ToArrayAsync();
            return assignments;
        }

        public async Task<OperationState> UpdateServiceAssignment(Guid compoundId, CompoundServiceModel[] assignments)
        {
            var oldAssignments = _compServRep.GetMany(x => x.CompoundId == compoundId).ToList();
            var deletedAssigns = oldAssignments.Where(z => assignments.Any(y => y.ServiceTypeId == z.ServiceTypeId && !y.Selected));
            foreach (var assign in deletedAssigns)
            {
                assign.IsDeleted = true;
                assign.IsActive = false;
                //_compServRep.Delete(assign);
            }

            var addedAssigns = assignments.Where(z => z.Selected && oldAssignments.All(y => y.ServiceTypeId != z.ServiceTypeId));
            foreach (var assign in addedAssigns)
            {
                var newAssign = mapper.Map<Core.Models.CompoundService>(assign);
                newAssign.CompoundId = compoundId;
                _compServRep.Add(newAssign);
            }
            var updatedAssigns = assignments.Where(z => z.Selected && oldAssignments.Any(y =>
                    y.ServiceTypeId == z.ServiceTypeId));
            foreach (var assign in updatedAssigns)
            {
                var updateAssign = oldAssignments.FirstOrDefault(z => z.ServiceTypeId == assign.ServiceTypeId);
                if (updateAssign == null) continue;
                updateAssign.Order = updateAssign.Order;
                _compServRep.Update(updateAssign);
            }
            return (await unitOfWork.CommitAsync()) > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<PagedOutput<ServiceRequestList>> GetServiceRequests(ServiceRequestFilter filter)
        {
            var requests = _serReqRep.Table.Include(x => x.ServiceType)
                .Include(s => s.ServiceRequestSubTypes).ThenInclude(s => s.ServiceSubType)
                .Include(z => z.OwnerRegistration).Include(x => x.CompoundUnit)
                .ThenInclude(x => x.CompoundGroup).ThenInclude(x => x.Compound)
                .Where(y => y.CompoundUnit.CompoundGroup.CompoundId == filter.CompoundId);

            if (!string.IsNullOrEmpty(filter.SearchText))
                requests = requests.Where(z => z.ServiceType.ArabicName.Contains(filter.SearchText) ||
                z.ServiceType.EnglishName.Contains(filter.SearchText) ||
                z.Comment.Contains(filter.SearchText) || z.Note.Contains(filter.SearchText));

            requests = requests.Where(x => filter.Status == null || x.Status == filter.Status.Value)
                    .Where(x => !filter.ServiceTypeIds.Any() || filter.ServiceTypeIds.Contains(x.ServiceTypeId))
                    .Where(x => filter.From == null || x.Date >= filter.From.Value)
                    .Where(x => filter.To == null || x.Date <= filter.To.Value);

            // apply sorting
            var columns = new Dictionary<string, Expression<Func<ServiceRequest, object>>>()
            {
                ["requestedBy"] = v => v.OwnerRegistration.Name,
                ["status"] = v => v.Status,
                ["postDate"] = v => v.PostTime,
                ["postTime"] = v => v.PostTime,
                ["serviceTypeEnglish"] = v => v.ServiceType.EnglishName,
                ["serviceTypeArabic"] = v => v.ServiceType.ArabicName
            };
            requests = requests.ApplySorting(filter, columns);

            var filteredRequests = requests
                    .Select(z => new ServiceRequestList()
                    {
                        ServiceRequestId = z.ServiceRequestId,
                        RequestNumber = z.RequestNumber,
                        ServiceTypeId = z.ServiceTypeId,
                        ServiceTypeArabic = z.ServiceType.ArabicName,
                        ServiceTypeEnglish = z.ServiceType.EnglishName,
                        Status = z.Status,
                        PostTime = z.PostTime,
                        From = z.From,
                        To = z.To,
                        Rate = z.Rate,
                        RequestedBy = z.OwnerRegistration.Name,
                        Email = z.OwnerRegistration.Email,
                        Phone = z.OwnerRegistration.Phone,
                        Date = z.Date.ToString("dd-MM-yyyy"),
                        Icon = s3Url + z.ServiceType.Icon,
                        UnitName = z.CompoundUnit.Name,
                        ServiceSubTypesTotalCost = z.ServiceSubTypesTotalCost,
                        ServiceSubTypes = z.ServiceRequestSubTypes.Select(s => new ServiceRequestSubTypeOutPutViewModel
                        {
                            ServiceRequestSubTypeId = s.ServiceRequestSubTypeId,
                            ServiceSubTypeId = s.ServiceSubTypeId,
                            ServiceRequestId = z.ServiceRequestId,
                            ServiceSubTypeCost = s.ServiceSubTypeCost,
                            ServiceSubTypeQuantity = s.ServiceSubTypeQuantity,
                            Order = s.Order,
                            ServiceSubTypeEnglish = s.ServiceSubType.EnglishName,
                            ServiceSubTypeArabic = s.ServiceSubType.ArabicName
                        }).ToList()
                    });

            var pageRequests = await filteredRequests.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize)
                    .ToListAsync();
            return new PagedOutput<ServiceRequestList>
            {
                TotalCount = filteredRequests.Count(),
                Result = pageRequests
            };
        }

        public async Task<OperationState> UpdateRequestStatus(Guid requestId, short status)
        {
            var request = _serReqRep.Get(z => z.ServiceRequestId == requestId);
            if (request == null) return OperationState.NotExists;
            request.Status = status;
            request.UpdateStatusTime = DateTime.Now;
            _serReqRep.Update(request);
            var result = (await unitOfWork.CommitAsync()) > 0 ? OperationState.Updated : OperationState.None;
            await _hub.Clients.All.SendAsync("UpdatePendingListCount", true);
           
            await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
            {
                NotificationType = status == 1 ? PushNotificationType.ServiceAccepted : PushNotificationType.ServiceCanceled,
                RecordId = request.ServiceRequestId
            });
            return result;
        }

        public async Task<OperationState> AddRequestComment(Guid requestId, AddCommentModel commentModel)
        {
            var request = _serReqRep.Get(z => z.ServiceRequestId == requestId);
            if (request == null) return OperationState.NotExists;
            request.Comment = commentModel.Comment;
            if (commentModel.Status.HasValue)
            {
                request.Status = commentModel.Status.Value;
                request.UpdateStatusTime = DateTime.Now;
            }
            _serReqRep.Update(request);
            var result = (await unitOfWork.CommitAsync()) > 0 ? OperationState.Updated : OperationState.None;
            await _hub.Clients.All.SendAsync("UpdatePendingListCount", true);
            
            await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
            {
                NotificationType = commentModel.Status.HasValue ? PushNotificationType.ServiceAcceptedWithComment : PushNotificationType.ServiceComment,
                RecordId = request.ServiceRequestId
            });
            return result;
        }

        public async Task<ServiceRequestViewModel> GetServiceRequest(Guid requestId)
        {
            return await _serReqRep.Table.Include(z => z.ServiceType)
                .Include(s => s.ServiceRequestSubTypes).ThenInclude(s => s.ServiceSubType)
                .Include(z => z.OwnerRegistration).Include(z => z.CompoundUnit)
                .Select(x => new ServiceRequestViewModel
                {
                    Comment = x.Comment,
                    ServiceRequestId = x.ServiceRequestId,
                    PostTime = x.PostTime,
                    From = x.From,
                    To = x.To,
                    OwnerComment = x.OwnerComment,
                    RequestNumber = x.RequestNumber,
                    Status = x.Status,
                    Note = x.Note,
                    Rate = x.Rate,
                    PresenterRate = x.PresenterRate,
                    ServiceTypeArabic = x.ServiceType.ArabicName,
                    ServiceTypeEnglish = x.ServiceType.EnglishName,
                    UnitName = x.CompoundUnit.Name,
                    OwnerName = x.OwnerRegistration.Name,
                    OwnerPhone = x.OwnerRegistration.Phone,
                    Icon = s3Url + x.ServiceType.Icon,
                    Record = x.Record,
                    Attachments = x.ServiceAttachments.Select(x => x.Path).ToList(),
                    ServiceSubTypesTotalCost = x.ServiceSubTypesTotalCost,
                    ServiceSubTypes = x.ServiceRequestSubTypes.Select(s => new ServiceRequestSubTypeOutPutViewModel
                    {
                        ServiceRequestSubTypeId = s.ServiceRequestSubTypeId,
                        ServiceSubTypeId = s.ServiceSubTypeId,
                        ServiceRequestId = x.ServiceRequestId,
                        ServiceSubTypeCost = s.ServiceSubTypeCost,
                        ServiceSubTypeQuantity = s.ServiceSubTypeQuantity,
                        Order = s.Order,
                        ServiceSubTypeEnglish = s.ServiceSubType.EnglishName,
                        ServiceSubTypeArabic = s.ServiceSubType.ArabicName
                    }).ToList()
                }).FirstOrDefaultAsync(z => z.ServiceRequestId == requestId);
        }
        #endregion

        #region Owner
        public async Task<ServiceTypeModel[]> GetCompoundServices(Guid compoundId, string lang)
        {
            return await _servTypeRep.Table.Include(z => z.CompoundServices)
                    .Where(z => z.IsFixed || z.CompoundServices.Any(x => x.CompoundId == compoundId))
                    .Select(z => new ServiceTypeModel
                    {
                        ServiceTypeId = z.ServiceTypeId,
                        CompoundId = compoundId,
                        Name = lang == "en" ? z.EnglishName : z.ArabicName,
                        Order = z.Order,
                        IsFixed = z.IsFixed,
                        Icon = s3Url + z.Icon
                    }).ToArrayAsync();
        }

        public async Task<OperationState> CancelRequestByOwner(Guid requestId, CancelRequestModel cancelModel)
        {
            var request = _serReqRep.Get(z => z.ServiceRequestId == requestId);
            if (request == null) return OperationState.NotExists;
            if (request.OwnerRegistrationId != cancelModel.OwnerRegistrationId) return OperationState.NotAllowed;
            if (request.Status != 0) throw new BusinessException("Only pending requests can be cancelled");
            request.Status = 2;
            request.CancelType = 0;
            request.UpdateStatusTime = DateTime.UtcNow;
            request.UpdateStatusBy = cancelModel.OwnerRegistrationId;
            _serReqRep.Update(request);
            return (await unitOfWork.CommitAsync()) > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<OperationState> AddServiceRequest(Guid compoundId, ServiceRequestModel request, IEnumerable<ServiceAttachmentModel> attachments = null)
        {
            if (request.ServiceRequestSubTypes.Count < 1)
            {
                throw new BusinessException("Service Request must include Service SubType");
            }
            if (request == null) return OperationState.None;
            var model = mapper.Map<ServiceRequest>(request);
            model.PostTime = DateTime.UtcNow;
            var maxRequest = _serReqRep.Table.Select(z => z.RequestNumber).DefaultIfEmpty().Max();
            model.RequestNumber = maxRequest + 1;
            model.CompoundId = compoundId;
            model.ServiceSubTypesTotalCost = request.ServiceRequestSubTypes.Sum(s => s.ServiceSubTypeCost * s.ServiceSubTypeQuantity);
            model.UpdateStatusTime = DateTime.UtcNow;
            model.UpdateStatusBy = _user.Id.Value;
            if (request.Record != null)
            {
                using var ms = new MemoryStream();
                await request.Record.CopyToAsync(ms);
                var fileUrl = _s3Service.UploadFile("Services", request.Record.FileName, ms.ToArray());
                model.Record = fileUrl;
            }
            foreach (var attachment in attachments)
            {
                var attachUrl = _s3Service.UploadFile("Services", attachment.FileName, attachment.File);
                model.ServiceAttachments.Add(new ServiceAttachment
                {
                    Path = attachUrl,
                    IsActive = true,
                    IsDeleted = false
                });
            }
            _serReqRep.Add(model);
            return (await unitOfWork.CommitAsync()) > 0 ? OperationState.Created : OperationState.None;
        }

        public async Task<OperationState> UpdateServiceRequest(Guid requestId, Guid compoundId, ServiceUpdateModel request, IEnumerable<ServiceAttachmentModel> attachments = null)
        {
            if (request.ServiceRequestSubTypes.Count < 1)
            {
                throw new BusinessException("Service Request must include Service SubType");
            }
            if (request == null) return OperationState.None;
            var model = await _serReqRep.GetByIdAsync(requestId);

            foreach (var subType in model.ServiceRequestSubTypes)
            {
                _serReqSubTypeRep.Delete(subType);
            }

            mapper.Map(request, model);
            model.PostTime = DateTime.UtcNow;
            var maxRequest = _serReqRep.Table.Select(z => z.RequestNumber).DefaultIfEmpty().Max();
            model.ServiceSubTypesTotalCost = request.ServiceRequestSubTypes.Sum(s => s.ServiceSubTypeCost * s.ServiceSubTypeQuantity);
            model.UpdateStatusTime = DateTime.UtcNow;
            model.UpdateStatusBy = _user.Id.Value;
            if (request.Record != null)
            {
                using var ms = new MemoryStream();
                await request.Record.CopyToAsync(ms);
                var fileUrl = _s3Service.UploadFile("Services", request.Record.FileName, ms.ToArray());
                model.Record = fileUrl;
            }
            if (model.ServiceAttachments != null && model.ServiceAttachments.Any())
                model.ServiceAttachments.Clear();
            foreach (var attachment in attachments)
            {
                var attachUrl = _s3Service.UploadFile("Services", attachment.FileName, attachment.File);
                model.ServiceAttachments.Add(new ServiceAttachment
                {
                    Path = attachUrl,
                    IsActive = true,
                    IsDeleted = false
                });
            }
            _serReqRep.Update(model);
            return (await unitOfWork.CommitAsync()) > 0 ? OperationState.Created : OperationState.None;
        }

        public async Task<OperationState> RateServiceByOwner(Guid requestId, ServiceEvaluationModel rateModel)
        {
            var request = _serReqRep.Get(z => z.ServiceRequestId == requestId);
            if (request == null) return OperationState.NotExists;
            if (request.OwnerRegistrationId != rateModel.OwnerRegistrationId) return OperationState.NotAllowed;
            if (request.Status != 1) throw new BusinessException("Only done requests can be rated");
            if (!string.IsNullOrEmpty(rateModel.Comment))
                request.OwnerComment = rateModel.Comment;
            request.Rate = rateModel.ServiceRate;
            request.PresenterRate = rateModel.ProviderRate;
            request.UpdateStatusTime = DateTime.UtcNow;
            request.UpdateStatusBy = rateModel.OwnerRegistrationId;
            _serReqRep.Update(request);
            return (await unitOfWork.CommitAsync()) > 0 ? OperationState.Updated : OperationState.None;
        }

        public async Task<PagedOutput<ServiceRequestList>> GetOwnerRequests(Guid compoundId, OwnerServiceFilter filter)
        {
            var requests = _serReqRep.Table.Include(x => x.ServiceType)
                .Include(s => s.ServiceRequestSubTypes).ThenInclude(s => s.ServiceSubType)
                .Include(z => z.OwnerRegistration).Include(x => x.CompoundUnit)
                .ThenInclude(x => x.CompoundGroup).ThenInclude(x => x.Compound)
                .Where(y => y.CompoundUnit.CompoundGroup.CompoundId == compoundId);

            if (filter.IsUpcoming)
                requests = requests.Where(z => z.Status == 0);
            else
                requests = requests.Where(z => z.Status != 0);

            if (filter.ServiceTypes.Any())
                requests = requests.Where(z => filter.ServiceTypes.Any(x => z.ServiceTypeId == x));

            var filteredRequests = requests
                    .Where(x => x.OwnerRegistrationId == filter.OwnerRegistrationId ||
                    x.OwnerRegistration.MainRegistrationId == filter.OwnerRegistrationId)
                    .Where(x => filter.DateFrom == null || x.Date >= filter.DateFrom.Value)
                    .Where(x => filter.DateTo == null || x.Date <= filter.DateTo.Value)
                    .OrderBy(x => x.Date)
                    .Select(z => new ServiceRequestList()
                    {
                        ServiceRequestId = z.ServiceRequestId,
                        RequestNumber = z.RequestNumber,
                        ServiceTypeArabic = z.ServiceType.ArabicName,
                        ServiceTypeEnglish = z.ServiceType.EnglishName,
                        Status = z.Status,
                        PostTime = z.PostTime,
                        RequestedBy = z.OwnerRegistration.Name,
                        Date = z.Date.ToString("yyyy-MM-dd"),
                        Icon = "http://circle360.s3.amazonaws.com/" + z.ServiceType.Icon,
                        ServiceTypeId = z.ServiceTypeId,
                        UnitName = z.CompoundUnit.Name,
                        ServiceSubTypesTotalCost = z.ServiceSubTypesTotalCost,
                        ServiceSubTypes = z.ServiceRequestSubTypes.Select(s => new ServiceRequestSubTypeOutPutViewModel
                        {
                            ServiceRequestSubTypeId = s.ServiceRequestSubTypeId,
                            ServiceSubTypeId = s.ServiceSubTypeId,
                            ServiceRequestId = z.ServiceRequestId,
                            ServiceSubTypeCost = s.ServiceSubTypeCost,
                            ServiceSubTypeQuantity = s.ServiceSubTypeQuantity,
                            Order = s.Order,
                            ServiceSubTypeEnglish = s.ServiceSubType.EnglishName,
                            ServiceSubTypeArabic = s.ServiceSubType.ArabicName
                        }).ToList()
                    }).OrderByDescending(s => s.PostTime);
            var pageRequests = await filteredRequests.Skip(filter.PageNumber * filter.PageSize).Take(filter.PageSize)
                    .ToListAsync();
            return new PagedOutput<ServiceRequestList>
            {
                TotalCount = filteredRequests.Count(),
                Result = pageRequests
            };
        }

        public async Task<OwnerServiceViewModel> GetOwnerRequest(Guid requestId, string lang)
        {
            return await _serReqRep.Table.Include(z => z.ServiceType)
                .Include(s => s.ServiceRequestSubTypes).ThenInclude(s => s.ServiceSubType)
                .Include(z => z.CompoundUnit)
                .Include(z => z.ServiceAttachments)
                .Where(z => z.ServiceRequestId == requestId).Select(z => new OwnerServiceViewModel
                {
                    ServiceType = lang == "en" ? z.ServiceType.EnglishName : z.ServiceType.ArabicName,
                    ServiceTypeId = z.ServiceTypeId,
                    Icon = z.ServiceType.Icon,
                    Attachments = z.ServiceAttachments.Select(x => x.Path).ToList(),
                    PostTime = z.PostTime,
                    Date = z.Date.ToString("yyyy-MM-dd"),
                    From = z.From.ToString("hh:mm:ss"),
                    To = z.To.ToString("hh:mm:ss"),
                    Note = z.Note,
                    RequestNumber = z.RequestNumber,
                    Record = z.Record,
                    Rate = z.Rate,
                    PresenterRate = z.PresenterRate,
                    Comment = z.Comment,
                    OwnerComment = z.OwnerComment,
                    CanEdit = z.OwnerRegistrationId == _user.Id && z.Status == 0,
                    CanRate = z.OwnerRegistrationId == _user.Id && z.Status == 1,
                    UnitName = z.CompoundUnit.Name,
                    Status = z.Status,
                    ServiceSubTypesTotalCost = z.ServiceSubTypesTotalCost,
                    ServiceSubTypes = z.ServiceRequestSubTypes.Select(s => new ServiceRequestSubTypeOutPutViewModel
                    {
                        ServiceRequestSubTypeId = s.ServiceRequestSubTypeId,
                        ServiceSubTypeId = s.ServiceSubTypeId,
                        ServiceRequestId = z.ServiceRequestId,
                        ServiceSubTypeCost = s.ServiceSubTypeCost,
                        ServiceSubTypeQuantity = s.ServiceSubTypeQuantity,
                        Order = s.Order,
                        ServiceSubTypeEnglish = s.ServiceSubType.EnglishName,
                        ServiceSubTypeArabic = s.ServiceSubType.ArabicName
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        #endregion

        #region ServiceType
        public async Task UpdateServiceIcon(Guid serviceTypeId, string fileName, byte[] fileBytes)
        {
            var _serviceType = await _servTypeRep.GetByIdAsync(serviceTypeId);
            if (_serviceType == null) throw new BusinessException("Service Type not found");
            string newFileName = "";
            var logoUrl = _s3Service.UploadFile("​service", fileName, fileBytes, out newFileName);

            if (!string.IsNullOrEmpty(logoUrl))
            {
                _serviceType.Icon = newFileName;
            }
            await unitOfWork.CommitAsync();
        }

        public async Task<CompoundServiceOutput[]> GetCompoundsServices(CompoundServiceInput compounds)
        {
            var services = await _compServRep.Table.Include(x => x.ServiceType).Include(s => s.ServiceSubTypes)
                .Where(x => x.ServiceType.IsFixed || compounds.Compounds.Contains(x.CompoundId))
                .ToListAsync();
            var model = services.Select(z => new CompoundServiceOutput
            {
                ServiceTypeId = z.ServiceTypeId,
                ArabicName = z.ServiceType.ArabicName,
                EnglishName = z.ServiceType.EnglishName,
                ServiceSubTypes = z.ServiceSubTypes.Select(s => new ServiceSubTypeOutputViewModel
                {
                    ServiceSubTypeId = s.ServiceSubTypeId,
                    CompoundServiceId = s.CompoundServiceId,
                    ServiceTypeId = z.ServiceTypeId,
                    EnglishName = s.EnglishName,
                    ArabicName = s.ArabicName,
                    Cost = s.Cost,
                    Order = s.Order
                }).ToList()
            }).ToArray();

            return model;
        }
        #endregion

    }
}
