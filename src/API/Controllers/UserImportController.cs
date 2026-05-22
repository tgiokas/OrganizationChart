using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Interfaces;

namespace IntegrationImport.Api.Controllers;

[ApiController]
[Route("api/integration")]
[Authorize]
public class UserImportController : ControllerBase
{
    private readonly IUserImportService _userImportService;

    public UserImportController(IUserImportService userImportService)
    {
        _userImportService = userImportService;
    }

    [HttpPost("users")]
    [ProducesResponseType(typeof(Result<PartialImportResponseDto>), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(Result<PartialImportResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportUsers([FromBody] UsersImportRequestDto request, CancellationToken ct)
    {
        var result = await _userImportService.ImportUsersAsync(request, ct);

        // Partial-success semantics:
        //  - success (full OR partial accept) → 202 Accepted, body carries acceptedCount + rejected[]
        //  - global failure (no items persisted) → 400 BadRequest, body still carries rejected[] if available

        if (!result.Success)
            return BadRequest(result);

        return Accepted(result);
    }
}
