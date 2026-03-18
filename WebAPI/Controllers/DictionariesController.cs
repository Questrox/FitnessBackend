using Application.Models.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionariesController(DictionariesService _dictionariesService) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<ReservationStatusDTO>>> GetReservationStatuses()
        {
            var statuses = await _dictionariesService.GetReservationStatusesAsync();
            return Ok(statuses);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<ReservationStatusDTO>>> GetTrainingStatuses()
        {
            var statuses = await _dictionariesService.GetTrainingStatusesAsync();
            return Ok(statuses);
        }
    }
}
