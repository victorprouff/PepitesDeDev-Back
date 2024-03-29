namespace Core.NuggetAggregate.Models;

public record CreateNuggetCommand(string Title, string Content, bool IsEnabled, Guid UserId, string? FileNameImage, MemoryStream Stream);