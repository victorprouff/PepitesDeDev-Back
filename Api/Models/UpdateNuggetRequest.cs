namespace Api.Models;

public class UpdateNuggetRequest
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}