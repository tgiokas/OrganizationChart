namespace IntegrationImport.Application.Dtos;

public class ImportStatusResponseDto
{
    public Guid SyncId { get; set; }
    public string ReferenceId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public int ItemCount { get; set; }
    public int PendingReviewItemCount { get; set; }
    public int ApprovedItemCount { get; set; }
    public int RejectedItemCount { get; set; }

    public int DispatchedItemCount { get; set; }
    public int AppliedItemCount { get; set; }
    public int FailedItemCount { get; set; }
    public string? Notes { get; set; }
    public List<ImportStatusItemDto> Items { get; set; } = [];
}
