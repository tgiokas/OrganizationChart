using IntegrationImport.Application.Constants;
using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Helpers;
using IntegrationImport.Application.Interfaces;
using IntegrationImport.Domain.Entities;
using IntegrationImport.Domain.Enums;
using IntegrationImport.Domain.Interfaces;

namespace IntegrationImport.Application.Services;

public class ImportProcessingService : IImportProcessingService
{
    private readonly IImportRequestRepository _importRepository;
    private readonly IImportedUserRepository _importedUserRepository;
    private readonly IUserImportItemRepository _userImportItemRepository;
    private readonly IOrganizationUnitImportItemRepository _organizationUnitImportItemRepository;
    private readonly IAuthApiClient _authenticationClient;
    private readonly IDbTransactionScope _transactionScope;
    private readonly IImportedOrganizationUnitRepository _importedOrganizationUnitRepository;
    private readonly IUserOrgUnitSynchronizer _userOrgUnitSynchronizer;
    //private readonly IBatchStatusRecalculationService _batchStatusRecalculationService;



    public ImportProcessingService(
        IImportRequestRepository importRepository,
        IImportedUserRepository importedUserRepository,
        IUserImportItemRepository userImportItemRepository,
        IOrganizationUnitImportItemRepository organizationUnitImportItemRepository,
        IAuthApiClient authenticationClient,
        IDbTransactionScope transactionScope,
        IImportedOrganizationUnitRepository importedOrganizationUnitRepository,
        IUserOrgUnitSynchronizer userOrgUnitSynchronizer)
    // IBatchStatusRecalculationService batchStatusRecalculationService)
    {
        _importRepository = importRepository;
        _userImportItemRepository = userImportItemRepository;
        _importedUserRepository = importedUserRepository;
        _organizationUnitImportItemRepository = organizationUnitImportItemRepository;
        _importedOrganizationUnitRepository = importedOrganizationUnitRepository;
        _authenticationClient = authenticationClient;
        _transactionScope = transactionScope;
        _userOrgUnitSynchronizer = userOrgUnitSynchronizer;
        // _batchStatusRecalculationService = batchStatusRecalculationService;
    }

    public async Task ProcessApprovedItemAsync(ImportΙtemApprovedMessageDto message, CancellationToken ct)
    {
        //IF MORE ENTITES ADDED IN FUTURE, THIS CAN BE CHANGED TO REFLECT A MORE SCALABLE APPROACH (E.G. USING A HANDLER/STRATEGY PATTERN OR SIMILAR)
        switch (message.EntityType.ToString().ToUpperInvariant())
        {
            case "USER":
                await ProcessUserAsync(message.ItemId, ct);
                break;

            case "ORGANIZATION_UNIT":
                await ProcessOrgUnitAsync(message.ItemId, ct);
                break;

            default:
                return;
        }

        // await _batchStatusRecalculationService.RecalculateAsync(item.ImportRequestId, ct);
    }


    private async Task ProcessUserAsync(Guid userId, CancellationToken ct)
    {

        var item = await _userImportItemRepository.GetTrackedUserItemByIdAsync(userId, ct);

        if (item is null)
            return;

        if (item.ReviewStatus != ImportItemReviewStatus.APPROVED)
            return;

        if (item.ProcessStatus == ImportItemProcessStatus.APPLIED)
            return;

        if (item.ProcessStatus != ImportItemProcessStatus.DISPATCHED)
            return;

        try
        {
            switch (item.Action?.Trim().ToUpperInvariant())
            {
                case UserAction.Create:
                    await ProcessCreateAsync(item, ct);
                    break;
                case UserAction.Update:
                    await ProcessUpdateAsync(item, ct);
                    break;
                case UserAction.Deactivate:
                    await ProcessDeactivateUserAsync(item, ct);
                    break;

                default:
                    item.ProcessStatus = ImportItemProcessStatus.FAILED;
                    item.ApplyError = $"Unsupported action '{item.Action}' for Authentication processing.";
                    await _transactionScope.SaveChangesAsync(ct);
                    break;
            }
        }
        catch (Exception ex)
        {
            item.ProcessStatus = ImportItemProcessStatus.FAILED;
            item.ApplyError = ex.Message;
            await _transactionScope.SaveChangesAsync(ct);
        }
    }
    private async Task ProcessOrgUnitAsync(Guid orgUnitId, CancellationToken ct)
    {
        var item = await _organizationUnitImportItemRepository.GetTrackedOrgUnitItemByIdAsync(orgUnitId, ct);

        if (item is null) return;
        if (item.ReviewStatus != ImportItemReviewStatus.APPROVED) return;
        if (item.ProcessStatus == ImportItemProcessStatus.APPLIED) return;
        if (item.ProcessStatus != ImportItemProcessStatus.DISPATCHED) return;

        try
        {
            switch (item.Action.Trim().ToUpperInvariant())
            {
                case OrgUnitAction.Create:
                    await ProcessOrgUnitCreateAsync(item, ct);
                    break;

                case OrgUnitAction.Update:
                    await ProcessOrgUnitUpdateAsync(item, ct);
                    break;

                case OrgUnitAction.Deactivate:
                    await ProcessOrgUnitDeactivateAsync(item, ct);
                    break;

                default:
                    item.ProcessStatus = ImportItemProcessStatus.FAILED;
                    item.ApplyError    = $"Unsupported action '{item.Action}' for ORGANIZATION_UNIT processing.";
                    await _transactionScope.SaveChangesAsync(ct);
                    break;
            }
        }
        catch (Exception ex)
        {
            item.ProcessStatus = ImportItemProcessStatus.FAILED;
            item.ApplyError    = ex.Message;
            await _transactionScope.SaveChangesAsync(ct);
        }
    }

