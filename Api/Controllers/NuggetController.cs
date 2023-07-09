using Api.Authorization;
using Api.Models.Nuggets;
using Core.NuggetAggregate;
using Core.NuggetAggregate.Models;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Nugget = Api.Models.Nuggets.Nugget;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class NuggetController : ControllerBase
{
    private readonly INuggetDomain _nuggetDomain;
    private readonly IJwtService _jwtService;

    public NuggetController(INuggetDomain nuggetDomain, IJwtService jwtService)
    {
        _nuggetDomain = nuggetDomain;
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken, int limit = 10, int offset = 0)
    {
        var response = await _nuggetDomain.GetAllAsync(limit, offset, cancellationToken);
        
        return Ok(new GetAllNuggetResponse(response.NbOfNuggets, response.Nuggets.Select(n => (Nugget)n)));
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var nugget = await _nuggetDomain.GetAsync(id, cancellationToken);

        return Ok((GetNuggetResponse)nugget);
    }
    
    [HttpGet("user")]
    public async Task<IActionResult> GetListByUserId(CancellationToken cancellationToken, int limit = 10, int offset = 0)
    {
        var response = await _nuggetDomain.GetAllByUserIdOrAdminAsync(GetUserId(), limit, offset, cancellationToken);

        return Ok(new GetAllNuggetResponse(response.NbOfNuggets, response.Nuggets.Select(n => (Nugget)n)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateNuggetRequest nugget, CancellationToken cancellationToken)
    {
        var nuggetId = await _nuggetDomain.CreateAsync(
            new CreateNuggetCommand(nugget.Title, nugget.Content, GetUserId()), cancellationToken);
        
        return Ok(nuggetId);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, UpdateNuggetRequest nuggetUpdate, CancellationToken cancellationToken)
    {
        _nuggetDomain.UpdateAsync(
            new UpdateNuggetCommand(id, GetUserId(), nuggetUpdate.Title, nuggetUpdate.Content),
            cancellationToken);

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id, CancellationToken cancellationToken)
    {
        _nuggetDomain.DeleteAsync(id, GetUserId(), cancellationToken);
        return Ok();
    }

    private Guid GetUserId()
    {
        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        return _jwtService.GetUserId(token);
    }
}