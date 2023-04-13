using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NuggetController : ControllerBase
{
    private readonly List<Nugget> _nuggets = new();

    [HttpPost]
    public IActionResult Create(CreateNuggetRequest nugget)
    {
        _nuggets.Add(new Nugget(nugget.Title, nugget.Description));
        return Ok();
    }

    [HttpGet]
    public IActionResult GetList()
    {
        return Ok(_nuggets.Select(n => (GetNuggetResponse)n));
    }
    
    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        var nugget = _nuggets.Find(n => n.Id == id);
        if (nugget == null)
        {
            return NotFound();
        }
        return Ok((GetNuggetResponse)nugget);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, UpdateNuggetRequest nuggetUpdate)
    {
        var nugget = _nuggets.Find(n => n.Id == id);
        if (nugget == null)
        {
            return NotFound();
        }
        
        nugget.Update(nuggetUpdate.Title, nuggetUpdate.Description);
        
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var nugget = _nuggets.Find(n => n.Id == id);
        if (nugget == null)
        {
            return NotFound();
        }
        _nuggets.Remove(nugget);
        return Ok();
    }
}

public class Nugget
{
    public Nugget(string title, string description)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
    }
    public Guid Id { get; }
    public string Title { get; private set; }
    public string Description { get; private set; }

    public void Update(string? title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title) is false)
        {
            Title = title;
        }

        if (string.IsNullOrWhiteSpace(description) is false)
        {
            Description = description;
        }
    }
}