    private async Task ProcessOrgUnitCreateAsync(OrganizationUnitImportItem item, CancellationToken ct)
    {
        // Entry guard — idempotency by SourceImportItemId
        var byItem = await _importedOrganizationUnitRepository.GetBySourceImportItemIdAsync(item.Id, ct);
        if (byItem is not null && byItem.LastSyncStatus == "APPLIED")
        {
            item.ProcessStatus = ImportItemProcessStatus.APPLIED;
            item.AppliedAtUtc ??= byItem.LastSyncedAtUtc ?? DateTime.UtcNow;
            item.ApplyError    = null;
            await _transactionScope.SaveChangesAsync(ct);
            return;
        }

        // Defensive — HrmsId already imported by some other path
        var byHrms = await _importedOrganizationUnitRepository.GetByExternalIdAsync(item.HrmsId, ct);
        if (byHrms is not null)
        {
            item.ProcessStatus = ImportItemProcessStatus.FAILED;
            item.ApplyError    = $"OrganizationUnit with HrmsId '{item.HrmsId}' already exists.";
            await _transactionScope.SaveChangesAsync(ct);
            return;
        }

        // Resolve parent FK if specified
        Guid? parentImportedId = null;
        if (!string.IsNullOrWhiteSpace(item.ParentHrmsId))
        {
            var parent = await _importedOrganizationUnitRepository
                .GetByExternalIdAsync(item.ParentHrmsId, ct);
            if (parent is null)
            {
                item.ProcessStatus = ImportItemProcessStatus.FAILED;
                item.ApplyError    = $"Parent OrganizationUnit '{item.ParentHrmsId}' not found.";
                await _transactionScope.SaveChangesAsync(ct);
                return;
            }
            parentImportedId = parent.Id;
        }

        await _transactionScope.BeginTransactionAsync(ct);
        try
        {
            var entity = new ImportedOrganizationUnit
            {
                Id                               = Guid.NewGuid(),
                SourceImportItemId               = item.Id,
                ExternalId                       = item.HrmsId,
                Name                             = item.Name,
                Abbreviation                     = item.Abbreviation,
                Email                            = item.Email,
                Location                         = item.Location,
                IsVirtual                        = item.IsVirtual ?? false,
                ParentExternalId                 = item.ParentHrmsId,
                ParentImportedOrganizationUnitId = parentImportedId,
                IsActive                         = item.IsActive ?? true,
                CreatedAtUtc                     = DateTime.UtcNow,
                LastSyncedAtUtc                  = DateTime.UtcNow,
                LastImportRequestId              = item.ImportRequestId,
                LastImportItemId                 = item.Id,
                LastAction                       = item.Action,
                LastSyncStatus                   = "APPLIED",
            };
            await _importedOrganizationUnitRepository.AddAsync(entity, ct);

            item.ProcessStatus = ImportItemProcessStatus.APPLIED;
            item.AppliedAtUtc  = DateTime.UtcNow;
            item.ApplyError    = null;

            await _transactionScope.CommitTransactionAsync(ct);
        }
        catch
        {
            await _transactionScope.RollbackTransactionAsync(ct);
            throw;
        }
    }

