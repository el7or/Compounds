using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Puzzle.Compound.Common;
using Puzzle.Compound.Common.Enums;
using Puzzle.Compound.Models.PushNotifications;
using Puzzle.Compound.Services;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class RegistrationForUsersController : ControllerBase
    {
        private readonly IRegistrationForUsersService _service;
        private readonly IPushNotificationService _notificationService;

        public RegistrationForUsersController(IRegistrationForUsersService service, IPushNotificationService notificationService)
        {
            _service = service;
            _notificationService = notificationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(RegistrationForUsersOutputViewModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddMobile([FromBody] RegistrationForUsersAddViewModel model)
        {
            await _notificationService.CreatePushNotification(new PushNotificationAddViewModel
            {
                NotificationType = PushNotificationType.RegisteredUserApproved,
                RecordId = model.UserId
            });
            var output = await _service.AddRegistration(model);
            return Ok(new PuzzleApiResponse(output));
        }

        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteByUserId(Guid userId)
        {
            await _service.DeleteByUserId(userId);
            return Ok(new PuzzleApiResponse(new { }));
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveRegistrationId(string registerId)
        {
            await _service.RemoveRegistrationId(registerId);
            return Ok(new PuzzleApiResponse(new { }));
        }
    }
}
