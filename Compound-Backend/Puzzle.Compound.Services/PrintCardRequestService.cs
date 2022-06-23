using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Amazon;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Data.Infrastructure;
using Puzzle.Compound.Data.Repositories;
using Puzzle.Compound.Models.PrintCardRequest;
using Puzzle.Compound.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Compound.Services
{
    public interface IPrintCardRequestService
    {
        Task<PrintCardOutputViewModel> AddAsync(PrintCardAddViewModel model);
        Task<PrintCardOutputViewModel> GetByIdAsync(Guid id);
        Task ApproveOrRejectAsync(PrintCardApproveViewModel model);
        Task PrintCancelAsync(Guid id);
        Task CardEnableOrCancelAsync(CardEnableOrCancelViewModel model);
        Task<PagedOutput<PrintCardOutputViewModel>> FilterByUserAsync(PagedInput model);
        Task<PagedOutput<PrintCardOutputViewModel>> FilterByUnitAsync(PrintCardFilterByUnitInputViewModel model);
        Task<PagedOutput<PrintCardOutputViewModel>> FilterByCompoundAsync(PrintCardFilterByCompoundInputViewModel model);
        Task<PagedOutput<PrintCardOutputViewModel>> FilterByCompanyAsync(PrintCardFilterByCompanyInputViewModel model);
    }

    public class PrintCardRequestService : BaseService, IPrintCardRequestService
    {
        private readonly IPrintCardRequestRepository _printCardRequestRepository;
        private readonly IVisitRequestService _visitRequestService;
        private readonly UserIdentity _user;
        private readonly IS3Service _s3Service;
        public PrintCardRequestService(IUnitOfWork unitOfWork, IMapper mapper, IPrintCardRequestRepository printCardRequestRepository, UserIdentity user, IS3Service s3Service, IVisitRequestService visitRequestService) : base(unitOfWork, mapper)
        {
            _printCardRequestRepository = printCardRequestRepository;
            _user = user;
            _s3Service = s3Service;
            _visitRequestService = visitRequestService;
        }

        public async Task<PrintCardOutputViewModel> AddAsync(PrintCardAddViewModel model)
        {
            if (model.Picture == null || model.Picture.Length == 0)
                throw new BusinessException("Picture is required");
            var card = mapper.Map<PrintCardRequest>(model);
            card.Status = PrintCardStatus.Request;
            card.Picture = _s3Service.UploadFile("Cards", model.PictureName, model.Picture);
            card.IsActive = true;
            card.CreationDate = DateTime.UtcNow;
            card.OwnerRegisterationId = _user.Id.Value;
            _printCardRequestRepository.Add(card);
            await unitOfWork.CommitAsync();
            card = await _printCardRequestRepository.Table.Include(x => x.OwnerRegistration).Include(x => x.CompoundUnit)
                .Where(x => x.Id == card.Id).FirstOrDefaultAsync();
            return mapper.Map<PrintCardOutputViewModel>(card);
        }

        public async Task<PrintCardOutputViewModel> GetByIdAsync(Guid id)
        {
            var card = await _printCardRequestRepository.Table.Include(x => x.OwnerRegistration).Include(x => x.VisitRequest).Where(x => x.Id == id && x.IsActive).FirstOrDefaultAsync();
            return mapper.Map<PrintCardOutputViewModel>(card);
        }

        public async Task ApproveOrRejectAsync(PrintCardApproveViewModel model)
        {
            var card = await _printCardRequestRepository.Table.Include(x => x.OwnerRegistration).Where(x => x.Id == model.Id && x.IsActive).FirstOrDefaultAsync();
            if (card == null)
                throw new BusinessException("Card request not found");
            card.Status = model.IsApproved ? PrintCardStatus.BeingPrinted : PrintCardStatus.Rejected;
            if(card.Status == PrintCardStatus.BeingPrinted)
            {
                if (!card.VisitRequestId.HasValue)
                {
                    var visit = await _visitRequestService.AddCardVisitAsync(card.CompoundUnitId);
                    card.VisitRequestId = visit.VisitRequestId;
                }
            }
            _printCardRequestRepository.Update(card);
            await unitOfWork.CommitAsync();
        }

        public async Task PrintCancelAsync(Guid id)
        {
            var card = await _printCardRequestRepository.Table.Include(x => x.OwnerRegistration).Where(x => x.Id == id && x.IsActive).FirstOrDefaultAsync();
            if (card == null)
                throw new BusinessException("Card request not found");
            if (card.Status == PrintCardStatus.Printed)
                throw new BusinessException("Can't cancel after print");
            card.Status = PrintCardStatus.PrintCanceled;
            _printCardRequestRepository.Update(card);
            await unitOfWork.CommitAsync();
        }

        public async Task CardEnableOrCancelAsync(CardEnableOrCancelViewModel model)
        {
            var card = await _printCardRequestRepository.Table.Include(x => x.OwnerRegistration).Where(x => x.Id == model.Id && x.IsActive).FirstOrDefaultAsync();
            if (card == null)
                throw new BusinessException("Card request not found");
            card.Status = model.IsCancel ? PrintCardStatus.CardCanceled : PrintCardStatus.Printed;
            _printCardRequestRepository.Update(card);
            await unitOfWork.CommitAsync();
            await _visitRequestService.EnableOrCancelCardVisitAsync(card.VisitRequestId.Value, model.IsCancel);
        }

        private async Task UpdateStatusAsync(PrintCardUpdateStatusInputViewModel model)
        {
            var card = await _printCardRequestRepository.Table.Include(x => x.OwnerRegistration).Where(x => x.Id == model.Id && x.IsActive).FirstOrDefaultAsync();
            if (card == null)
                throw new BusinessException("Card request not found");
            if (model.Status == PrintCardStatus.CardCanceled && card.Status != PrintCardStatus.Printed)
                throw new BusinessException("Card not printed to be cancelled");
            if (model.Status == PrintCardStatus.PrintCanceled && card.Status == PrintCardStatus.Printed)
                throw new BusinessException("Can't cancel card after print");
            card.Status = model.Status;
            _printCardRequestRepository.Update(card);
            await unitOfWork.CommitAsync();
        }

        public async Task<PagedOutput<PrintCardOutputViewModel>> FilterByUserAsync(PagedInput model)
        {
            var query = _printCardRequestRepository.Table.Include(x => x.OwnerRegistration).Include(x => x.CompoundUnit).Where(x => x.Id == _user.Id && x.IsActive);
            var cards = await query.OrderByDescending(x => x.CreationDate).Skip(model.PageNumber * model.PageSize).Take(model.PageSize).ToListAsync();
            var result = new PagedOutput<PrintCardOutputViewModel>
            {
                TotalCount = await query.CountAsync(),
                Result = mapper.Map<List<PrintCardOutputViewModel>>(cards)
            };
            return result;
        }

        public async Task<PagedOutput<PrintCardOutputViewModel>> FilterByUnitAsync(PrintCardFilterByUnitInputViewModel model)
        {
            var query = _printCardRequestRepository.Table.Include(x => x.OwnerRegistration).Include(x => x.CompoundUnit).Where(x => x.IsActive);
            if (model.Ids != null && model.Ids.Any())
                query = query.Where(x => model.Ids.Contains(x.CompoundUnitId));
            var cards = await query.OrderByDescending(x => x.CreationDate).Skip(model.PageNumber * model.PageSize).Take(model.PageSize).ToListAsync();
            var result = new PagedOutput<PrintCardOutputViewModel>
            {
                TotalCount = await query.CountAsync(),
                Result = mapper.Map<List<PrintCardOutputViewModel>>(cards)
            };
            return result;
        }

        public async Task<PagedOutput<PrintCardOutputViewModel>> FilterByCompoundAsync(PrintCardFilterByCompoundInputViewModel model)
        {
            var query = _printCardRequestRepository.Table.Include(x => x.OwnerRegistration).Include(x => x.CompoundUnit).Where(x => x.IsActive);
            if (model.Ids != null && model.Ids.Any())
                query = query.Where(x => model.Ids.Contains(x.CompoundUnit.CompoundGroup.CompoundId));
            var cards = await query.OrderByDescending(x => x.CreationDate).Skip(model.PageNumber * model.PageSize).Take(model.PageSize).ToListAsync();
            var result = new PagedOutput<PrintCardOutputViewModel>
            {
                TotalCount = await query.CountAsync(),
                Result = mapper.Map<List<PrintCardOutputViewModel>>(cards)
            };
            return result;
        }

        public async Task<PagedOutput<PrintCardOutputViewModel>> FilterByCompanyAsync(PrintCardFilterByCompanyInputViewModel model)
        {
            var query = _printCardRequestRepository.Table.Include(x => x.OwnerRegistration).Include(x => x.CompoundUnit).Where(x => x.IsActive);
            if (model.Ids != null && model.Ids.Any())
                query = query.Where(x => model.Ids.Contains(x.CompoundUnit.CompoundGroup.Compound.CompoundId));
            var cards = await query.OrderByDescending(x => x.CreationDate).Skip(model.PageNumber * model.PageSize).Take(model.PageSize).ToListAsync();
            var result = new PagedOutput<PrintCardOutputViewModel>
            {
                TotalCount = await query.CountAsync(),
                Result = mapper.Map<List<PrintCardOutputViewModel>>(cards)
            };
            return result;
        }
    }
}
