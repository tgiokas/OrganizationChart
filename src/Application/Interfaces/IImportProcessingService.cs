using IntegrationImport.Application.Dtos;

namespace IntegrationImport.Application.Interfaces;

public interface IImportProcessingService
{
    Task ProcessApprovedItemAsync(ImportΙtemApprovedMessageDto message, CancellationToken ct);
}

