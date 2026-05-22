using System.Text.Json.Serialization;

namespace IntegrationImport.Application.Dtos;

public class UserItemDto
{
    [JsonPropertyName("hrmsId")]
    public required string HrmsId { get; set; }

    [JsonPropertyName("action")]
    public required string Action { get; set; }   // "CREATE" | "UPDATE" | "DEACTIVATE"

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    [JsonPropertyName("vatId")]
    public string? VatId { get; set; }

    [JsonPropertyName("changedFields")]
    public List<string>? ChangedFields { get; set; }

    [JsonPropertyName("orgUnitHrmsIds")]
    public List<string>? OrgUnitHrmsIds { get; set; }
}
