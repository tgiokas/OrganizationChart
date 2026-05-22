using System.Text.Json.Serialization;
namespace IntegrationImport.Application.Dtos;
public class OrgUnitItemDto
{
    [JsonPropertyName("hrmsId")]
    public required string HrmsId { get; set; }

    [JsonPropertyName("action")]
    public required string Action { get; set; }   // "CREATE" | "UPDATE" | "DEACTIVATE"

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("abbreviation")]
    public string? Abbreviation { get; set; }

    [JsonPropertyName("parentHrmsId")]
    public string? ParentHrmsId { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("active")]
    public bool? Active { get; set; }

    [JsonPropertyName("virtual")]
    public bool? IsVirtual { get; set; }

    public List<string>? ChangedFields { get; set; }
}