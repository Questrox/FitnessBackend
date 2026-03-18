using Application.Models.CreateDTOs;
using Application.Models.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController(MembershipService _membershipService, ILogger<MembershipController> _logger) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<MembershipDTO>>> GetMemberships()
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает список всех абонементов");

            var memberships = await _membershipService.GetMembershipsAsync();
            return Ok(memberships);
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<MembershipDTO?>> GetMembershipById(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает абонемент с id {id}");

            var membership = await _membershipService.GetMembershipByIdAsync(id);
            if (membership == null) return NotFound();
            return Ok(membership);
        }

        [HttpGet("[action]/{clientId}")]
        public async Task<ActionResult<IEnumerable<MembershipDTO>>> GetClientMemberships(int clientId)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} получает абонементы клиента с id {clientId}");

            var memberships = await _membershipService.GetClientMembershipsAsync(clientId);
            return Ok(memberships);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<MembershipDTO>> AddMembership(CreateMembershipDTO membership)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} создаёт абонемент");

            var newMembership = await _membershipService.AddMembershipAsync(membership);
            return Ok(newMembership);
        }

        [HttpPut("[action]/{id}")]
        public async Task<ActionResult<MembershipDTO>> UpdateMembership(int id, MembershipDTO membership)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != membership.Id) return BadRequest();

            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} обновляет абонемент с id {id}");

            var updatedMembership = await _membershipService.UpdateMembershipAsync(membership);
            return Ok(updatedMembership);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteMembership(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} удаляет абонемент с id {id}");

            await _membershipService.DeleteMembership(id);
            return NoContent();
        }
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> SoftDeleteMembership(int id)
        {
            var userName = User.Identity?.IsAuthenticated == true ? User.Identity.Name : "Гость";
            _logger.LogInformation($"Пользователь {userName} мягко удаляет абонемент с id {id}");

            await _membershipService.SoftDeleteMembership(id);
            return NoContent();
        }
    }
}
