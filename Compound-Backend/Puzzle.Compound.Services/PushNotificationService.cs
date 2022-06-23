using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Extensions;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.PushNotifications;
using Puzzle.Compound.Services.Exceptions;

namespace Puzzle.Compound.Services
{
    public interface IPushNotificationService
    {
        Task<NotificationSchedule> CreatePushNotification(PushNotificationAddViewModel pushNotification);
        Task<List<NotificationScheduleViewModel>> GetAllNotifications();
        Task<bool> ResetPushNotification(Guid id);
    }

    public class PushNotificationService : BaseService, IPushNotificationService
    {
        private readonly UserIdentity _userIdentity;
        private readonly IOwnerRegistrationRepository _ownerRegistrationRepository;
        private readonly IVisitorRequestRepository _visitRequestRepository;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly ICompoundNotificationRepository _compoundNotificationRepository;
        private readonly IIssueRequestRepository _issueRequestRepository;
        private readonly ICompoundAdRepository _compoundAdRepository;
        private readonly ICompoundNewsRepository _compoundNewsRepository;
        private readonly IOptionsSnapshot<NotificationMessages> _options;
        private readonly IOptionsSnapshot<RouteAndroid> _routeAndroid;
        private readonly INotificationScheduleRepository _notificationScheduleRepository;
        //

        public PushNotificationService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IOptionsSnapshot<NotificationMessages> options,
            IOptionsSnapshot<RouteAndroid> routeAndroid,
            INotificationScheduleRepository notificationScheduleRepository,
            UserIdentity userIdentity,
            IOwnerRegistrationRepository ownerRegistrationRepository,
            IVisitorRequestRepository visitRequestRepository,
            IServiceRequestRepository serviceRequestRepository,
            ICompoundNotificationRepository compoundNotificationRepository,
            IIssueRequestRepository issueRequestRepository,
            ICompoundAdRepository compoundAdRepository,
            ICompoundNewsRepository compoundNewsRepository)
            : base(unitOfWork, mapper)
        {
            _userIdentity = userIdentity;
            _ownerRegistrationRepository = ownerRegistrationRepository;
            _visitRequestRepository = visitRequestRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _compoundNotificationRepository = compoundNotificationRepository;
            _issueRequestRepository = issueRequestRepository;
            _compoundAdRepository = compoundAdRepository;
            _compoundNewsRepository = compoundNewsRepository;
            _options = options;
            _routeAndroid = routeAndroid;
            _notificationScheduleRepository = notificationScheduleRepository;
        }

        public async Task<NotificationSchedule> CreatePushNotification(PushNotificationAddViewModel pushNotification)
        {
            return await SaveNotification(pushNotification);
        }

        public async Task<List<NotificationScheduleViewModel>> GetAllNotifications()
        {
            var notificationData = await _notificationScheduleRepository.TableNoTracking
                .Include(q => q.NotificationUsers)
                .ToListAsync();

            return mapper.Map<List<NotificationScheduleViewModel>>(notificationData);
        }

        public async Task<bool> ResetPushNotification(Guid id)
        {
            var ExistRecord = _notificationScheduleRepository.GetById(id)
                ?? throw new BusinessException("Notification schedual is not exist.");

            ExistRecord.IsActive = true;
            ExistRecord.HasSend = false;
            ExistRecord.HasError = false;
            ExistRecord.IsDeleted = false;

            _notificationScheduleRepository.Update(ExistRecord);
            var excuted = await unitOfWork.CommitAsync();

            return excuted > 0;
        }

