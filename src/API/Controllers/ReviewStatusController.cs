using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Interfaces;

namespace IntegrationImport.Api.Controllers;

[ApiController]
[Route("api/integration")]
[Authorize]
public class ReviewStatusController : ControllerBase
{
    private readonly IReviewStatusService _reviewStatusService;

    public ReviewStatusController(IReviewStatusService reviewStatusService)
    {
        _reviewStatusService = reviewStatusService;
    }

    [HttpGet("getstatus/{syncId:guid}")]
    public async Task<IActionResult> GetImportStatus([FromRoute] Guid syncId, CancellationToken ct)
    {
        var result = await _reviewStatusService.GetStatusAsync(syncId, ct);

        if (result is null || !result.Success)
        {
            return Accepted(result);
        }

        return Ok(result);
    }

    [HttpPost("review-items")]
    public async Task<IActionResult> ReviewItems([FromBody] ReviewImportRequestDto request, CancellationToken ct)
    {
        var result = await _reviewStatusService.ReviewItemsAsync(request, ct);
        if (!result.Success)
        {
            return Accepted(result);
        }

        return Ok(result);
    }
}