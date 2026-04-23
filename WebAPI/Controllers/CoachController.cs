using Application.Models;
using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoachController(CoachService _coachService, ILogger<CoachController> _logger) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<CoachDTO>>> GetCoaches()
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает список всех тренеров");

            var coaches = await _coachService.GetCoachesAsync();
            return Ok(coaches);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<CoachDTO>>> GetAvailableCoaches(DateTime start, DateTime end)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает свободных тренеров в период {start.ToLocalTime()} - {end.ToLocalTime()}");

            var coaches = await _coachService.GetAvailableCoachesAsync(start.ToLocalTime(), end.ToLocalTime());
            return Ok(coaches);
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<CoachDTO?>> GetCoachById(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает тренера с id {id}");

            var coach = await _coachService.GetCoachByIdAsync(id);
            if (coach == null) return NotFound();
            return Ok(coach);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<LoginCredentials>> AddCoach(CreateCoachDTO coach)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создаёт тренера");

            var newCoach = await _coachService.AddCoachAsync(coach);
            return Ok(newCoach);
        }

        [HttpPut("[action]/{id}")]
        public async Task<ActionResult<CoachDTO>> UpdateCoach(int id, CoachDTO coach)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != coach.Id) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет тренера с id {id}");

            var updatedCoach = await _coachService.UpdateCoachAsync(coach);
            return Ok(updatedCoach);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCoach(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет тренера с id {id}");

            await _coachService.DeleteCoach(id);
            return NoContent();
        }
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> SoftDeleteCoach(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} мягко удаляет тренера с id {id}");

            await _coachService.SoftDeleteCoach(id);
            return NoContent();
        }
    }
}