    private async Task ProcessOrgUnitUpdateAsync(OrganizationUnitImportItem item, CancellationToken ct)
    {
        var existing = await _importedOrganizationUnitRepository.GetByExternalIdAsync(item.HrmsId, ct);
        if (existing is null)
        {
            item.ProcessStatus = ImportItemProcessStatus.FAILED;
            item.ApplyError    = $"OrganizationUnit '{item.HrmsId}' not found.";
            await _transactionScope.SaveChangesAsync(ct);
            return;
        }

        // Coalesce semantics — HRMS spec for OrgUnit doesn't include ChangedFields.
        // If BA adds it later, swap to the Changed(...) gated pattern (same as user UPDATE).
        existing.Name         = item.Name         ?? existing.Name;
        existing.Abbreviation = item.Abbreviation ?? existing.Abbreviation;
        existing.Email        = item.Email        ?? existing.Email;
        existing.Location     = item.Location     ?? existing.Location;
        existing.IsActive     = item.IsActive     ?? existing.IsActive;
        existing.IsVirtual    = item.IsVirtual    ?? existing.IsVirtual;

        // Parent change — handled separately because of FK lookup
        if (!string.IsNullOrWhiteSpace(item.ParentHrmsId)
            && item.ParentHrmsId != existing.ParentExternalId)
        {
            var parent = await _importedOrganizationUnitRepository
                .GetByExternalIdAsync(item.ParentHrmsId, ct);
            if (parent is null)
            {
                item.ProcessStatus = ImportItemProcessStatus.FAILED;
                item.ApplyError    = $"Parent OrganizationUnit '{item.ParentHrmsId}' not found.";
                await _transactionScope.SaveChangesAsync(ct);
                return;
            }
            existing.ParentExternalId                  = item.ParentHrmsId;
            existing.ParentImportedOrganizationUnitId  = parent.Id;
        }

        existing.LastSyncedAtUtc     = DateTime.UtcNow;
        existing.LastImportRequestId = item.ImportRequestId;
        existing.LastImportItemId    = item.Id;
        existing.LastAction          = item.Action;
        existing.LastSyncStatus      = "APPLIED";

        item.ProcessStatus = ImportItemProcessStatus.APPLIED;
        item.AppliedAtUtc  = DateTime.UtcNow;
        item.ApplyError    = null;

        await _transactionScope.SaveChangesAsync(ct);
    }

    private async Task ProcessOrgUnitDeactivateAsync(OrganizationUnitImportItem item, CancellationToken ct)
    {
        var existing = await _importedOrganizationUnitRepository.GetByExternalIdAsync(item.HrmsId, ct);
        if (existing is null)
        {
            item.ProcessStatus = ImportItemProcessStatus.FAILED;
            item.ApplyError    = $"OrganizationUnit '{item.HrmsId}' not found.";
            await _transactionScope.SaveChangesAsync(ct);
            return;
        }

        existing.IsActive            = false;
        existing.LastSyncedAtUtc     = DateTime.UtcNow;
        existing.LastImportRequestId = item.ImportRequestId;
        existing.LastImportItemId    = item.Id;
        existing.LastAction          = item.Action;
        existing.LastSyncStatus      = "APPLIED";

        item.ProcessStatus = ImportItemProcessStatus.APPLIED;
        item.AppliedAtUtc  = DateTime.UtcNow;
        item.ApplyError    = null;

        await _transactionScope.SaveChangesAsync(ct);
    }

