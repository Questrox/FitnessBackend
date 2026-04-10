using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController(TrainingService _trainingService, ILogger<TrainingController> _logger) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<TrainingDTO>>> GetTrainingsForPeriod(DateTime start, DateTime end)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает тренировки за период {start.ToLocalTime().Date} - {end.ToLocalTime().Date}");

            var trainings = await _trainingService.GetTrainingsForPeriodAsync(start.ToLocalTime().Date, end.ToLocalTime().Date);
            return Ok(trainings);
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<TrainingDTO?>> GetTrainingById(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает тренировку с id {id}");

            var training = await _trainingService.GetTrainingByIdAsync(id);
            if (training == null) return NotFound();
            return Ok(training);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<TrainingDTO>> AddTraining(CreateTrainingDTO training)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создаёт тренировку");

            var newTraining = await _trainingService.AddTrainingAsync(training);
            return Ok(newTraining);
        }

        [HttpPut("[action]/{id}")]
        public async Task<ActionResult<TrainingDTO>> UpdateTraining(int id, TrainingDTO training)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != training.Id) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет тренировку с id {id}");

            var updatedTraining = await _trainingService.UpdateTrainingAsync(training);
            return Ok(updatedTraining);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteTraining(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет тренировку с id {id}");

            await _trainingService.DeleteTraining(id);
            return NoContent();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> SoftDeleteTraining(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} мягко удаляет тренировку с id {id}");

            await _trainingService.SoftDeleteTraining(id);
            return NoContent();
        }
    }
}
