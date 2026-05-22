namespace IntegrationImport.Application.Dtos;

public class ImportResponseDto
{
    public Guid SyncId { get; set; }
    public required string ReferenceId { get; set; }
    public required string Status { get; set; }
    public DateTime ReceivedAt { get; set; }
    public int ItemCount { get; set; }
    public required string Message { get; set; }
}
