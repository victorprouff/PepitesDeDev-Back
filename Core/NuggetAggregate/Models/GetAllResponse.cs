namespace Core.NuggetAggregate.Models;

public record GetAllResponse(int NbOfNuggets, IEnumerable<Nugget> Nuggets);