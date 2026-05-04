namespace WebApplication2.Domain.Models.DTO;

public record LatestRateRequest(string Base, string Target);
public record HistoryRateRequest(string Base, string Target, DateTime From, DateTime To);