using System.Text.Json.Serialization;

namespace IntegrationImport.Application.Dtos;

/// <summary>
/// Single item (user or organization unit) that was rejected at API ingest,
/// with the list of validation errors that caused the rejection. HRMS uses
/// this to identify the offending row, fix it, and resend (with a new
/// ReferenceId).
/// </summary>
public class RejectedItemDto
{
    /// <summary>Zero-based index of this item in the original submitted batch.</summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }

    /// <summary>HrmsId of the item, when available (may be null if HrmsId itself was missing).</summary>
    [JsonPropertyName("hrmsId")]
    public string? HrmsId { get; set; }

    /// <summary>List of human-readable errors for this specific item.</summary>
    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new();
}
