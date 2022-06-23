using Puzzle.Compound.Common.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle.Compound.Models.PushNotifications
{
    public class RouteAndroid
    {
        public string BaseUrl { get; set; }
        public List<RouteAndroidActivity> Activities { get; set; }

        public string GetActivity(int notificationType)
        {
            return BaseUrl + Activities.FirstOrDefault(q => q.Id == notificationType).Activity;
        }
    }
}
