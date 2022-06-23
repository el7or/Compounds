using System;

namespace Puzzle.Compound.Models.PushNotifications
{
    public class RegistrationForUsersAddViewModel
    {
        public string RegisterId { get; set; }
        public Guid UserId { get; set; }
        public string RegisterType { get; set; }
    }
}