namespace Core.NuggetAggregate.Models;

public record DeleteNuggetImageCommand(Guid NuggetId, Guid UserId);