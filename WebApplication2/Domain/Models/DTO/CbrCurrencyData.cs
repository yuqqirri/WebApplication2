namespace WebApplication2.Domain.Models.DTO;

public class CbrCurrencyData
{
    public DateTime Date { get; set; }
    public Dictionary<string, CbrCurrencyInfo> Valute { get; set; } = new();
}

public class CbrCurrencyInfo
{
    public decimal Value { get; set; }
    public int Nominal { get; set; }
}