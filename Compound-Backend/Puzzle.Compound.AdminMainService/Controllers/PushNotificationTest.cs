using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Models.PushNotifications;
using Puzzle.Compound.Services;
using System;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PushNotificationTest : ControllerBase
    {
        private readonly IPushNotificationService _pushNotificationService;

        public PushNotificationTest(IPushNotificationService pushNotificationService)
        {
            _pushNotificationService = pushNotificationService;
        }

        [HttpPost("AddSchedual")]
        public async Task<IActionResult> AddSchedual(PushNotificationAddViewModel notificationDto)
        {
            var notification = await _pushNotificationService.CreatePushNotification(notificationDto);
            return Ok(new PuzzleApiResponse(notification));
        }

        [HttpGet("GetScheduals")]
        public async Task<IActionResult> GetScheduals()
        {
            var list = await _pushNotificationService.GetAllNotifications();
            return Ok(new PuzzleApiResponse(list));
            //return Ok(list);
        }

        [HttpPost("ResetScheduals")]
        public async Task<IActionResult> ResetScheduals(Guid notificationId)
        {
            var data = await _pushNotificationService.ResetPushNotification(notificationId);
            return Ok(new PuzzleApiResponse(data));
        }

    }
}
