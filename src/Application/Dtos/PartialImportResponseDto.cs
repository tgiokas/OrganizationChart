using System.Text.Json.Serialization;

namespace IntegrationImport.Application.Dtos;

/// <summary>
/// Response for an import call (user or organization unit) under
/// partial-success semantics: some items may be accepted (persisted, queued
/// for review/dispatch) while others may be rejected at ingest with per-item
/// errors.
/// </summary>
public class PartialImportResponseDto
{
    [JsonPropertyName("syncId")]
    public Guid SyncId { get; set; }


    [JsonPropertyName("referenceId")]
    public string ReferenceId { get; set; } = null!;


    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;


    [JsonPropertyName("receivedAt")]
    public DateTime ReceivedAt { get; set; }


    [JsonPropertyName("submittedCount")]
    public int SubmittedCount { get; set; }

  
    [JsonPropertyName("acceptedCount")]
    public int AcceptedCount { get; set; }

   
    [JsonPropertyName("rejectedCount")]
    public int RejectedCount { get; set; }

 
    [JsonPropertyName("rejected")]
    public List<RejectedItemDto> Rejected { get; set; } = new();


    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
