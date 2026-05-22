using IntegrationImport.Application.Constants;
using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Errors;
using IntegrationImport.Application.Helpers;
using IntegrationImport.Domain.Entities;
using IntegrationImport.Domain.Interfaces;

namespace IntegrationImport.Application.Helpers;

public class UserOrgUnitSynchronizer : IUserOrgUnitSynchronizer
{
    private readonly IImportedOrganizationUnitRepository _orgUnitRepository;
    private readonly IImportedUserOrganizationUnitRepository _userOrgUnitRepository;

    public UserOrgUnitSynchronizer(
        IImportedOrganizationUnitRepository orgUnitRepository,
        IImportedUserOrganizationUnitRepository userOrgUnitRepository)
    {
        _orgUnitRepository = orgUnitRepository;
        _userOrgUnitRepository = userOrgUnitRepository;
    }

    public async Task<Result<List<ImportedOrganizationUnit>>> ResolveAsync(
        UserImportItem item, CancellationToken ct)
    {
        var ids = FieldsParser.ParseOrgUnitHrmsIds(item.OrgUnitHrmsIdsJson);
        if (ids.Count == 0)
            return Result<List<ImportedOrganizationUnit>>.Ok(new List<ImportedOrganizationUnit>());

        var existing = await _orgUnitRepository.GetByExternalIdsAsync(ids, ct);
        var existingIds = existing.Select(x => x.ExternalId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = ids.Where(x => !existingIds.Contains(x)).ToList();

        if (missing.Count > 0)
            return Result<List<ImportedOrganizationUnit>>.Fail(
                errorCode: ErrorCodes.IMPORT.OrgUnitHrmsIdNotFound,
                message:   $"The following OrgUnitHrmsIds do not exist in the system: {string.Join(", ", missing)}");

        return Result<List<ImportedOrganizationUnit>>.Ok(existing);
    }

    public async Task<Result<bool>> SyncAsync(
        UserImportItem item,
        ImportedUser importedUser,
        IReadOnlyCollection<string> changedFields,
        CancellationToken ct)
    {
        // Gate: only touch org units if HRMS explicitly says they changed.
        if (!changedFields.Contains(UserChangedField.OrgUnitHrmsIds, StringComparer.OrdinalIgnoreCase))
            return Result<bool>.Ok(true);

        // Resolve desired set + validate all org units exist.
        var desiredResult = await ResolveAsync(item, ct);
        if (!desiredResult.Success)
            return Result<bool>.Fail(
                errorCode: desiredResult.ErrorCode ?? ErrorCodes.IMPORT.OrgUnitHrmsIdNotFound,
                message:   desiredResult.Message   ?? "OrgUnit resolution failed.");

        // Delegate the diff/upsert to the repo.
        await _userOrgUnitRepository.UpsertRelationsAsync(
            importedUser,
            desiredResult.Data,
            item.ImportRequestId,
            item.Id,
            ct);

        return Result<bool>.Ok(true);
    }
}
