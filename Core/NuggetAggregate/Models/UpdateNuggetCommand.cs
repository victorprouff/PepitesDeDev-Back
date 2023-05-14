namespace Core.NuggetAggregate.Models;

public record UpdateNuggetCommand(Guid Id, string? Title, string? Content);