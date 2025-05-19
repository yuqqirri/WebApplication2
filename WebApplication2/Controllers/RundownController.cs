// Controllers/RundownController.cs
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RundownController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RundownController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/rundown
        [HttpPost]
        public async Task<ActionResult<Rundown>> PostRundown(Rundown rundown)
        {
            if (rundown == null || rundown.Rundown_value <= 0)
            {
                return BadRequest("Rundown value must be greater than zero.");
            }

            _context.Rundowns.Add(rundown);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRundown), new { id = rundown.Rundown_id }, rundown);
        }

        // Метод для получения сводки (если нужно)
        [HttpGet("{id}")]
        public async Task<ActionResult<Rundown>> GetRundown(int id)
        {
            var rundown = await _context.Rundowns.FindAsync(id);

            if (rundown == null)
            {
                return NotFound();
            }

            return rundown;
        }
    }
}