using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Amazon;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Helpers;
using Puzzle.Compound.Common.Hubs;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.PushNotifications;
using Puzzle.Compound.Models.VisitRequest;
using Puzzle.Compound.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface IVisitRequestService
    {
        Task<VisitRequestOutputViewModel> AddAsync(VisitRequestAddInputViewModel visitRequest);
        Task UpdateAsync(VisitRequestUpdateViewModel visitRequest);
        Task<PagedOutput<VisitRequestFilterOutputViewModel>> FilterByCompoundAsync(VisitRequestFilterByCompoundInputViewModel model);
        Task ApproveAsync(VisitRequestApproveInputViewModel model);
        Task<VisitRequestOutputViewModel> GetByIdAsync(Guid id);
        Task CancelAsync(Guid id);
        Task<PagedOutput<VisitRequestFilterOutputViewModel>> FilterByCompanyAsync(VisitRequestFilterByCompanyInputViewModel model);
        Task<PagedOutput<VisitRequestFilterOutputViewModel>> FilterByUnitAsync(VisitRequestFilterByUnitInputViewModel model);
        Task<PagedOutput<VisitRequestFilterOutputViewModel>> FilterByUserAsync(VisitRequestFilterByUserInputViewModel model, Guid compoundId);
        Task RequestByQrCodeAsync(RequestByQrCodeViewModel model, int timezone);
        Task RequestByCodeAsync(RequestByQrCodeViewModel model, int timezone);
        Task RequestGateByQrCodeAsync(RequestVisitGateByCodeViewModel model, int timezone);
        Task RequestGateByCodeAsync(RequestVisitGateByCodeViewModel model, int timezone);
        Task<VisitRequestOutputViewModel> AddOwnerVisitAsync(Guid compoundUnitId);
        Task<VisitRequestOutputViewModel> AddCardVisitAsync(Guid compoundUnitId);
        Task EnableOrCancelCardVisitAsync(Guid visitId, bool isCancel);
        Task<PagedOutput<VisitRequestFilterOutputViewModel>> FilterByGateAsync(VisitRequestFilterByUserInputViewModel model, Guid gateId);
        Task<ConfirmVisitWithAttachResponse> ConfirmWithAttachments(Guid visitRequestId, List<VisitAttachmentViewModel> attachmemts);
        Task<ValidateVisitResponse> ValidateVisitByCode(Guid gateId, string code);
        Task<ValidateVisitResponse> ValidateVisitByQrCode(Guid gateId, string code);
    }

    public class VisitRequestService : BaseService, IVisitRequestService
    {
        private readonly IVisitorRequestRepository _visitRequestRepository;
        private readonly ICompoundGateRepository _compoundGateRepository;
        private readonly IOwnerUnitRepository _ownerUnitRepository;
        private readonly UserIdentity _user;
        private readonly IS3Service _s3Service;
        private readonly IOwnerAssignedUnitRepository _assignedUnitRepository;
        private readonly IHubContext<CounterHub> _hub;

        private readonly IPushNotificationService _pushNotificationService;

        public VisitRequestService(IMapper mapper,
                        IVisitorRequestRepository visitRequestRepository,
                        IUnitOfWork unitOfWork,
                        ICompoundGateRepository compoundGateRepository,
                        UserIdentity user, IS3Service s3Service,
                        IOwnerUnitRepository ownerUnitRepository,
                        IOwnerAssignedUnitRepository assignedUnitRepository, IHubContext<CounterHub> hub,
                        IPushNotificationService pushNotificationService) : base(unitOfWork, mapper)
        {
            _visitRequestRepository = visitRequestRepository;
            _compoundGateRepository = compoundGateRepository;
            _user = user;
            _s3Service = s3Service;
            _ownerUnitRepository = ownerUnitRepository;
            _assignedUnitRepository = assignedUnitRepository;
            _hub = hub;
            _pushNotificationService = pushNotificationService;
        }

        public async Task<VisitRequestOutputViewModel> AddAsync(VisitRequestAddInputViewModel visitRequest)
        {
            ValidateVisit(visitRequest);

            var compoundUnit = await _ownerUnitRepository.Table.Where(x => x.IsActive == true && x.IsDeleted == false &&
                                            x.CompoundUnitId == visitRequest.CompoundUnitId && x.CompoundOwner.OwnerRegistrationId == _user.Id)
                                            .Select(x => new
                                            {
                                                x.CompoundUnitId,
                                                x.CompoundUnit.CompoundGroup.CompoundId,
                                                x.CompoundUnit.CompoundGroup.Compound.CompanyId
                                            }).FirstOrDefaultAsync();
            if (compoundUnit == null)
                compoundUnit = await _assignedUnitRepository.Table.Where(x => x.IsActive == true && x.IsDeleted == false &&
                x.CompoundUnitId == visitRequest.CompoundUnitId && x.OwnerRegistrationId == _user.Id)
                        .Select(x => new
                        {
                            x.CompoundUnitId,
                            x.CompoundUnit.CompoundGroup.Compound.CompoundId,
                            x.CompoundUnit.CompoundGroup.Compound.CompanyId
                        }).FirstOrDefaultAsync();

            if (compoundUnit == null)
                throw new BusinessException("Unit not found");

            // map
            var model = mapper.Map<VisitRequest>(visitRequest);
            model.Type = VisitRequestType.Visit;
            model.CompoundUnitId = compoundUnit.CompoundUnitId;
            model.CompoundId = compoundUnit.CompoundId;
            model.CompanyId = compoundUnit.CompanyId;
            model.IsActive = true;
            model.IsDeleted = false;
            model.CreationDate = DateTime.UtcNow;
            model.OwnerRegistrationId = _user.Id.Value;

            if (model.VisitType != VisitType.Labor && model.VisitType != VisitType.Group)
                model.IsConfirmed = true;

            var codes = await GenerateCodesAsync(model.CompanyId);
            model.Code = codes.Code;
            model.QrCode = codes.QrCode;

            AddVisitAttachments(visitRequest.Files, model);

            _visitRequestRepository.Add(model);
            await unitOfWork.CommitAsync();
            model = await _visitRequestRepository.Table.Include(x => x.CompoundUnit).Include(x => x.OwnerRegistration)
                                            .Where(x => x.VisitRequestId == model.VisitRequestId).FirstOrDefaultAsync();
            return mapper.Map<VisitRequestOutputViewModel>(model);
        }

        public async Task UpdateAsync(VisitRequestUpdateViewModel visitRequest)
        {
            var vr = await _visitRequestRepository.Table.Include(x => x.Attachments)
                                            .Where(x => x.VisitRequestId == visitRequest.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
            if (vr == null)
                throw new BusinessException("Visit not found");
            if (vr.IsConsumed)
                throw new BusinessException("Cannot update used visit");
            if (vr.IsConfirmed == true)
                throw new BusinessException("Cannot update confirmed visit");
            if (vr.IsCanceled)
                throw new BusinessException("Cannot update canceled visit");

            ValidateVisit(visitRequest);

            var compoundUnit = await _ownerUnitRepository.Table.Where(x => x.IsActive == true && x.IsDeleted == false &&
                                            x.CompoundUnitId == visitRequest.CompoundUnitId && x.CompoundOwner.OwnerRegistrationId == _user.Id)
                                            .Select(x => new
                                            {
                                                x.CompoundUnitId,
                                                x.CompoundUnit.CompoundGroup.CompoundId,
                                                x.CompoundUnit.CompoundGroup.Compound.CompanyId
                                            }).FirstOrDefaultAsync();

            if (compoundUnit == null)
                throw new BusinessException("Unit not found");

            foreach (var file in vr.Attachments)
            {
                file.IsDeleted = true;
                file.IsActive = false;
            }
            AddVisitAttachments(visitRequest.Files, vr);
            mapper.Map(visitRequest, vr);
            vr.CompoundUnitId = compoundUnit.CompoundUnitId;
            vr.CompoundId = compoundUnit.CompoundId;
            vr.CompanyId = compoundUnit.CompanyId;

            if (vr.VisitType != VisitType.Labor && vr.VisitType != VisitType.Group)
                vr.IsConfirmed = true;
            else
                vr.IsConfirmed = null;

            await unitOfWork.CommitAsync();
        }

        public async Task<VisitRequestOutputViewModel> AddOwnerVisitAsync(Guid compoundUnitId)
        {
            // TODO: get owner units
            var compoundUnit = await _ownerUnitRepository.Table.Where(x => x.IsActive == true && x.IsDeleted == false &&
                                            x.CompoundUnitId == compoundUnitId && x.CompoundOwner.OwnerRegistrationId == _user.Id)
                                            .Select(x => new
                                            {
                                                x.CompoundUnitId,
                                                x.CompoundUnit.CompoundGroup.CompoundId,
                                                x.CompoundUnit.CompoundGroup.Compound.CompanyId
                                            }).FirstOrDefaultAsync();

            if (compoundUnit == null)
                throw new BusinessException("Unit not found");

            var codes = await GenerateCodesAsync(compoundUnit.CompanyId);

            var visit = new VisitRequest
            {
                DateFrom = DateTime.UtcNow,
                DateTo = DateTime.UtcNow.AddSeconds(30),
                Type = VisitRequestType.Owner,
                IsActive = true,
                IsDeleted = false,
                CreationDate = DateTime.UtcNow,
                CompoundUnitId = compoundUnit.CompoundUnitId,
                CompanyId = compoundUnit.CompanyId,
                CompoundId = compoundUnit.CompoundId,
                OwnerRegistrationId = _user.Id.Value,
                Code = codes.Code,
                QrCode = codes.QrCode,
                IsConfirmed = true
            };
            _visitRequestRepository.Add(visit);
            await unitOfWork.CommitAsync();
            visit = await _visitRequestRepository.Table.Include(x => x.CompoundUnit).Include(x => x.OwnerRegistration)
                                            .Where(x => x.VisitRequestId == visit.VisitRequestId).FirstOrDefaultAsync();
            return mapper.Map<VisitRequestOutputViewModel>(visit);
        }

        public async Task<VisitRequestOutputViewModel> AddCardVisitAsync(Guid compoundUnitId)
        {
            // TODO: get owner units
            var compoundUnit = await _ownerUnitRepository.Table.Where(x => x.IsActive == true && x.IsDeleted == false &&
                                            x.CompoundUnitId == compoundUnitId && x.CompoundOwner.OwnerRegistrationId == _user.Id)
                                            .Select(x => new
                                            {
                                                x.CompoundUnitId,
                                                x.CompoundUnit.CompoundGroup.CompoundId,
                                                x.CompoundUnit.CompoundGroup.Compound.CompanyId
                                            }).FirstOrDefaultAsync();

            if (compoundUnit == null)
                throw new BusinessException("Unit not found");

            var codes = await GenerateCodesAsync(compoundUnit.CompanyId);

            var visit = new VisitRequest
            {
                Type = VisitRequestType.Card,
                IsActive = true,
                IsDeleted = false,
                CreationDate = DateTime.UtcNow,
                CompoundUnitId = compoundUnit.CompoundUnitId,
                CompanyId = compoundUnit.CompanyId,
                CompoundId = compoundUnit.CompoundId,
                OwnerRegistrationId = _user.Id.Value,
                Code = codes.Code,
                QrCode = codes.QrCode,
                IsConfirmed = true
            };
            _visitRequestRepository.Add(visit);
            await unitOfWork.CommitAsync();
            visit = await _visitRequestRepository.Table.Include(x => x.CompoundUnit).Include(x => x.OwnerRegistration)
                                            .Where(x => x.VisitRequestId == visit.VisitRequestId).FirstOrDefaultAsync();
            return mapper.Map<VisitRequestOutputViewModel>(visit);
        }

        public async Task EnableOrCancelCardVisitAsync(Guid visitId, bool isCancel)
        {
            var vr = await _visitRequestRepository.Table.Include(x => x.Attachments)
                                            .Where(x => x.VisitRequestId == visitId && x.IsActive && !x.IsDeleted && x.Type == VisitRequestType.Card).FirstOrDefaultAsync();
            if (vr == null)
                throw new BusinessException("Visit not found");
            vr.IsCanceled = isCancel;
            _visitRequestRepository.Update(vr);
            await unitOfWork.CommitAsync();
        }

        public async Task<PagedOutput<VisitRequestFilterOutputViewModel>> FilterByCompoundAsync(VisitRequestFilterByCompoundInputViewModel model)
        {
            var query = GetVisitQuery();
            if (model.Ids != null && model.Ids.Any())
                query = query.Where(x => model.Ids.Contains(x.CompoundId));
            // var pagedQuery = query.OrderBy(x => x.CreationDate).Skip(model.PageNumber * model.PageSize).Take(model.PageSize);
            // var result = await mapper.ProjectTo<VisitRequestOutputViewModel>(pagedQuery).ToListAsync();
            var result = await query.OrderBy(x => x.CreationDate).Skip(model.PageNumber * model.PageSize).Take(model.PageSize).ToListAsync();
            var output = new PagedOutput<VisitRequestFilterOutputViewModel>
            {
                TotalCount = query.Count(),
                Result = mapper.Map<List<VisitRequestFilterOutputViewModel>>(result)
            };
            return output;
        }

        public async Task ApproveAsync(VisitRequestApproveInputViewModel model)
        {
            var vr = await _visitRequestRepository.GetByIdAsync(model.Id);
            if (vr == null)
                throw new BusinessException("Not found");
            vr.IsConfirmed = model.IsApproved;
            if (vr.VisitType == VisitType.Labor && model.Attachments != null)
            {
                AddVisitAttachments(model.Attachments, vr);
            }
            _visitRequestRepository.Update(vr);
            await unitOfWork.CommitAsync();
            await _hub.Clients.All.SendAsync("UpdatePendingListCount", true);

            await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
            {
                NotificationType = PushNotificationType.VisitApprove,
                RecordId = model.Id
            });
        }

        public async Task<VisitRequestOutputViewModel> GetByIdAsync(Guid id)
        {
            var vr = await _visitRequestRepository.Table
                    .Include(x => x.CompoundUnit)
                            .ThenInclude(x => x.OwnerUnits)
                                    .ThenInclude(x => x.CompoundOwner)
                    .Include(x => x.OwnerRegistration)
                    .Include(x => x.Attachments)
                    .Where(x => x.VisitRequestId == id && x.IsActive && !x.IsDeleted)
                    .FirstOrDefaultAsync();
            if (vr == null)
                throw new BusinessException("Not found");
            return mapper.Map<VisitRequestOutputViewModel>(vr);
        }

        public async Task CancelAsync(Guid id)
        {
            var vr = await _visitRequestRepository.Table.Where(x => x.VisitRequestId == id).FirstOrDefaultAsync();
            if (vr == null)
                throw new BusinessException("Not found");
            if (vr.IsConsumed)
                throw new BusinessException("This visit used before");
            vr.IsCanceled = true;
            _visitRequestRepository.Update(vr);
            await unitOfWork.CommitAsync();

            await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
            {
                NotificationType = PushNotificationType.VisitCanceled,
                RecordId = id
            });
        }

        public async Task<PagedOutput<VisitRequestFilterOutputViewModel>> FilterByCompanyAsync(VisitRequestFilterByCompanyInputViewModel model)
        {
            var query = GetVisitQuery().Where(x => x.CompanyId == model.Id);
            // var pagedQuery = query.OrderBy(x => x.CreationDate).Skip(model.PageNumber * model.PageSize).Take(model.PageSize);
            // var result = await mapper.ProjectTo<VisitRequestOutputViewModel>(pagedQuery).ToListAsync();
            var result = await query.OrderBy(x => x.CreationDate).Skip(model.PageNumber * model.PageSize).Take(model.PageSize).ToListAsync();
            var output = new PagedOutput<VisitRequestFilterOutputViewModel>
            {
                TotalCount = query.Count(),
                Result = mapper.Map<List<VisitRequestFilterOutputViewModel>>(result)
            };
            return output;
        }

        public async Task<PagedOutput<VisitRequestFilterOutputViewModel>> FilterByUnitAsync(VisitRequestFilterByUnitInputViewModel model)
        {
            var query = GetVisitQuery().Where(x => x.CompoundUnitId == model.Id);
            // var pagedQuery = query.OrderBy(x => x.CreationDate).Skip(model.PageNumber * model.PageSize).Take(model.PageSize);
            // var result = await mapper.ProjectTo<VisitRequestOutputViewModel>(pagedQuery).ToListAsync();
            var result = await query.OrderBy(x => x.CreationDate).Skip(model.PageNumber * model.PageSize).Take(model.PageSize).ToListAsync();
            var output = new PagedOutput<VisitRequestFilterOutputViewModel>
            {
                TotalCount = query.Count(),
                Result = mapper.Map<List<VisitRequestFilterOutputViewModel>>(result)
            };
            return output;
        }

        public async Task<PagedOutput<VisitRequestFilterOutputViewModel>> FilterByUserAsync(VisitRequestFilterByUserInputViewModel model, Guid compoundId)
        {
            var query = GetVisitQuery().Where(x => x.OwnerRegistrationId == _user.Id);
            if (model.DateFrom.HasValue && model.DateTo.HasValue)
            {
                model.DateFrom = model.DateFrom.Value;
                model.DateTo = model.DateTo.Value;
                query = query.Where(x => x.CreationDate.Date >= model.DateFrom.Value.Date && x.CreationDate.Date <= model.DateTo.Value.Date);
            }

            if (model.Types != null && model.Types.Any())
                query = query.Where(x => model.Types.Contains(x.VisitType));
            if (compoundId == Guid.Empty)
                query = query.Where(z => z.CompoundId == compoundId);
            if (model.IsUpcoming)
                query = query.Where(x => !x.IsConsumed && x.DateTo.Value.Date >= DateTime.Now.Date);
            else
                query = query.Where(x => x.IsConsumed || x.DateTo.Value.Date < DateTime.Now.Date);

            // var pagedQuery = query.OrderBy(x => x.CreationDate).Skip(model.PageNumber * model.PageSize).Take(model.PageSize);
            // var result = await mapper.ProjectTo<VisitRequestOutputViewModel>(pagedQuery).ToListAsync();
            var result = await query.OrderByDescending(x => x.CreationDate).Skip(model.PageNumber * model.PageSize).Take(model.PageSize).ToListAsync();
            var output = new PagedOutput<VisitRequestFilterOutputViewModel>
            {
                TotalCount = query.Count(),
                Result = mapper.Map<List<VisitRequestFilterOutputViewModel>>(result)
            };
            return output;
        }

        public async Task<PagedOutput<VisitRequestFilterOutputViewModel>> FilterByGateAsync(VisitRequestFilterByUserInputViewModel model, Guid gateId)
        {
            var query = _visitRequestRepository.Table.Include(x => x.CompoundUnit).Include(x => x.OwnerRegistration)
                .Include(x => x.VisitTransactionHistories).ThenInclude(x => x.Gate)
                .Where(x => x.IsActive && !x.IsDeleted && x.Type == VisitRequestType.Visit)
                .Where(x => x.VisitTransactionHistories.Any(z => z.GateId == gateId));
            if (model.DateFrom.HasValue)
                query = query.Where(x => x.CreationDate.Date >= model.DateFrom.Value.Date);

            if (model.DateTo.HasValue)
                query = query.Where(x => x.CreationDate.Date <= model.DateTo.Value.Date);
            if (model.Types != null && model.Types.Any())
                query = query.Where(x => model.Types.Contains(x.VisitType));

            if (model.IsUpcoming)
                query = query.Where(x => x.VisitTransactionHistories.All(z => z.Gate.EntryType == GateEntryType.Entrance));
            else
                query = query.Where(x => x.VisitTransactionHistories.Any(z => z.Gate.EntryType == GateEntryType.Entrance) &&
                x.VisitTransactionHistories.Any(z => z.Gate.EntryType == GateEntryType.Exit));

            var result = await query.OrderByDescending(x => x.CreationDate).Skip(model.PageNumber * model.PageSize).Take(model.PageSize).ToListAsync();
            var output = new PagedOutput<VisitRequestFilterOutputViewModel>
            {
                TotalCount = query.Count(),
                Result = mapper.Map<List<VisitRequestFilterOutputViewModel>>(result)
            };
            return output;
        }

        public Task RequestByQrCodeAsync(RequestByQrCodeViewModel model, int timezone)
        {
            return RequestByQrCodeOrCodeAsync(model, true, timezone);
        }

        public Task RequestByCodeAsync(RequestByQrCodeViewModel model, int timezone)
        {
            return RequestByQrCodeOrCodeAsync(model, false, timezone);
        }

        public Task RequestGateByQrCodeAsync(RequestVisitGateByCodeViewModel model, int timezone)
        {
            if (string.IsNullOrWhiteSpace(model.SecurityKey))
                throw new BusinessException("Security key required");
            return RequestByQrCodeOrCodeAsync(new RequestByQrCodeViewModel { GateId = model.GateId, Code = model.Code }, true, timezone);
        }

        public Task RequestGateByCodeAsync(RequestVisitGateByCodeViewModel model, int timezone)
        {
            if (string.IsNullOrWhiteSpace(model.SecurityKey))
                throw new BusinessException("Security key required");
            return RequestByQrCodeOrCodeAsync(new RequestByQrCodeViewModel { GateId = model.GateId, Code = model.Code }, false, timezone);
        }

        public async Task<ConfirmVisitWithAttachResponse> ConfirmWithAttachments(Guid visitRequestId, List<VisitAttachmentViewModel> attachmemts)
        {
            var visit = await _visitRequestRepository.Table.Include(x => x.Attachments)
                .FirstOrDefaultAsync(x => x.VisitRequestId == visitRequestId);
            if (visit == null)
                throw new BusinessException("Visit not found");
            if (attachmemts.Count > 0)
                AddVisitAttachments(attachmemts, visit);
            visit.IsConfirmed = true;
            _visitRequestRepository.Update(visit);
            await unitOfWork.CommitAsync();
            return new ConfirmVisitWithAttachResponse
            {
                VisitRequestId = visit.VisitRequestId,
                Attachments = visit.Attachments.Select(x => x.Path).ToArray()
            };
        }

        public async Task<ValidateVisitResponse> ValidateVisitByCode(Guid gateId, string code)
        {
            var vr = await ValidateVisitCode(gateId, code, false);
            return vr;
        }

        public async Task<ValidateVisitResponse> ValidateVisitByQrCode(Guid gateId, string code)
        {
            var vr = await ValidateVisitCode(gateId, code, true);
            return vr;
        }

        private async Task RequestByQrCodeOrCodeAsync(RequestByQrCodeViewModel model, bool isQrCode,
                        int timezone)
        {
            var gateCompounds = await _compoundGateRepository.Table.Where(x => x.GateId == model.GateId && x.IsActive.Value && !x.IsDeleted.Value)
                                            .Select(x => new { x.Gate.EntryType, x.CompoundId, x.Compound.CompanyId }).ToListAsync();

            if (!gateCompounds.Any())
                throw new BusinessException("Gate not found");

            var gateCompound = gateCompounds.FirstOrDefault();
            var query = _visitRequestRepository.Table.Include(x => x.OwnerRegistration).ThenInclude(x => x.OwnerAssignedUnits)
                    .Include(x => x.VisitTransactionHistories).ThenInclude(x => x.Gate)
                                            .Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == gateCompound.CompanyId);
            query = isQrCode ? query.Where(x => x.QrCode == model.Code) : query.Where(x => x.Code == model.Code);
            var vr = await query.FirstOrDefaultAsync();
            if (vr == null)
                throw new BusinessException("Visit Not found");

            if (!gateCompounds.Any(x => x.CompoundId == vr.CompoundId))
                throw new BusinessException("Invalid compound");
            if (vr.IsCanceled)
                throw new BusinessException("Visit is canceled");
            if (vr.OwnerRegistration.UserType == OwnerRegistrationType.Tenant &&
                    !vr.OwnerRegistration.OwnerAssignedUnits.Any(x => x.StartFrom.Value.Date <= DateTime.UtcNow.Date && DateTime.UtcNow.Date <= x.EndTo.Value.Date))
                throw new BusinessException("Tenant finished his period");
            if (vr.Type == VisitRequestType.Visit || vr.Type == VisitRequestType.Owner)
            {
                if (vr.DateTo.Value < DateTime.UtcNow)
                    throw new BusinessException("Visit expired");
            }
            if (vr.Type == VisitRequestType.Visit)
            {
                if (vr.VisitType == VisitType.Group && gateCompound.EntryType == GateEntryType.Entrance
                        && vr.VisitTransactionHistories.Where(x => x.Gate.EntryType == GateEntryType.Entrance).Count() == vr.GroupNo)
                    throw new BusinessException("Group Count is exceeded");
                if ((vr.VisitType == VisitType.Labor || vr.VisitType == VisitType.Group) && vr.IsConfirmed != true)
                    throw new BusinessException("Visit not confirmed");
                if (vr.VisitType == VisitType.Periodic || vr.VisitType == VisitType.Labor)
                {
                    var dayOfWeek = DateTime.UtcNow.DayOfWeek.ToString();
                    if (!vr.Days.Contains(dayOfWeek))
                        throw new BusinessException("Invalid day");
                }
                else if (vr.VisitType != VisitType.Group && gateCompound.EntryType == GateEntryType.Entrance &&
                      vr.VisitTransactionHistories.Any()) //x => x.Gate.EntryType == GateEntryType.Exit
                {
                    throw new BusinessException("Used before");
                }
            }
            vr.IsConsumed = true;
            vr.VisitTransactionHistories.Add(new VisitTransactionHistory
            {
                GateId = model.GateId,
                CompanyUserId = model.UserId,
                Date = DateTime.UtcNow
            });
            _visitRequestRepository.Update(vr);
            await unitOfWork.CommitAsync();

            await _pushNotificationService.CreatePushNotification(new PushNotificationAddViewModel
            {
                NotificationType = PushNotificationType.VisitRequestedOnGate,
                RecordId = vr.VisitRequestId
            });
        }

        private void AddVisitAttachments(List<VisitAttachmentViewModel> visitRequestFiles, VisitRequest vr)
        {
            if (visitRequestFiles != null && visitRequestFiles.Count > 0)
            {
                foreach (var file in visitRequestFiles)
                {
                    if (!string.IsNullOrEmpty(file.FileBase64))
                        file.FileBytes = Convert.FromBase64String(file.FileBase64);
                    var fileUrl = _s3Service.UploadFile("Visits", file.FileName, file.FileBytes);
                    vr.Attachments.Add(new VisitRequestAttachment
                    {
                        IsActive = true,
                        IsDeleted = false,
                        Path = fileUrl
                    });
                }
            }
        }

        private IQueryable<VisitRequest> GetVisitQuery()
        {
            var query = _visitRequestRepository.Table.Include(x => x.CompoundUnit).Include(x => x.OwnerRegistration)
                                            .Where(x => x.IsActive && !x.IsDeleted && x.Type == VisitRequestType.Visit);
            return query;
        }

        private void ValidateVisit(VisitRequestAddInputViewModel visitRequest)
        {
            if (visitRequest.Type == VisitType.None)
                throw new BusinessException("Invalid Type");
            if (visitRequest.Type != VisitType.Taxi && visitRequest.Type != VisitType.Group)
            {
                if (string.IsNullOrEmpty(visitRequest.VisitorName))
                    throw new BusinessException("Visitor name is required");
            }
            if (visitRequest.Type == VisitType.Taxi || visitRequest.Type == VisitType.Delivery)
            {
                if (visitRequest.Type == VisitType.Taxi && string.IsNullOrEmpty(visitRequest.CarNo))
                    throw new BusinessException("Car number is required");
                visitRequest.DateFrom = DateTime.UtcNow;
                visitRequest.DateTo = DateTime.UtcNow.AddHours(1);
            }
            else if (visitRequest.Type == VisitType.Once)
            {
                if (visitRequest.DateFrom == null || visitRequest.DateTo == null)
                {
                    visitRequest.DateFrom = DateTime.UtcNow.Date;
                    visitRequest.DateTo = DateTime.UtcNow.AddMonths(1).Date;
                }
            }
            else
            {
                if (visitRequest.DateFrom == null || visitRequest.DateTo == null || visitRequest.DateFrom >= visitRequest.DateTo)
                    throw new BusinessException("Date required");
            }

            // visitRequest.DateFrom = visitRequest.DateFrom.Value.ToUniversalTime();
            // visitRequest.DateTo = visitRequest.DateTo.Value.ToUniversalTime();

            if (visitRequest.Type == VisitType.Periodic || visitRequest.Type == VisitType.Labor)
            {
                if (visitRequest.Days == null || visitRequest.Days.Count == 0)
                {
                    visitRequest.Days = new List<Day> { Day.Saturday, Day.Sunday, Day.Monday, Day.Thursday, Day.Wednesday, Day.Thursday, Day.Friday };
                }
            }

            if (visitRequest.Type == VisitType.Group && visitRequest.GroupNo == 0)
                throw new BusinessException("Group number is required");
        }

        private async Task<VisitCodeViewModel> GenerateCodesAsync(Guid companyId)
        {
            var currentCodes = await _visitRequestRepository.Table
                                            .Where(x => x.CompanyId == companyId && !x.IsCanceled && (x.DateTo == null || x.DateTo.Value.Date >= DateTime.Now.Date))
                                            .Select(x => new { x.Code, x.QrCode }).ToListAsync();

            var code = NumberHelpers.GenerateUniqueRandomNumber(currentCodes.Select(x => x.Code).ToList());
            var qrCode = GenerateQrCode(currentCodes.Select(x => x.QrCode).ToList());
            return new VisitCodeViewModel { Code = code, QrCode = qrCode };
        }

        private string GenerateQrCode(List<string> currentCodes)
        {
            var result = Guid.NewGuid().ToString();
            while (currentCodes.Contains(result))
                result = Guid.NewGuid().ToString();
            return result;
        }

        private async Task<ValidateVisitResponse> ValidateVisitCode(Guid gateId, string code, bool isQrCode)
        {
            var gateCompounds = await _compoundGateRepository.Table.Where(x => x.GateId == gateId && x.IsActive.Value && !x.IsDeleted.Value)
                                            .Select(x => new { x.Gate.EntryType, x.CompoundId, x.Compound.CompanyId }).ToListAsync();

            if (!gateCompounds.Any())
                throw new BusinessException("Gate not found");

            var gateCompound = gateCompounds.FirstOrDefault();
            var query = _visitRequestRepository.Table.Include(x => x.OwnerRegistration).ThenInclude(x => x.OwnerAssignedUnits)
                    .Include(x => x.VisitTransactionHistories).ThenInclude(x => x.Gate)
                                            .Where(x => x.IsActive && !x.IsDeleted && x.CompanyId == gateCompound.CompanyId);
            query = isQrCode ? query.Where(x => x.QrCode == code) : query.Where(x => x.Code == code);
            var vr = await query.FirstOrDefaultAsync();
            if (vr == null)
                throw new BusinessException("Visit Not found");

            if (!gateCompounds.Any(x => x.CompoundId == vr.CompoundId))
                throw new BusinessException("Invalid compound");
            if (vr.IsCanceled)
                return new ValidateVisitResponse { Status = false, VisitRequestId = vr.VisitRequestId, Message = "Visit is cancelled" };
            if (vr.OwnerRegistration.UserType == OwnerRegistrationType.Tenant &&
                    !vr.OwnerRegistration.OwnerAssignedUnits.Any(x => x.StartFrom.Value.Date <= DateTime.UtcNow.Date && DateTime.UtcNow.Date <= x.EndTo.Value.Date))
                return new ValidateVisitResponse { Status = false, VisitRequestId = vr.VisitRequestId, Message = "Tenant finished his period" };
            if (vr.Type == VisitRequestType.Visit || vr.Type == VisitRequestType.Owner)
            {
                if (vr.DateTo.Value < DateTime.UtcNow)
                    return new ValidateVisitResponse { Status = false, VisitRequestId = vr.VisitRequestId, Message = "Visit is expired" };
            }
            if (vr.Type == VisitRequestType.Visit)
            {
                if (vr.VisitType == VisitType.Group && gateCompound.EntryType == GateEntryType.Entrance
                        && vr.VisitTransactionHistories.Where(x => x.Gate.EntryType == GateEntryType.Entrance).Count() == vr.GroupNo)
                    return new ValidateVisitResponse { Status = false, VisitRequestId = vr.VisitRequestId, Message = "Group count exceed" };
                if ((vr.VisitType == VisitType.Labor || vr.VisitType == VisitType.Group) && vr.IsConfirmed != true)
                    return new ValidateVisitResponse { Status = false, VisitRequestId = vr.VisitRequestId, Message = "Visit not confirmed" };
                if (vr.VisitType == VisitType.Periodic || vr.VisitType == VisitType.Labor)
                {
                    var dayOfWeek = DateTime.UtcNow.DayOfWeek.ToString();
                    if (!vr.Days.Contains(dayOfWeek))
                        return new ValidateVisitResponse { Status = false, VisitRequestId = vr.VisitRequestId, Message = "Invalid day" };
                }
                else if (vr.VisitType != VisitType.Group && gateCompound.EntryType == GateEntryType.Entrance &&
                      vr.VisitTransactionHistories.Any())
                {
                    return new ValidateVisitResponse { Status = false, VisitRequestId = vr.VisitRequestId, Message = "Used before" };
                }
            }
            return new ValidateVisitResponse { Status = true, VisitRequestId = vr.VisitRequestId }; ;
        }

    }
}
