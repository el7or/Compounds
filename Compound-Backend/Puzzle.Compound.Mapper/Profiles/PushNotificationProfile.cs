using AutoMapper;
using Puzzle.Compound.Core.Models;
using Puzzle.Compound.Models.PushNotifications;

namespace Puzzle.Compound.Mapper.Profiles
{
    public class PushNotificationProfile : Profile
    {
        public PushNotificationProfile()
        {
            CreateMap<NotificationSchedule, NotificationScheduleViewModel>();
            CreateMap<NotificationUser, NotificationUserViewModel>();
            CreateMap<RegistrationForUser, RegistrationForUsersOutputViewModel>();
        }
    }
}
