using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingReservationController(TrainingReservationService _reservationService, ClientService _clientService,
        ILogger<TrainingReservationController> _logger) : ControllerBase
    {
        [HttpGet("[action]/{clientId}")]
        public async Task<ActionResult<IEnumerable<TrainingReservationDTO>>> GetClientReservations(int clientId)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает записи на тренировки клиента с id {clientId}");

            var reservations = await _reservationService.GetClientReservationsAsync(clientId);
            return Ok(reservations);
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<TrainingReservationDTO?>> GetReservationById(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает запись на тренировку с id {id}");

            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<ActionResult<TrainingReservationDTO>> AddReservation(CreateTrainingReservationDTO reservation)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            bool isClient = false;
            if (reservation.ClientId == null) // Если запрос делает сам клиент
            {
                isClient = true;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    throw new UnauthorizedAccessException("Пользователь не авторизован");
                var client = await _clientService.GetClientByUserIdAsync(userId);
                if (client == null)
                    throw new KeyNotFoundException("Клиент не найден");
                reservation.ClientId = client.Id;
                _logger.LogInformation($"Клиент {userName} создаёт запись на тренировку с Id {reservation.TrainingId}");
            }
            else
                _logger.LogInformation($"Пользователь {userName} создаёт запись на тренировку с Id {reservation.TrainingId} " +
                    $"для клиента c Id {reservation.ClientId}");

            var newReservation = await _reservationService.AddReservationAsync(reservation, isClient);
            return Ok(newReservation);
        }

        [HttpPut("[action]/{id}")]
        public async Task<ActionResult<TrainingReservationDTO>> UpdateReservation(int id, TrainingReservationDTO reservation)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != reservation.Id) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет запись на тренировку с id {id}");

            var updatedReservation = await _reservationService.UpdateReservationAsync(reservation);
            return Ok(updatedReservation);
        }
        [HttpPut("[action]/{id}")]
        public async Task<ActionResult<TrainingReservationDTO>> CancelReservation(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} отменяет запись на тренировку с id {id}");

            var updatedReservation = await _reservationService.CancelReservationAsync(id);
            return Ok(updatedReservation);
        }
        [HttpPut("[action]/{reservationId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TrainingReservationDTO>> ConfirmReservationPayment(int reservationId, CreatePaymentDTO dto)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} подтверждает оплату записи на тренировку с id {reservationId}");
            dto.AdminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _reservationService.ConfirmPaymentAsync(reservationId, dto);
            return Ok(result);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет запись на тренировку с id {id}");

            await _reservationService.DeleteReservation(id);
            return NoContent();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> SoftDeleteReservation(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} мягко удаляет запись на тренировку с id {id}");

            await _reservationService.SoftDeleteReservation(id);
            return NoContent();
        }
    }
}
