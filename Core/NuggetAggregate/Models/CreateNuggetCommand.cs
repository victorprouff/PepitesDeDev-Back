namespace Core.NuggetAggregate.Models;

public record CreateNuggetCommand(string Title, string Content, Guid UserId, string? FileNameImage, MemoryStream Stream);