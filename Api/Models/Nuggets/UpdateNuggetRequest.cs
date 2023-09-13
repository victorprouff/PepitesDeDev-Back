namespace Api.Models.Nuggets;

public record UpdateNuggetRequest(string? Title, string? Content, bool IsEnabled, IFormFile? File);