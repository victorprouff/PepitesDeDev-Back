using Api.Models.Users;
using Core.UserAggregate;
using Core.UserAggregate.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserDomain _userDomain;

    public UserController(IUserDomain userDomain)
    {
        _userDomain = userDomain;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest nugget)
    {
        var userId = await _userDomain.CreateAsync(new CreateUserCommand(nugget.Email, nugget.Password));
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