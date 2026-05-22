using System.Text.Json.Serialization;

namespace IntegrationImport.Application.Dtos;

public class OrgUnitImportRequestDto
{

    [JsonPropertyName("referenceId")]
    public required string ReferenceId { get; set; }

    [JsonPropertyName("effectiveDate")]
    public required DateOnly EffectiveDate { get; set; }

    [JsonPropertyName("organizationUnits")]
    public required List<OrgUnitItemDto> OrganizationUnits { get; set; } = [];
}
