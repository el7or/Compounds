using AutoMapper;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models;
using Puzzle.Compound.Models.VisitRequest;
using Puzzle.Compound.Models.VisitTransactionHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puzzle.Compound.Mapper.Profiles
{
    public class VisitRequestProfile : Profile
    {
        public VisitRequestProfile()
        {
            CreateMap<VisitRequestAddInputViewModel, VisitRequest>()
                .ForMember(x => x.VisitType, opt => opt.MapFrom(x => x.Type))
                .ForMember(x => x.Days, opt => opt.MapFrom(x => x.Days == null ? null : string.Join(",", x.Days)));

            CreateMap<VisitRequestUpdateViewModel, VisitRequest>()
                .ForMember(x => x.VisitRequestId, opt => opt.MapFrom(x => x.Id))
                .IncludeBase<VisitRequestAddInputViewModel, VisitRequest>();

            CreateMap<VisitRequest, VisitRequestOutputViewModel>()
                .ForMember(x => x.Type, opt => opt.MapFrom(x => x.VisitType))
                .ForMember(x => x.Files, opt => opt.MapFrom(x => x.Attachments.Select(x => x.Path)))
                .ForMember(x => x.Days, opt => opt.MapFrom(x => x.Days.Split(",", StringSplitOptions.None).Select(x => Enum.Parse<Day>(x))))
                .ForMember(x => x.UnitName, opt => opt.MapFrom(x => x.CompoundUnit.Name))
                .ForMember(x => x.OwnerName, opt => opt.MapFrom(x => x.OwnerRegistration.Name))
                .ForMember(x => x.UserType, opt => opt.MapFrom(x => x.OwnerRegistration.UserType))
                .ForMember(x => x.Owner, opt => opt.MapFrom(x => x.CompoundUnit.OwnerUnits.FirstOrDefault().CompoundOwner))
                .ForMember(x => x.CanUpload, opt => opt.MapFrom(x => x.VisitType == VisitType.Delivery || x.VisitType == VisitType.Taxi || x.VisitType == VisitType.Labor))
                .ForMember(x => x.VisitorName, opt => opt.MapFrom(v =>
                v.VisitType == VisitType.Taxi ? v.CarNo
                : v.VisitType == VisitType.Group ? v.Details
                : v.VisitorName));

            CreateMap<CompoundOwner, VisitRequestOwnerViewModel>()
                .ForMember(x => x.Units, opt => opt.MapFrom(x => x.OwnerUnits.Select(u => u.CompoundUnit.Name).ToList()));

            CreateMap<VisitRequest, VisitRequestFilterOutputViewModel>()
                .ForMember(x => x.Type, opt => opt.MapFrom(x => x.VisitType))
                .ForMember(x => x.UnitName, opt => opt.MapFrom(x => x.CompoundUnit.Name))
                .ForMember(x => x.OwnerName, opt => opt.MapFrom(x => x.OwnerRegistration.Name))
                .ForMember(x => x.OwnerPhone, opt => opt.MapFrom(x => x.OwnerRegistration.Phone))
                .ForMember(x => x.Days, opt => opt.MapFrom(x => x.Days.Split(",", StringSplitOptions.None).Select(x => Enum.Parse<Day>(x))));

            CreateMap<VisitRequestAttachment, PuzzleFileInfo>()
            .ForMember(i => i.SizeInBytes, opt => opt.Ignore());

            CreateMap<VisitTransactionHistory, VisitTransactionHistoryFilterOutputViewModel>()
                .ForMember(x => x.OwnerRegistrationId, opt => opt.MapFrom(x => x.VisitRequest.OwnerRegistration.OwnerRegistrationId))
                .ForMember(x => x.UnitName, opt => opt.MapFrom(x => x.VisitRequest.CompoundUnit.Name))
                .ForMember(x => x.GateName, opt => opt.MapFrom(x => x.Gate.GateName))
                .ForMember(x => x.VisitorName, opt => opt.MapFrom(x => x.VisitRequest.VisitorName))
                .ForMember(x => x.OwnerName, opt => opt.MapFrom(x => x.VisitRequest.OwnerRegistration.Name))
                .ForMember(x => x.Type, opt => opt.MapFrom(x => x.VisitRequest.VisitType))
                .ForMember(x => x.Status, opt => opt.MapFrom(x =>
                    x.VisitRequest.IsCanceled ? VisitStatus.Canceled :
                    x.VisitRequest.DateTo.HasValue && x.VisitRequest.DateTo.Value.Date < DateTime.Now.Date ? VisitStatus.Expired :
                    x.VisitRequest.IsConsumed ? VisitStatus.Consumed :
                    x.VisitRequest.IsConfirmed == true ? VisitStatus.Confirmed :
                   x.VisitRequest.IsConfirmed == false ? VisitStatus.NotConfirmed :
                    VisitStatus.Pending
                ));

            CreateMap<VisitRequest, VisitTransactionHistoryFilterOutputViewModel>()
                .ForMember(x => x.OwnerRegistrationId, opt => opt.MapFrom(x => x.OwnerRegistration.OwnerRegistrationId))
                .ForMember(x => x.UnitName, opt => opt.MapFrom(x => x.CompoundUnit.Name))
                .ForMember(x => x.OwnerName, opt => opt.MapFrom(x => x.OwnerRegistration.Name))
                .ForMember(x => x.Type, opt => opt.MapFrom(x => x.VisitType))
                .ForMember(x => x.Status, opt => opt.MapFrom(x => VisitStatus.Pending));

            CreateMap<OwnerRegistration, VisitsCompoundOwnersViewModel>();
            CreateMap<CompoundUnit, VisitsCompoundUnitsViewModel>();
            CreateMap<Gate, VisitsCompoundGatesViewModel>();
        }
    }
}