    private async Task ProcessCreateAsync(UserImportItem item, CancellationToken ct)
    {
        // ---------- Entry guard ----------
        var reservation = await _importedUserRepository
            .GetBySourceImportItemIdAsync(item.Id, ct);

        if (reservation is not null
            && reservation.LastSyncStatus == "APPLIED"
            && reservation.KeycloakUserId is not null)
        {
            item.ProcessStatus = ImportItemProcessStatus.APPLIED;
            item.AppliedAtUtc ??= reservation.LastSyncedAtUtc ?? DateTime.UtcNow;
            item.ApplyError = null;
            await _transactionScope.SaveChangesAsync(ct);
            return;
        }

        // Defensive: a different ImportedUser already maps this HrmsId,
        // and it didn't come from this item.
        if (reservation is null)
        {
            var byHrms = await _importedUserRepository.GetByExternalIdAsync(item.HrmsId, ct);
            if (byHrms is not null)
            {
                item.ProcessStatus = ImportItemProcessStatus.FAILED;
                item.ApplyError = "Authentication create user failed.";
                item.ReviewNotes = $"ImportedUser already exists for HrmsId '{item.HrmsId}'.";
                await _transactionScope.SaveChangesAsync(ct);
                return;
            }
        }


        var orgUnitsResult = await _userOrgUnitSynchronizer.ResolveAsync(item, ct);

        if (!orgUnitsResult.Success)
        {
            item.ProcessStatus = ImportItemProcessStatus.FAILED;
            item.ApplyError = orgUnitsResult.Message;
            await _transactionScope.SaveChangesAsync(ct);
            return;
        }

        List<ImportedOrganizationUnit>? orgUnits = orgUnitsResult.Data;

        // ---------- Tx1: reservation in PENDING_AUTH ----------
        if (reservation is null)
        {
            await _transactionScope.BeginTransactionAsync(ct);
            try
            {
                reservation = new ImportedUser
                {
                    Id = Guid.NewGuid(),
                    SourceImportItemId = item.Id,
                    ExternalId = item.HrmsId,
                    KeycloakUserId = null,
                    Username = item.Username ?? string.Empty,
                    Email = item.Email,
                    PhoneNumber = item.PhoneNumber,
                    IsActive = item.Enabled ?? true,
                    CreatedAtUtc = DateTime.UtcNow,
                    LastImportRequestId = item.ImportRequestId,
                    LastImportItemId = item.Id,
                    LastAction = item.Action,
                    LastSyncStatus = "PENDING_AUTH",
                };
                await _importedUserRepository.AddAsync(reservation, ct);
                await _transactionScope.CommitTransactionAsync(ct);
            }
            catch
            {
                // Race: another consumer already reserved. Re-load and continue.
                await _transactionScope.RollbackTransactionAsync(ct);
                reservation = await _importedUserRepository
                    .GetBySourceImportItemIdAsync(item.Id, ct)
                    ?? throw new InvalidOperationException("Failed to reload reservation after race condition.");
            }
        }

        // Re-check after the reload — maybe the racing instance already finished.
        if (reservation.LastSyncStatus == "APPLIED" && reservation.KeycloakUserId is not null)
        {
            item.ProcessStatus = ImportItemProcessStatus.APPLIED;
            item.AppliedAtUtc ??= DateTime.UtcNow;
            await _transactionScope.SaveChangesAsync(ct);
            return;
        }



        // ---------- HTTP: call auth-api (no DB tx held) ----------

        AuthApiUserCreateDto request = new AuthApiUserCreateDto
        {
            User = new UserCreateDto
            {
                Username = item.Username ?? throw new InvalidOperationException("Username is required for CREATE."),

                Password = null,
                PasswordTemp = true,

                Email = item.Email ?? throw new InvalidOperationException("Email is required for CREATE."),

                FirstName = item.FirstName,
                LastName = item.LastName,
                PhoneNumber = item.PhoneNumber,

                Enabled = item.Enabled ?? true,
                IsAdmin = false,
                EmailVerified = true,
            },

            Role = new RoleDto
            {
                RoleName = "integration-api-testDefaultRole",
                Description = null
            }
        };

        // HTTP 1 : create user
        var result = await _authenticationClient.CreateUserWithRoleAsync(request, ct);

        Guid? keycloakUserId = null;

        if (result.Success && result.Data is not null)
        {
            keycloakUserId = result.Data.Id;
        }
        else if (IsUsernameAlreadyExistsError(result))
        {
            // ---------- Recovery: previous attempt of THIS item already created the user. ----------
            var lookup = await _authenticationClient
                .GetUserByUsernameAsync(request.User.Username, ct);

            if (lookup.Success && lookup.Data is not null)
            {
                keycloakUserId = lookup.Data.Id;
            }
        }

        if (keycloakUserId is null)
        {
            // failure or recovery couldn't resolve. Leave reservation in PENDING_AUTH
            // for an operator/reaper to investigate; mark item FAILED.
            item.ProcessStatus = ImportItemProcessStatus.FAILED;
            item.ApplyError = result.ErrorCode ?? "Authentication create user failed.";
            await _transactionScope.SaveChangesAsync(ct);
            return;
        }

       

        // ---------- Tx2: finalize ----------
        await _transactionScope.BeginTransactionAsync(ct);
        try
        {
            reservation.KeycloakUserId = keycloakUserId;
            reservation.LastSyncedAtUtc = DateTime.UtcNow;
            reservation.LastSyncStatus = "APPLIED";


            item.ProcessStatus = ImportItemProcessStatus.APPLIED;
            item.AppliedAtUtc = DateTime.UtcNow;
            item.ApplyError = null;

            if (orgUnits is not null && orgUnits.Count > 0)
            {
                foreach (var ou in orgUnits)
                {
                    reservation.OrganizationUnits.Add(new ImportedUserOrganizationUnit
                    {
                        Id = Guid.NewGuid(),
                        ImportedUserId = reservation.Id,
                        ImportedOrganizationUnitId = ou.Id,
                        UserExternalId = item.HrmsId,
                        OrgUnitExternalId = ou.ExternalId,
                        IsActive = true,
                        CreatedAtUtc = DateTime.UtcNow,
                        LastSyncedAtUtc = DateTime.UtcNow,
                        LastImportRequestId = item.ImportRequestId,
                        LastImportItemId = item.Id,
                        ExternalSyncStatus = "PENDING",
                    });
                }
            }

            await _transactionScope.CommitTransactionAsync(ct);
        }
        catch
        {
            await _transactionScope.RollbackTransactionAsync(ct);
            throw;
        }
    }
  
