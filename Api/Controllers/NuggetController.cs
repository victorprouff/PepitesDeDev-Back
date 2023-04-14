using Api.Models.Nuggets;
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
    public async Task<IActionResult> Create(CreateNuggetRequest nugget)
    {
        var nuggetId = await _nuggetDomain.CreateAsync(new CreateNuggetCommand(nugget.Title, nugget.Description));
        return Ok(nuggetId);
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var nuggets = await _nuggetDomain.GetAsync();
        
        return Ok(nuggets.Select(n => (GetNuggetResponse)n));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var nugget = await _nuggetDomain.GetAsync(id);
        if (nugget == null)
        {
            return NotFound();
        }
        return Ok((GetNuggetResponse)nugget);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, UpdateNuggetRequest nuggetUpdate)
    {
        _nuggetDomain.UpdateAsync(new UpdateNuggetCommand(id, nuggetUpdate.Title, nuggetUpdate.Description));

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _nuggetDomain.DeleteAsync(id);
        return Ok();
    }
}