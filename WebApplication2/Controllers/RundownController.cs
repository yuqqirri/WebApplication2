using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Domain.Models;
using WebApplication2.Domain.Interfaces;

namespace WebApplication2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RundownController(ICurrencyService currencyService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Rundown>> PostRundown([FromBody] Rundown rundown)
        {
            if (rundown.Rundown_value <= 0)
            {
                return BadRequest("Rundown value must be greater than zero.");
            }

            var created = await currencyService.CreateRundownAsync(rundown);
            return CreatedAtAction(nameof(GetRundown), new { id = created.Rundown_id }, created);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Rundown>> GetRundown(int id)
        {
            var rundown = await currencyService.GetRundownAsync(id);
            if (rundown == null) return NotFound();

            return Ok(rundown);
        }
    }
}