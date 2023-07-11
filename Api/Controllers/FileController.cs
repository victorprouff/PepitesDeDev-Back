using System.Net.Http.Headers;
using Api.Authorization;
using Core.Interfaces;
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
    private readonly IFileStorage _fileStorage;

    public FileController(IJwtService jwtService, INuggetDomain nuggetDomain, IFileStorage fileStorage)
    {
        _jwtService = jwtService;
        _nuggetDomain = nuggetDomain;
        _fileStorage = fileStorage;
    }

    [HttpPost("nugget/{nuggetId:guid}"), DisableRequestSizeLimit]
    public async Task<IActionResult> Upload(Guid nuggetId, CancellationToken cancellationToken)
    {
        try
        {
            var formCollection = await Request.ReadFormAsync(cancellationToken);
            var file = formCollection.Files[0];

            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"')
                    ?? Guid.NewGuid().ToString();
                
                var bucketName = "nuggets-images";
                
                var fullPath = Path.Combine($"https://{bucketName}.cellar-c2.services.clever-cloud.com/", fileName);

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream, cancellationToken);
                    await _fileStorage.UploadFileAsync(bucketName, fileName, stream);
                }
                
                await _nuggetDomain.UpdateUrlImageAsync(nuggetId, GetUserId(), fullPath, cancellationToken);

                return Ok(new { fullPath });
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