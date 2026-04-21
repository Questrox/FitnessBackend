using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(NotificationService _notificationService, ILogger<NotificationController> _logger) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<CancellationNotificationDTO>>> GetNotifications()
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает список всех уведомлений");

            var notifications = await _notificationService.GetNotificationsAsync();
            return Ok(notifications);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<CancellationNotificationDTO>>> GetActiveNotifications()
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает список активных уведомлений");

            var notifications = await _notificationService.GetActiveNotificationsAsync();
            return Ok(notifications);
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<CancellationNotificationDTO?>> GetNotificationById(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает уведомление с id {id}");

            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null) return NotFound();
            return Ok(notification);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<CancellationNotificationDTO>> AddNotification(CreateCancellationNotificationDTO notification)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создаёт уведомление");

            var newNotification = await _notificationService.AddNotificationAsync(notification);
            return Ok(newNotification);
        }

        [HttpPut("[action]/{id}")]
        public async Task<ActionResult<CancellationNotificationDTO>> UpdateNotification(int id, CancellationNotificationDTO notification)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != notification.Id) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет уведомление с id {id}");

            var updatedNotification = await _notificationService.UpdateNotificationAsync(notification);
            return Ok(updatedNotification);
        }

        [HttpPut("[action]/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CancellationNotificationDTO>> ConfirmNotification(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет уведомление с id {id}");

            string adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var updatedNotification = await _notificationService.ConfirmNotificationAsync(id, adminId);
            return Ok(updatedNotification);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет уведомление с id {id}");

            await _notificationService.DeleteNotification(id);
            return NoContent();
        }
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> SoftDeleteNotification(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} мягко удаляет уведомление с id {id}");

            await _notificationService.SoftDeleteNotification(id);
            return NoContent();
        }
    }
}
