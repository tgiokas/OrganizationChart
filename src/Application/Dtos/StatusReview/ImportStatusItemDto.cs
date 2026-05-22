namespace IntegrationImport.Application.Dtos;

public class ImportStatusItemDto
{
    public Guid ItemId { get; set; }
    public string HrmsId { get; set; } = string.Empty;
    public string ReviewStatus { get; set; } = string.Empty;
    public string ProcessStatus { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
