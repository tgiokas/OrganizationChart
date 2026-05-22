namespace IntegrationImport.Application.Dtos;

public enum ReviewDecision
{
    APPROVED,
    REJECTED
}

public class ReviewImportRequestDto
{
    public Guid SyncId { get; set; }
    public string? ReviewedBy { get; set; }
    public List<ReviewImportItemDto> Items { get; set; } = [];
}
