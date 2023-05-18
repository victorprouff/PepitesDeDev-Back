namespace Core.NuggetAggregate.Models;

public record UpdateNuggetCommand(Guid Id, Guid UserId, string? Title, string? Content);