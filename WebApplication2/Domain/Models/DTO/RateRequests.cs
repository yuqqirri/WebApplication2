namespace WebApplication2.Domain.Models.DTO;

public record LatestRateRequest(string Base, string Target);
public record HistoryRateRequest(string Base, string Target, DateTime From, DateTime To);

public record HistoryRequest(string Base, string Target, DateTime From, DateTime To);


public record ChangeRequest(string Base, string Target, DateTime Past, DateTime Current);


public record RateResponse(string Base, string Target, decimal Value, DateTime Date);