        private async Task<NotificationSchedule> SaveNotification(PushNotificationAddViewModel pushNotification)
        {
            if (pushNotification.RecordId == Guid.Empty) return null;

            var checkRecordExist = _notificationScheduleRepository.Get(q =>
            q.RecordId == pushNotification.RecordId &&
            q.NotificationType == (int)pushNotification.NotificationType &&
            q.NotificationType != (int)PushNotificationType.News);

            if (checkRecordExist != null) return null;

            var notificationSchedule = new NotificationSchedule()
            {
                NotificationType = (int)pushNotification.NotificationType,
                IsActive = true,
                RecordId = pushNotification.RecordId,
                CreatedDate = DateTime.Now,
                ScheduleDateTime = DateTime.Now,
                PostDatetime = DateTime.Now,
                CreatedBy = _userIdentity?.Id.ToString(),
                PrioritySend = (int)SendPriority.Low
            };

            switch (pushNotification.NotificationType)
            {
                case PushNotificationType.RegisteredUserApproved://
                    RegisteredUserApproved(notificationSchedule);
                    break;
                case PushNotificationType.VisitApprove://
                    Visit(notificationSchedule, _options.Value.VisitApproveAr, _options.Value.VisitApproveEn);
                    break;
                case PushNotificationType.VisitCanceled://
                    Visit(notificationSchedule, _options.Value.VisitCanceledAr, _options.Value.VisitCanceledEn);
                    break;
                case PushNotificationType.VisitRequestedOnGate://
                    Visit(notificationSchedule, _options.Value.VisitRequestedOnGateAr, _options.Value.VisitRequestedOnGateEn);
                    break;
                case PushNotificationType.ServiceAccepted://
                    Service(notificationSchedule, _options.Value.ServiceAcceptedAr, _options.Value.ServiceAcceptedEn);
                    break;
                case PushNotificationType.ServiceAcceptedWithComment://
                    Service(notificationSchedule, _options.Value.ServiceAcceptedWithCommentAr, _options.Value.ServiceAcceptedWithCommentEn);
                    break;
                case PushNotificationType.ServiceComment://
                    Service(notificationSchedule, _options.Value.ServiceCommentAr, _options.Value.ServiceCommentEn);
                    break;
                case PushNotificationType.ServiceCanceled://
                    Service(notificationSchedule, _options.Value.ServiceCanceledAr, _options.Value.ServiceCanceledEn);
                    break;
                case PushNotificationType.IssueAccepted://
                    Issue(notificationSchedule, _options.Value.IssueAcceptedAr, _options.Value.IssueAcceptedEn);
                    break;
                case PushNotificationType.IssueAcceptedWithComment://
                    Issue(notificationSchedule, _options.Value.IssueAcceptedWithCommentAr, _options.Value.IssueAcceptedWithCommentEn);
                    break;
                case PushNotificationType.IssueComment://
                    Issue(notificationSchedule, _options.Value.IssueCommentAr, _options.Value.IssueCommentEn);
                    break;
                case PushNotificationType.IssueCanceled://
                    Issue(notificationSchedule, _options.Value.IssueCanceledAr, _options.Value.IssueCanceledEn);
                    break;
                case PushNotificationType.Advertise://
                    Advertise(notificationSchedule);
                    break;
                case PushNotificationType.Notification://
                    Notification(notificationSchedule);
                    break;
                case PushNotificationType.News://
                    var isPublished = News(notificationSchedule, checkRecordExist);
                    if (!isPublished) return null;
                    break;
                case PushNotificationType.ContractEnd:
                    ContractEnd(notificationSchedule);
                    break;
                case PushNotificationType.SubAccountDeleted://
                    SubAccount(notificationSchedule, _options.Value.SubAccountDeletedAr, _options.Value.SubAccountDeletedEn);
                    break;
                case PushNotificationType.SubAccountCanceled://
                    SubAccount(notificationSchedule, _options.Value.SubAccountCanceledAr, _options.Value.SubAccountCanceledEn);
                    break;
                case PushNotificationType.SubAccountActive://
                    SubAccount(notificationSchedule, _options.Value.SubAccountActiveAr, _options.Value.SubAccountActiveEn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pushNotification.NotificationType), pushNotification.NotificationType, null);
            }

