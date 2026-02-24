using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ExternalIntegrations.OrganizationChart.Application.Dtos;
using ExternalIntegrations.OrganizationChart.Application.Interfaces;

namespace ExternalIntegrations.OrganizationChart.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize] // Partner must have a valid JWT from Keycloak (client_credentials)
public class ImportController : ControllerBase
{
    private readonly IImportService _importService;

    public ImportController(IImportService importService)
    {
        _importService = importService;
    }

    /// Full batch import (initial push or reconciliation).
    [HttpPost("users/batch")]
    public async Task<IActionResult> BatchImport(BatchImportRequest request)
    {
        //  Call _importService.SubmitBatchImportAsync(request)
        throw new NotImplementedException();
    }

    /// Delta/incremental import (ongoing changes).
    [HttpPost("users/delta")]
    public async Task<IActionResult> DeltaImport(DeltaImportRequest request)
    {
        //  Call _importService.SubmitDeltaImportAsync(request)
        throw new NotImplementedException();
    }

    /// Get the status of an import job.
    [HttpGet("jobs/{jobId:guid}")]
    public async Task<IActionResult> GetJobStatus([FromRoute] Guid jobId)
    {
        //  Call _importService.GetJobStatusAsync(jobId)
        throw new NotImplementedException();
    }

    /// Get the full report of an import job including per-user results.
    [HttpGet("jobs/{jobId:guid}/report")]
    public async Task<IActionResult> GetJobReport([FromRoute] Guid jobId)
    {
        //  Call _importService.GetJobReportAsync(jobId)
        throw new NotImplementedException();
    }
}
