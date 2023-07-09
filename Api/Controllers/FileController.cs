using System.Net.Http.Headers;
using Api.Authorization;
using Core.NuggetAggregate;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly INuggetDomain _nuggetDomain;

    public FileController(IJwtService jwtService, INuggetDomain nuggetDomain)
    {
        _jwtService = jwtService;
        _nuggetDomain = nuggetDomain;
    }
    
    [HttpPost("nugget/{nuggetId:guid}"), DisableRequestSizeLimit]
    public async Task<IActionResult> Upload(Guid nuggetId, CancellationToken cancellationToken)
    {
        try
        {
            var formCollection = await Request.ReadFormAsync(cancellationToken);
            var file = formCollection.Files[0];
            
            var folderName = Path.Combine("Resources", "Images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);
                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream, cancellationToken);
                }

                await _nuggetDomain.UpdateUrlImageAsync(nuggetId, GetUserId(), fullPath, cancellationToken);
                
                return Ok(new { dbPath });
            }

            return BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }
    
    private Guid GetUserId()
    {
        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        return _jwtService.GetUserId(token);
    }
}