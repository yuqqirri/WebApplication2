using Microsoft.AspNetCore.Mvc;
using WebApplication2.Domain.Interfaces;
using WebApplication2.Domain.Models.DTO;

namespace WebApplication2.Controllers;

[ApiController]
[Route("api/rates")]
public class RatesController(ICurrencyService currencyService) : ControllerBase
{
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest([FromQuery] LatestRateRequest request)
    {
        var result = await currencyService.GetLatestRateAsync(request);
        if (result == null) return NotFound(new { Message = "Данные по указанной валюте не найдены" });
        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] HistoryRequest request)
    {
        var result = await currencyService.GetHistoryAsync(request);
        return Ok(result);
    }

    [HttpGet("change")]
    public async Task<IActionResult> GetChange([FromQuery] ChangeRequest request)
    {
        var percent = await currencyService.GetPriceChangeAsync(request);
        if (percent == null) return BadRequest(new { Message = "Не удалось найти курсы за указанные даты для расчета" });

        return Ok(new
        {
            Base = request.Base,
            Target = request.Target,
            ChangePercent = $"{percent:F2}%",
            Description = $"Изменение курса с {request.Past:yyyy-MM-dd} по {request.Current:yyyy-MM-dd}"
        });
    }
}