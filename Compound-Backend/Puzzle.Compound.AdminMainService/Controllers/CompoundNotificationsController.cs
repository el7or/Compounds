using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Compound.Common;
using Puzzle.Compound.Models.Notifications;
using Puzzle.Compound.Services;
using System;
using System.Threading.Tasks;

namespace Puzzle.Compound.AdminMainService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CompoundNotificationsController : ControllerBase
    {
        private readonly ICompoundNotificationService _compoundNotificationService;

        public CompoundNotificationsController(ICompoundNotificationService compoundNotificationService)
        {
            _compoundNotificationService = compoundNotificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] NotificationFilterViewModel model)
        {
            var compoundNotificationList = await _compoundNotificationService.GetAsync(model);
            return Ok(new PuzzleApiResponse(compoundNotificationList));
        }

        [HttpGet("{notificationId:Guid}/{ownerRegistrationId:Guid?}")]
        public async Task<IActionResult> GetById(Guid notificationId, Guid? ownerRegistrationId)
        {
            var compoundNotification = await _compoundNotificationService.GetByIdAsync(notificationId, ownerRegistrationId);
            return Ok(new PuzzleApiResponse(compoundNotification));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationInputViewModel model)
        {
            var operationState = await _compoundNotificationService.AddAsync(model);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] NotificationInputViewModel model)
        {
            var operationState = await _compoundNotificationService.UpdateAsync(model);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var operationState = await _compoundNotificationService.DeleteAsync(id);
            return Ok(new PuzzleApiResponse(operationState));
        }

        [HttpGet("mobile")]
        public async Task<IActionResult> GetMobileNotification([FromQuery] NotificationFilterViewModel model, [FromHeader] string Language)
        {
            var compoundNotificationList = await _compoundNotificationService.GetMobileNotificationsAsync(model, Language);
            return Ok(new PuzzleApiResponse(compoundNotificationList));
        }

        [HttpGet("mobile/{notificationId:Guid}/{ownerRegistrationId:Guid?}")]
        public async Task<IActionResult> GetMobileNotificationById(Guid notificationId, Guid? ownerRegistrationId, [FromHeader] string Language)
        {
            var compoundNotification = await _compoundNotificationService.GetMobileNotificationByIdAsync(notificationId, ownerRegistrationId, Language);
            return Ok(new PuzzleApiResponse(compoundNotification));
        }

        [HttpGet("unread-count/{compoundId:Guid}/{ownerRegistrationId:Guid}")]
        public async Task<IActionResult> GetUnreadNotificationsCount(Guid compoundId, Guid ownerRegistrationId)
        {
            var compoundNotificationList = await _compoundNotificationService.GetUnreadNotificationsCount(compoundId, ownerRegistrationId);
            return Ok(new PuzzleApiResponse(compoundNotificationList));
        }
    }
}
