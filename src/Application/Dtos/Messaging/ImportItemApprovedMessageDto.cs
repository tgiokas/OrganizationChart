using IntegrationImport.Domain.Enums;

namespace IntegrationImport.Application.Dtos;

public class ImportΙtemApprovedMessageDto
{
    public Guid SyncId { get; set; }
    public Guid ItemId { get; set; }
    public string ReferenceId { get; set; } = string.Empty;
    public ImportEntityType EntityType { get; set; }
    public string Status { get; set; } = null!;
    public DateTime ApprovedAtUtc { get; set; }
}