    private async Task ProcessUpdateAsync(UserImportItem user, CancellationToken ct)
    {
        var importedUser = await _importedUserRepository.GetByExternalIdWithOrgUnitsAsync(user.HrmsId, ct);

        if (importedUser is null)
        {
            user.ProcessStatus = ImportItemProcessStatus.FAILED;
            user.ApplyError = "Authentication update user failed.";
            user.ReviewNotes = $"ImportedUser mapping was not found for HrmsId '{user.HrmsId}'.";

            await _transactionScope.SaveChangesAsync(ct);
            return;
        }

        if (importedUser.KeycloakUserId is null)
        {
            user.ProcessStatus = ImportItemProcessStatus.FAILED;
            user.ApplyError = "ImportedUser mapping has no KeycloakUserId.";

            await _transactionScope.SaveChangesAsync(ct);
            return;
        }


        List<string> changedFields = FieldsParser.ParseChangedFields(user.ChangedFieldsJson);
        bool Changed(string field) => changedFields.Contains(field, StringComparer.OrdinalIgnoreCase);

        var updateRequest = new AuthApiUserUpdateDto
        {
            Id = importedUser.KeycloakUserId.ToString(),
            Username = Changed(UserChangedField.Username) ? user.Username : importedUser.Username,
            Email = Changed(UserChangedField.Email)
                            ? user.Email ?? throw new InvalidOperationException("Email is required for UPDATE.")
                            : importedUser.Email ?? throw new InvalidOperationException("Email is required for UPDATE."),
            FirstName = Changed(UserChangedField.FirstName) ? user.FirstName : importedUser.FirstName,
            LastName = Changed(UserChangedField.LastName) ? user.LastName : importedUser.LastName,
            PhoneNumber = Changed(UserChangedField.PhoneNumber) ? user.PhoneNumber : importedUser.PhoneNumber,
            Enabled = Changed(UserChangedField.Enabled) ? (user.Enabled ?? false) : importedUser.IsActive,
            IsAdmin = false,
            EmailVerified = false,
        };

        var result = await _authenticationClient.UpdateUserAsync(updateRequest, ct);
        if (!result.Success || result.Data is null)
        {
            user.ProcessStatus = ImportItemProcessStatus.FAILED;
            user.ApplyError = result.ErrorCode ?? "Authentication update user failed.";
            await _transactionScope.SaveChangesAsync(ct);
            return;
        }


        await _transactionScope.BeginTransactionAsync(ct);
        try
        {
            user.ProcessStatus = ImportItemProcessStatus.APPLIED;
            user.AppliedAtUtc = DateTime.UtcNow;
            user.ApplyError = null;

            UpdateImportedUser(user, importedUser, changedFields);

            var ouResult = await _userOrgUnitSynchronizer.SyncAsync(user, importedUser, changedFields, ct);
            if (!ouResult.Success)
            {
                await _transactionScope.RollbackTransactionAsync(ct);
                user.ProcessStatus = ImportItemProcessStatus.FAILED;
                user.ApplyError = ouResult.Message;
                await _transactionScope.SaveChangesAsync(ct);
                return;
            }

            await _transactionScope.CommitTransactionAsync(ct);
        }
        catch
        {
            await _transactionScope.RollbackTransactionAsync(ct);
            throw;
        }

    }
    private async Task ProcessDeactivateUserAsync(UserImportItem user, CancellationToken ct)
    {
        var importedUser = await _importedUserRepository.GetByExternalIdAsync(user.HrmsId, ct);

        if (importedUser is null)
        {
            user.ProcessStatus = ImportItemProcessStatus.FAILED;
            user.ApplyError = "Authentication update user failed.";
            user.ReviewNotes = $"ImportedUser mapping was not found for HrmsId '{user.HrmsId}'.";

            await _transactionScope.SaveChangesAsync(ct);
            return;
        }

        if (importedUser.KeycloakUserId is null)
        {
            user.ProcessStatus = ImportItemProcessStatus.FAILED;
            user.ApplyError = "ImportedUser mapping has no KeycloakUserId.";

            await _transactionScope.SaveChangesAsync(ct);
            return;
        }


        var updateRequest = new AuthApiUserUpdateDto
        {
            Id = importedUser.KeycloakUserId.ToString(),
            Username = user.Username ?? importedUser.Username,
            Email = user.Email ?? importedUser.Email ?? throw new InvalidOperationException("Email is required for UPDATE."),
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Enabled = false,  //DEACTIVATE USER
            IsAdmin = false,
            EmailVerified = false
        };

        var result = await _authenticationClient.UpdateUserAsync(updateRequest, ct);

        if (!result.Success || result.Data is null)
        {
            user.ProcessStatus = ImportItemProcessStatus.FAILED;
            user.ApplyError = result.ErrorCode ?? "Authentication Deactivation user failed.";
            await _transactionScope.SaveChangesAsync(ct);
            return;
        }

        await _transactionScope.BeginTransactionAsync(ct);
        {
            try
            {
                user.ProcessStatus = ImportItemProcessStatus.APPLIED;
                user.AppliedAtUtc = DateTime.UtcNow;
                user.ApplyError = null;

                DeactivateImportedUser(user, importedUser, ct);

                await _transactionScope.CommitTransactionAsync(ct);
            }
            catch
            {
                await _transactionScope.RollbackTransactionAsync(ct);
                throw;
            }
        }

    }

