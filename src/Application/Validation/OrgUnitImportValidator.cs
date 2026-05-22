using IntegrationImport.Application.Constants;
using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Interfaces.Validation;

namespace IntegrationImport.Application.Validation;

public class OrgUnitImportValidator : IOrgUnitImportValidator
{
    private const int MaxOrgUnitsPerRequest = 100;

    public ValidationResult Validate(OrgUnitImportRequestDto request)
    {
        ValidationResult result = new ValidationResult();

        if (request is null)
        {
            result.AddGlobal("Request is null.");
            return result;
        }

        result.AddGlobal(ImportRequestGenericValidation.CheckReferenceId(request.ReferenceId));
        result.AddGlobal(ImportRequestGenericValidation.CheckCollectionNotEmpty(request.OrganizationUnits, nameof(request.OrganizationUnits)));
        result.AddGlobal(ImportRequestGenericValidation.CheckCollectionMaxCount(request.OrganizationUnits, MaxOrgUnitsPerRequest, nameof(request.OrganizationUnits)));

        if (request.OrganizationUnits is not null)
        {
            for (var i = 0; i < request.OrganizationUnits.Count; i++)
            {
                ValidateOrgUnit(request.OrganizationUnits[i], i, result);
            }
        }

        return result;
    }

    private static void ValidateOrgUnit(OrgUnitItemDto ou, int index, ValidationResult result)
    {
        if (ou is null)
        {
            result.AddItem(index, "Item is null.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ou.HrmsId))
            result.AddItem(index, "HrmsId is required.");

        var actionRecognized = false;
        switch (ou.Action?.Trim().ToUpperInvariant())
        {
            case OrgUnitAction.Create:
                ValidateCreate(ou, index, result);
                actionRecognized = true;
                break;
            case OrgUnitAction.Update:
                ValidateUpdate(ou, index, result);
                actionRecognized = true;
                break;
            case OrgUnitAction.Deactivate:
                ValidateDeactivate(ou, index, result);
                actionRecognized = true;
                break;
        }

        if (!actionRecognized)
            result.AddItem(index, "Action must be one of CREATE/UPDATE/DEACTIVATE.");

        if (!string.IsNullOrWhiteSpace(ou.Email) && !IsValidEmail(ou.Email))
            result.AddItem(index, "Email is invalid.");
    }

    private static void ValidateCreate(OrgUnitItemDto ou, int index, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(ou.Name))
            result.AddItem(index, "Name is required for CREATE.");
    }

    private static void ValidateUpdate(OrgUnitItemDto ou, int index, ValidationResult result)
    {
        if (ou.ChangedFields is not null && ou.ChangedFields.Count == 0)
            result.AddItem(index, "ChangedFields cannot be empty when provided.");
    }

    private static void ValidateDeactivate(OrgUnitItemDto ou, int index, ValidationResult result)
    {
        if (ou.Active.HasValue && ou.Active.Value)
            result.AddItem(index, "Active cannot be true for DEACTIVATE.");
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var _ = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
