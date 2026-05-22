using IntegrationImport.Application.Constants;
using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Interfaces.Validation;

namespace IntegrationImport.Application.Validation;

public class UserImportValidator : IUserImportValidator
{
    private const int MaxUsersPerRequest = 100;

    public ValidationResult Validate(UsersImportRequestDto request)
    {
        ValidationResult result = new ValidationResult();

        if (request is null)
        {
            result.AddGlobal("Request is null.");
            return result;
        }

        result.AddGlobal(ImportRequestGenericValidation.CheckReferenceId(request.ReferenceId));
        result.AddGlobal(ImportRequestGenericValidation.CheckCollectionNotEmpty(request.Users, nameof(request.Users)));
        result.AddGlobal(ImportRequestGenericValidation.CheckCollectionMaxCount(request.Users, MaxUsersPerRequest, nameof(request.Users)));

        if (request.Users is not null)
        {
            for (var i = 0; i < request.Users.Count; i++)
            {
                ValidateUser(request.Users[i], i, result);
            }
        }

        return result;
    }

    private static void ValidateUser(UserItemDto user, int index, ValidationResult result)
    {
        if (user is null)
        {
            result.AddItem(index, "Item is null.");
            return;
        }

        if (string.IsNullOrWhiteSpace(user.HrmsId))
            result.AddItem(index, "HrmsId is required.");

        var actionRecognized = false;
        switch (user.Action?.Trim().ToUpperInvariant())
        {
            case UserAction.Create:
                ValidateCreate(user, index, result);
                actionRecognized = true;
                break;
            case UserAction.Update:
                ValidateUpdate(user, index, result);
                actionRecognized = true;
                break;
            case UserAction.Deactivate:
                ValidateDeactivate(user, index, result);
                actionRecognized = true;
                break;
        }

        if (!actionRecognized)
            result.AddItem(index, "Action must be one of CREATE/UPDATE/DEACTIVATE.");

        if (!string.IsNullOrWhiteSpace(user.Email) && !IsValidEmail(user.Email))
            result.AddItem(index, "Email is invalid.");

        if (user.OrgUnitHrmsIds is not null)
        {
            if (user.OrgUnitHrmsIds.Any(string.IsNullOrWhiteSpace))
                result.AddItem(index, "OrgUnitHrmsIds contains empty value.");

            var duplicates = user.OrgUnitHrmsIds
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .GroupBy(x => x.Trim(), StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count > 0)
                result.AddItem(index, $"OrgUnitHrmsIds contains duplicate values: {string.Join(", ", duplicates)}.");
        }
    }

    private static void ValidateCreate(UserItemDto user, int index, ValidationResult result)
    {
        if (string.IsNullOrWhiteSpace(user.Username))
            result.AddItem(index, "Username is required for CREATE.");

        if (string.IsNullOrWhiteSpace(user.Email))
            result.AddItem(index, "Email is required for CREATE.");

        if (string.IsNullOrWhiteSpace(user.FirstName))
            result.AddItem(index, "FirstName is required for CREATE.");

        if (string.IsNullOrWhiteSpace(user.LastName))
            result.AddItem(index, "LastName is required for CREATE.");
    }

    private static void ValidateUpdate(UserItemDto user, int index, ValidationResult result)
    {
        if (user.ChangedFields is not null && user.ChangedFields.Count == 0)
            result.AddItem(index, "ChangedFields cannot be empty when provided.");
    }

    private static void ValidateDeactivate(UserItemDto user, int index, ValidationResult result)
    {
        if (user.Enabled.HasValue && user.Enabled.Value)
            result.AddItem(index, "Enabled cannot be true for DEACTIVATE.");
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var mail = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
