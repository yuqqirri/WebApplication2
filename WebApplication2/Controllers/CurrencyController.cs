using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Domain.Models;
using WebApplication2.Domain.Interfaces; // Используем интерфейсы

namespace WebApplication2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    // Основной конструктор с сервисом
    public class CurrencyController(ICurrencyService currencyService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Currency>> PostCurrency([FromBody] Currency currency)
        {
            if (string.IsNullOrWhiteSpace(currency.Currency_name))
            {
                return BadRequest("Currency name is required.");
            }

            // Вся работа с базой теперь внутри сервиса
            var created = await currencyService.CreateCurrencyAsync(currency);
            return CreatedAtAction(nameof(GetCurrency), new { id = created.Currency_id }, created);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Currency>> GetCurrency(int id)
        {
            var currency = await currencyService.GetCurrencyAsync(id);
            if (currency == null) return NotFound();

            return Ok(currency);
        }
    }
}