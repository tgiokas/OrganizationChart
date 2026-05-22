using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Validation;

namespace IntegrationImport.Application.Interfaces.Validation;

public interface IUserImportValidator
{
    ValidationResult Validate(UsersImportRequestDto request);
}
