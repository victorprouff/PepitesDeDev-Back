namespace Core.NuggetAggregate.Models;

public record UpdateNuggetImageCommand(Guid NuggetId, Guid UserId, string? FileName, MemoryStream Stream);