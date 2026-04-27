using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoachScheduleController(CoachScheduleService _scheduleService, ILogger<CoachScheduleController> _logger) : ControllerBase
    {
        [HttpGet("[action]/{coachId}")]
        public async Task<ActionResult<IEnumerable<CoachScheduleDTO>>> GetScheduleForCoach(int coachId)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает расписание тренера с id {coachId}");

            var schedules = await _scheduleService.GetScheduleForCoachAsync(coachId);
            return Ok(schedules);
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<CoachScheduleDTO?>> GetCoachScheduleById(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает расписание с id {id}");

            var schedule = await _scheduleService.GetCoachScheduleByIdAsync(id);
            if (schedule == null) return NotFound();
            return Ok(schedule);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<CoachScheduleDTO>> AddCoachSchedule(CreateCoachScheduleDTO schedule)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создаёт расписание тренера с Id {schedule.CoachId}");

            var newSchedule = await _scheduleService.AddCoachScheduleAsync(schedule);
            return Ok(newSchedule);
        }

        [HttpPut("[action]/{id}")]
        public async Task<ActionResult<CoachScheduleDTO>> UpdateCoachSchedule(int id, CoachScheduleDTO schedule)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != schedule.Id) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет расписание с id {id}");

            var updatedSchedule = await _scheduleService.UpdateCoachScheduleAsync(schedule);
            return Ok(updatedSchedule);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteCoachSchedule(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет расписание с id {id}");

            await _scheduleService.DeleteCoachSchedule(id);
            return NoContent();
        }
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> SoftDeleteCoachSchedule(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} мягко удаляет расписание с id {id}");

            await _scheduleService.SoftDeleteCoachSchedule(id);
            return NoContent();
        }
    }
}
