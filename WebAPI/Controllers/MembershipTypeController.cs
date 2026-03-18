using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipTypeController(MembershipTypeService _typeService, ILogger<MembershipTypeController> _logger) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<MembershipTypeDTO>>> GetMembershipTypes()
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает список всех типов абонементов");

            var types = await _typeService.GetMembershipTypesAsync();
            return Ok(types);
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<MembershipTypeDTO?>> GetMembershipTypeById(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает тип абонемента с id {id}");

            var type = await _typeService.GetMembershipTypeByIdAsync(id);
            if (type == null) return NotFound();
            return Ok(type);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<MembershipTypeDTO>> AddMembershipType(CreateMembershipTypeDTO type)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создаёт тип абонемента");

            var newType = await _typeService.AddMembershipTypeAsync(type);
            return Ok(newType);
        }

        [HttpPut("[action]/{id}")]
        public async Task<ActionResult<MembershipTypeDTO>> UpdateMembershipType(int id, MembershipTypeDTO type)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != type.Id) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет тип абонемента с id {id}");

            var updatedType = await _typeService.UpdateMembershipTypeAsync(type);
            return Ok(updatedType);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteMembershipType(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет тип абонемента с id {id}");

            await _typeService.DeleteMembershipType(id);
            return NoContent();
        }
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> SoftDeleteMembershipType(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} мягко удаляет тип абонемента с id {id}");

            await _typeService.SoftDeleteMembershipType(id);
            return NoContent();
        }
    }
}
