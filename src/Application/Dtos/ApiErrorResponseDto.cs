namespace IntegrationImport.Application.Dtos;

public class ApiErrorResponseDto
{
    public required string Code { get; set; }
    public required string Message { get; set; }
    public string? Details { get; set; }
}
