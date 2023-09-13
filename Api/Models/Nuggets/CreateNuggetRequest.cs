namespace Api.Models.Nuggets;

public record CreateNuggetRequest(IFormFile? File, string Title, string Content, bool IsEnabled = true);