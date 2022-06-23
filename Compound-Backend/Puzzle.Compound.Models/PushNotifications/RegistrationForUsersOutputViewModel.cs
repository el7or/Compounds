using System;

namespace Puzzle.Compound.Models.PushNotifications
{
    public class RegistrationForUsersOutputViewModel
    {
        public Guid Id { get; set; }
        public string RegisterId { get; set; }
        public Guid UserId { get; set; }
        public string RegisterType { get; set; }
    }
}