            _notificationScheduleRepository.Add(notificationSchedule);

            await unitOfWork.CommitAsync();

            return notificationSchedule;
        }

        #region Methods
        private string ContentAr, ContentEn, route_android;

        private void RegisteredUserApproved(NotificationSchedule notification)
        {
            var recordData = _ownerRegistrationRepository.GetById(notification.RecordId)
                             ?? throw new BusinessException("Registered user is not exist");

            ContentAr = _options.Value.ActivateAccountAr;
            ContentEn = _options.Value.ActivateAccountEn;

            var Data = new Dictionary<string, string>()
            {
                { nameof(route_android), _routeAndroid.Value.GetActivity(notification.NotificationType)},
                { nameof(_options.Value.TitleAr),_options.Value.TitleAr},
                { nameof(_options.Value.TitleEn),_options.Value.TitleEn},
                { nameof(ContentAr),ContentAr},
                { nameof(ContentEn),ContentEn},
                //
                { nameof(notification.RecordId),notification.RecordId.ToString()},
                { nameof(notification.NotificationType),notification.NotificationType.ToString()}
            };

            notification.NotificationData = JsonConvert.SerializeObject(Data);
            notification.NotificationText = ContentAr;
            notification.NotificationTextEn = ContentEn;
            notification.PrioritySend = (int)SendPriority.Medium;
            notification.NotificationUsers = new List<NotificationUser>() { new NotificationUser { OwnerRegistrationId = notification.RecordId } };
        }
        private void Visit(NotificationSchedule notification, string messageAr, string messageEn)
        {
            var recordData = _visitRequestRepository.GetById(notification.RecordId)
                            ?? throw new BusinessException("Visit request is not exist");

            var visitTypeAr = (int)recordData.VisitType > 2 ? recordData.VisitType.GetDescription() : "الزائر";
            var visitTypeEn = (int)recordData.VisitType > 2 ? recordData.VisitType.ToString() : "Visitor";
            ContentAr = string.Format(messageAr, visitTypeAr, recordData.VisitorName);
            ContentEn = string.Format(messageEn, visitTypeEn, recordData.VisitorName);
            var Data = new Dictionary<string, string>()
            {
                { nameof(route_android), _routeAndroid.Value.GetActivity(notification.NotificationType)},
                { nameof(_options.Value.TitleAr),_options.Value.TitleAr},
                { nameof(_options.Value.TitleEn),_options.Value.TitleEn},
                { nameof(ContentAr),ContentAr},
                { nameof(ContentEn),ContentEn},
                //
                { nameof(notification.RecordId),notification.RecordId.ToString() },
                { nameof(notification.NotificationType),notification.NotificationType.ToString() },
                { nameof(recordData.CompoundUnitId) , recordData.CompoundUnitId.ToString() },
                { nameof(recordData.CompoundId),recordData.CompoundId.ToString() }
            };

            notification.NotificationData = JsonConvert.SerializeObject(Data);
            notification.NotificationText = ContentAr;
            notification.NotificationTextEn = ContentEn;
            notification.PrioritySend = (int)SendPriority.High;
            notification.NotificationUsers = new List<NotificationUser>() { new NotificationUser { OwnerRegistrationId = recordData.OwnerRegistrationId } };
        }
        private void SubAccount(NotificationSchedule notification, string messageAr, string messageEn)
        {
            var recordData = _ownerRegistrationRepository.GetById(notification.RecordId)
                             ?? throw new BusinessException("Sub Account user is not exist");

            ContentAr = string.Format(messageAr, recordData.Name);
            ContentEn = string.Format(messageEn, recordData.Name);
            var Data = new Dictionary<string, string>()
            {
                { nameof(route_android), _routeAndroid.Value.GetActivity(notification.NotificationType)},
                { nameof(_options.Value.TitleAr),_options.Value.TitleAr},
                { nameof(_options.Value.TitleEn),_options.Value.TitleEn},
                { nameof(ContentAr),ContentAr},
                { nameof(ContentEn),ContentEn},
                //
                { nameof(notification.RecordId),notification.RecordId.ToString()},
                { nameof(notification.NotificationType),notification.NotificationType.ToString()}
            };

            notification.NotificationData = JsonConvert.SerializeObject(Data);
            notification.NotificationText = ContentAr;
            notification.NotificationTextEn = ContentEn;
            notification.PrioritySend = (int)SendPriority.Medium;
            notification.NotificationUsers = new List<NotificationUser>() { new NotificationUser { OwnerRegistrationId = notification.RecordId } };
        }
        private void Service(NotificationSchedule notification, string messageAr, string messageEn)
        {
            var recordData = _serviceRequestRepository.GetById(notification.RecordId)
                 ?? throw new BusinessException("Service request is not exist");

            ContentAr = string.Format(messageAr, recordData.ServiceType.ArabicName);
            ContentEn = string.Format(messageEn, recordData.ServiceType.EnglishName);
            var Data = new Dictionary<string, string>()
            {
                { nameof(route_android), _routeAndroid.Value.GetActivity(notification.NotificationType)},
                { nameof(_options.Value.TitleAr),_options.Value.TitleAr},
                { nameof(_options.Value.TitleEn),_options.Value.TitleEn},
                { nameof(ContentAr),ContentAr},
                { nameof(ContentEn),ContentEn},
                //
                { nameof(notification.RecordId),notification.RecordId.ToString() },
                { nameof(notification.NotificationType),notification.NotificationType.ToString() },
                { nameof(recordData.CompoundUnitId) , recordData.CompoundUnitId.ToString() },
                { nameof(recordData.CompoundId),recordData.CompoundId.ToString() }
            };

            notification.NotificationData = JsonConvert.SerializeObject(Data);
            notification.NotificationText = ContentAr;
            notification.NotificationTextEn = ContentEn;
            notification.PrioritySend = (int)SendPriority.Medium;
            notification.NotificationUsers = new List<NotificationUser>() { new NotificationUser { OwnerRegistrationId = recordData.OwnerRegistrationId } };
        }
        private void Notification(NotificationSchedule notification)
        {
            var recordData = _compoundNotificationRepository.GetById(notification.RecordId)
                          ?? throw new BusinessException("Compound Notification is not exist");

            ContentAr = string.Format(_options.Value.NotificationAr, recordData.ArabicTitle);
            ContentEn = string.Format(_options.Value.NotificationEn, recordData.EnglishTitle);
            var Data = new Dictionary<string, string>()
            {
                { nameof(route_android), _routeAndroid.Value.GetActivity(notification.NotificationType)},
                { nameof(_options.Value.TitleAr),_options.Value.TitleAr},
                { nameof(_options.Value.TitleEn),_options.Value.TitleEn},
                { nameof(ContentAr),ContentAr},
                { nameof(ContentEn),ContentEn},
                //
                { nameof(notification.RecordId),notification.RecordId.ToString() },
                { nameof(notification.NotificationType),notification.NotificationType.ToString() } ,
                { nameof(recordData.CompoundId),recordData.CompoundId.ToString() }
            };

            notification.NotificationData = JsonConvert.SerializeObject(Data);
            notification.NotificationText = ContentAr;
            notification.NotificationTextEn = ContentEn;
            notification.PrioritySend = (int)SendPriority.Low;
        }
        private void Issue(NotificationSchedule notification, string messageAr, string messageEn)
        {
            var recordData = _issueRequestRepository.GetById(notification.RecordId)
                 ?? throw new BusinessException("Issue request is not exist");

            ContentAr = string.Format(messageAr, recordData.IssueType.ArabicName);
            ContentEn = string.Format(messageEn, recordData.IssueType.EnglishName);
            var Data = new Dictionary<string, string>()
            {
                { nameof(route_android), _routeAndroid.Value.GetActivity(notification.NotificationType)},
                { nameof(_options.Value.TitleAr),_options.Value.TitleAr},
                { nameof(_options.Value.TitleEn),_options.Value.TitleEn},
                { nameof(ContentAr),ContentAr},
                { nameof(ContentEn),ContentEn},
                //
                { nameof(notification.RecordId),notification.RecordId.ToString() },
                { nameof(notification.NotificationType),notification.NotificationType.ToString() },
                { nameof(recordData.CompoundId),recordData.CompoundId.ToString() }
            };

            notification.NotificationData = JsonConvert.SerializeObject(Data);
            notification.NotificationText = ContentAr;
            notification.NotificationTextEn = ContentEn;
            notification.PrioritySend = (int)SendPriority.Medium;
            notification.NotificationUsers = new List<NotificationUser>() { new NotificationUser { OwnerRegistrationId = recordData.OwnerRegistrationId } };
        }
        private void Advertise(NotificationSchedule notification)
        {
            var recordData = _compoundAdRepository.GetById(notification.RecordId)
                         ?? throw new BusinessException("Compound Advertise is not exist");

            ContentAr = string.Format(_options.Value.AdvertiseAr, recordData.ArabicTitle);
            ContentEn = string.Format(_options.Value.AdvertiseEn, recordData.EnglishTitle);
            var Data = new Dictionary<string, string>()
            {
                { nameof(route_android), _routeAndroid.Value.GetActivity(notification.NotificationType)},
                { nameof(_options.Value.TitleAr),_options.Value.TitleAr},
                { nameof(_options.Value.TitleEn),_options.Value.TitleEn},
                { nameof(ContentAr),ContentAr},
                { nameof(ContentEn),ContentEn},
                //
                { nameof(notification.RecordId),notification.RecordId.ToString() },
                { nameof(notification.NotificationType),notification.NotificationType.ToString() } ,
            };

            notification.NotificationData = JsonConvert.SerializeObject(Data);
            notification.NotificationText = ContentAr;
            notification.NotificationTextEn = ContentEn;
            notification.PrioritySend = (int)SendPriority.Low;
        }
        private bool News(NotificationSchedule notification, NotificationSchedule current)
        {
            //IsActive //عدم نشر الخبر
            var recordData = _compoundNewsRepository.GetById(notification.RecordId)
                        ?? throw new BusinessException("Compound News is not exist");
            if (recordData.IsActive == false)
            {
                current.IsActive = false;
                _notificationScheduleRepository.Update(current);
                unitOfWork.Commit();

                return false;
            }

            ContentAr = string.Format(_options.Value.NewsAr, recordData.ArabicTitle);
            ContentEn = string.Format(_options.Value.NewsEn, recordData.EnglishTitle);

            var Data = new Dictionary<string, string>()
            {
                { nameof(route_android), _routeAndroid.Value.GetActivity(notification.NotificationType)},
                { nameof(_options.Value.TitleAr),_options.Value.TitleAr},
                { nameof(_options.Value.TitleEn),_options.Value.TitleEn},
                { nameof(ContentAr),ContentAr},
                { nameof(ContentEn),ContentEn},
                //
                { nameof(notification.RecordId),notification.RecordId.ToString() },
                { nameof(notification.NotificationType),notification.NotificationType.ToString() } ,
                { nameof(recordData.CompoundId),recordData.CompoundId.ToString() }
            };

            notification.NotificationData = JsonConvert.SerializeObject(Data);
            notification.NotificationText = ContentAr;
            notification.NotificationTextEn = ContentEn;
            notification.PrioritySend = (int)SendPriority.Low;
            notification.ScheduleDateTime = recordData.PublishDate;
            return true;
        }
        //
        private void ContractEnd(NotificationSchedule notification)
        {
        }

        #endregion
    }

}