    private void UpdateImportedUser(UserImportItem item, ImportedUser importedUser, IReadOnlyCollection<string> changedFields)
    {
        bool Changed(string f) => changedFields.Contains(f, StringComparer.OrdinalIgnoreCase);

        if (Changed(UserChangedField.Username)) importedUser.Username = item.Username ?? string.Empty;
        if (Changed(UserChangedField.Email)) importedUser.Email = item.Email;
        if (Changed(UserChangedField.FirstName)) importedUser.FirstName = item.FirstName;
        if (Changed(UserChangedField.LastName)) importedUser.LastName = item.LastName;
        if (Changed(UserChangedField.PhoneNumber)) importedUser.PhoneNumber = item.PhoneNumber;
        if (Changed(UserChangedField.Enabled)) importedUser.IsActive = item.Enabled ?? importedUser.IsActive;

        importedUser.LastSyncedAtUtc = DateTime.UtcNow;
        importedUser.LastImportRequestId = item.ImportRequestId;
        importedUser.LastImportItemId = item.Id;
        importedUser.LastAction = item.Action;
        importedUser.LastSyncStatus = "APPLIED";
    }

    private void DeactivateImportedUser(UserImportItem item, ImportedUser importedUser, CancellationToken ct)
    {
        importedUser.Username = item.Username ?? importedUser.Username;
        importedUser.Email = item.Email ?? importedUser.Email;
        importedUser.PhoneNumber = item.PhoneNumber;
        importedUser.IsActive = false;
        importedUser.LastSyncedAtUtc = DateTime.UtcNow;
        importedUser.LastImportRequestId = item.ImportRequestId;
        importedUser.LastImportItemId = item.Id;
        importedUser.LastAction = item.Action;
        importedUser.LastSyncStatus = "APPLIED";
    }
    private static bool IsUsernameAlreadyExistsError(Result<AuthApiUserProfileDto> result)
=> string.Equals(result.ErrorCode, "AUTH-006", StringComparison.OrdinalIgnoreCase);

}

