using Core.Services.Interfaces;

namespace Api.Controllers;

public class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
{
    private readonly IJwtService _jwtService;

    public ControllerBase(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    internal Guid GetUserId()
    {
        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        return _jwtService.GetUserId(token);
    }
}