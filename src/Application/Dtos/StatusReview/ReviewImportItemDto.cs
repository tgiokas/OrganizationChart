using IntegrationImport.Domain.Enums;

namespace IntegrationImport.Application.Dtos;

public class ReviewImportItemDto
{
    public Guid ItemId { get; set; }
    public ImportEntityType EntityType { get; set; }
    public ReviewDecision Decision { get; set; }
    public string? Notes { get; set; }
}
