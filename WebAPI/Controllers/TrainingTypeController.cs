using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingTypeController(TrainingTypeService _typeService, ILogger<TrainingTypeController> _logger,
        IWebHostEnvironment _env) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<TrainingTypeDTO>>> GetTrainingTypes()
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает список всех типов тренировок");

            var types = await _typeService.GetTrainingTypesAsync();
            return Ok(types);
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<TrainingTypeDTO?>> GetTrainingTypeById(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает тип тренировки с id {id}");

            var type = await _typeService.GetTrainingTypeByIdAsync(id);
            if (type == null) return NotFound();
            return Ok(type);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<TrainingTypeDTO>> AddTrainingType(CreateTrainingTypeDTO type)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создаёт тип тренировки");

            var newType = await _typeService.AddTrainingTypeAsync(type, _env);
            return Ok(newType);
        }

        [HttpPut("[action]/{routeId}")]
        public async Task<ActionResult<TrainingTypeDTO>> UpdateTrainingType(int routeId, TrainingTypeDTO type)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (routeId != type.Id) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет тип тренировки с id {routeId}");

            var updatedType = await _typeService.UpdateTrainingTypeAsync(type, _env);
            return Ok(updatedType);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteTrainingType(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет тип тренировки с id {id}");

            await _typeService.DeleteTrainingType(id);
            return NoContent();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> SoftDeleteTrainingType(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} мягко удаляет тип тренировки с id {id}");

            await _typeService.SoftDeleteTrainingType(id);
            return NoContent();
        }
    }
}
