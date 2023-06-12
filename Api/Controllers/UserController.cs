using Api.Authorization;
using Api.Models.Users;
using Core.UserAggregate;
using Core.UserAggregate.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserDomain _userDomain;

    public UserController(IUserDomain userDomain)
    {
        _userDomain = userDomain;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate(AuthenticateUserRequest request)
    {
        var response = await _userDomain.Authenticate(request.EmailOrUsername, request.Password);
        if (response is null)
        {
            return BadRequest(new { message = "Email or password is incorrect" });
        }
        
        return Ok((AuthenticateUserResponse)response);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var userId = await _userDomain.CreateAsync(new CreateUserCommand(new Email(request.Email), request.Username, request.Password));
        return Ok(userId);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var user = await _userDomain.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok((GetUserResponse)user);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _userDomain.DeleteAsync(id);
        return Ok();
    }
}