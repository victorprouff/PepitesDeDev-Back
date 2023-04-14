using Api.Models;
using Core.NuggetAggregate;
using Core.NuggetAggregate.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NuggetController : ControllerBase
{
    private readonly INuggetDomain _nuggetDomain;

    public NuggetController(INuggetDomain nuggetDomain)
    {
        _nuggetDomain = nuggetDomain;
    }

    [HttpPost]
    public IActionResult Create(CreateNuggetRequest nugget)
    {
        _nuggetDomain.Create(new CreateNuggetCommand(nugget.Title, nugget.Description));
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var nuggets = await _nuggetDomain.Get();
        
        return Ok(nuggets.Select(n => (GetNuggetResponse)n));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var nugget = await _nuggetDomain.Get(id);
        if (nugget == null)
        {
            return NotFound();
        }
        return Ok((GetNuggetResponse)nugget);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, UpdateNuggetRequest nuggetUpdate)
    {
        _nuggetDomain.Update(new UpdateNuggetCommand(id, nuggetUpdate.Title, nuggetUpdate.Description));

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _nuggetDomain.Delete(id);
        return Ok();
    }
}