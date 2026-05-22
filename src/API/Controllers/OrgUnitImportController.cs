using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Interfaces;


namespace IntegrationImport.Api.Controllers;


[ApiController]
[Route("api/integration")]
[Authorize]
public class OrgUnitImportController : ControllerBase
{
    private readonly IOrgUnitImportService _orgUnitImportService;
    private readonly IReviewStatusService _reviewStatusService;

    public OrgUnitImportController(IOrgUnitImportService orgUnitImportService, IReviewStatusService reviewStatusService)
    {
        _orgUnitImportService = orgUnitImportService;
        _reviewStatusService = reviewStatusService;
    }


    [HttpPost("organizationUnits")]
    [ProducesResponseType(typeof(Result<PartialImportResponseDto>), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(Result<PartialImportResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportOrgUnits([FromBody] OrgUnitImportRequestDto request, CancellationToken ct)
    {
        var result = await _orgUnitImportService.ImportOrgUnitsAsync(request, ct);

        // Partial-success semantics:
        
        if (!result.Success)
            return BadRequest(result);

        return Accepted(result);
    }
}
