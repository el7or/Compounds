using AutoMapper;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models;
using Puzzle.Compound.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puzzle.Compound.Mapper.Profiles
{
   public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<CompoundNotification, NotificationOutputViewModel>()
                .ForMember(x => x.IsOwnerRead, cfg => cfg.MapFrom(n => n.OwnerNotifications.Count > 0 ? true : false))
                .ForMember(x => x.OwnerReadOn, cfg => cfg.MapFrom(n => n.OwnerNotifications.FirstOrDefault().CreationDate))
                .ForMember(x => x.ToGroupsIds, cfg => cfg.MapFrom(n => n.NotificationUnits.Select(u => u.CompoundUnit.CompoundGroupId).Distinct().ToList()))
                .ForMember(x => x.ToUnitsIds, cfg => cfg.MapFrom(n => n.NotificationUnits.Select(u => u.CompoundUnitId).ToList()));

            CreateMap<NotificationInputViewModel, CompoundNotification>()
                .ForMember(i => i.OwnerNotifications, opt => opt.Ignore())
                .ForMember(i => i.NotificationUnits, opt => opt.Ignore())
                .ForMember(i => i.Images, opt => opt.Ignore());

            CreateMap<CompoundNotificationImage, PuzzleFileInfo>()
            .ForMember(i => i.SizeInBytes, opt => opt.Ignore());
        }
    }
}
