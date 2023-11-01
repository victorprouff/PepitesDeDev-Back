using System.Runtime.CompilerServices;
using Api.Authorization;
using Api.Models.Comments;
using Core.CommentAggregate;
using Core.CommentAggregate.Models;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentDomain _commentDomain;

    public CommentController(ICommentDomain commentDomain, IJwtService jwtService) : base(jwtService)
    {
        _commentDomain = commentDomain;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCommentRequest comment, CancellationToken cancellationToken)
    {
        var nuggetId = await _commentDomain.CreateAsync(
            new CreateCommentCommand(comment.NuggetId, GetUserId(), comment.Content),
            cancellationToken);

        return Ok(nuggetId);
    }
}