using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingReservationController(TrainingReservationService _reservationService, ILogger<TrainingReservationController> _logger) : ControllerBase
    {
        [HttpGet("[action]/{clientId}")]
        public async Task<ActionResult<IEnumerable<TrainingReservationDTO>>> GetClientReservations(int clientId)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает бронирования клиента с id {clientId}");

            var reservations = await _reservationService.GetClientReservationsAsync(clientId);
            return Ok(reservations);
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<TrainingReservationDTO?>> GetReservationById(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает бронирование с id {id}");

            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<TrainingReservationDTO>> AddReservation(CreateTrainingReservationDTO reservation)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создаёт бронирование");

            var newReservation = await _reservationService.AddReservationAsync(reservation);
            return Ok(newReservation);
        }

        [HttpPut("[action]/{id}")]
        public async Task<ActionResult<TrainingReservationDTO>> UpdateReservation(int id, TrainingReservationDTO reservation)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != reservation.Id) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет бронирование с id {id}");

            var updatedReservation = await _reservationService.UpdateReservationAsync(reservation);
            return Ok(updatedReservation);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет бронирование с id {id}");

            await _reservationService.DeleteReservation(id);
            return NoContent();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> SoftDeleteReservation(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} мягко удаляет бронирование с id {id}");

            await _reservationService.SoftDeleteReservation(id);
            return NoContent();
        }
    }
}
