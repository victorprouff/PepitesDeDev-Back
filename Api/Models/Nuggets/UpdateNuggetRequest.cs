namespace Api.Models.Nuggets;

public record UpdateNuggetRequest(string? Title, string? Content, IFormFile? File);