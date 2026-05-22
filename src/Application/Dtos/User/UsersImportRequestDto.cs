namespace IntegrationImport.Application.Dtos;

public class UsersImportRequestDto
{
    public required string ReferenceId { get; set; }
    public required DateOnly EffectiveDate { get; set; }
    public required List<UserItemDto> Users { get; set; } = [];
}
