using Api.Authorization;
using Api.Models.Users;
using Core.Services.Interfaces;
using Core.UserAggregate;
using Core.UserAggregate.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize]
[EnableCors]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserDomain _userDomain;
    private readonly IJwtService _jwtService;
    
    public UserController(IUserDomain userDomain, IJwtService jwtService)
    {
        _userDomain = userDomain;
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate(AuthenticateUserRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _userDomain.Authenticate(request.EmailOrUsername, request.Password, cancellationToken);
        if (response is null)
        {
            return BadRequest(new { message = "Email or password is incorrect" });
        }
        
        return Ok((AuthenticateUserResponse)response);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var userId = await _userDomain.CreateAsync(
            new CreateUserCommand(new Email(request.Email),
                request.Username,
                request.Password),
            cancellationToken);
        
        return Ok(userId);
    }

    [HttpPut("email")]
    public async Task<IActionResult> UpdateEmail(UpdateEmailRequest request, CancellationToken cancellationToken = default)
    {
        await _userDomain.UpdateEmail(GetUserId(), new Email(request.Email), cancellationToken);

        return NoContent();
    }
    
    [HttpPut("username")]
    public async Task<IActionResult> UpdateUsername(UpdateUsernameRequest request, CancellationToken cancellationToken = default)
    {
        await _userDomain.UpdateUsername(GetUserId(), request.Username, cancellationToken);

        return NoContent();
    }
    
    [HttpPut("password")]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest request, CancellationToken cancellationToken = default)
    {
        await _userDomain.UpdatePassword(GetUserId(), request.OldPassword, request.NewPassword, cancellationToken);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userDomain.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound();
        }
        return Ok((GetUserResponse)user);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id, CancellationToken cancellationToken = default)
    {
        _userDomain.DeleteAsync(id, cancellationToken);
        return Ok();
    }
    
    private Guid GetUserId()
    {
        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        return _jwtService.GetUserId(token);
    }
}