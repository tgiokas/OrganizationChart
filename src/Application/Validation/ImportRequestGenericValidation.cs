namespace IntegrationImport.Application.Validation;

public static class ImportRequestGenericValidation
{
    // ===== Legacy throwing variants =====
    // Kept for backwards compatibility while the OrgUnit validator still
    // uses them. Will be removed after the OrgUnit validator migrates to
    // accumulating-error semantics.

    public static void ValidateReferenceId(string? referenceId)
    {
        if (string.IsNullOrWhiteSpace(referenceId))
            throw new ArgumentException("ReferenceId is required.", nameof(referenceId));

        if (referenceId.Length > 100)
            throw new ArgumentException("ReferenceId cannot exceed 100 characters.", nameof(referenceId));
    }

    public static void ValidateCollectionNotEmpty<T>(ICollection<T>? items, string fieldName)
    {
        if (items is null || items.Count == 0)
            throw new ArgumentException($"{fieldName} must contain at least one item.", fieldName);
    }

    public static void ValidateCollectionMaxCount<T>(ICollection<T>? items, int maxCount, string fieldName)
    {
        if (items is not null && items.Count > maxCount)
            throw new ArgumentException($"{fieldName} cannot contain more than {maxCount} items.", fieldName);
    }

    // ===== Non-throwing variants =====
    // Return null on success or a human-readable error message on failure.

    public static string? CheckReferenceId(string? referenceId)
    {
        if (string.IsNullOrWhiteSpace(referenceId))
            return "ReferenceId is required.";

        if (referenceId.Length > 100)
            return "ReferenceId cannot exceed 100 characters.";

        return null;
    }

    public static string? CheckCollectionNotEmpty<T>(ICollection<T>? items, string fieldName)
    {
        if (items is null || items.Count == 0)
            return $"{fieldName} must contain at least one item.";

        return null;
    }

    public static string? CheckCollectionMaxCount<T>(ICollection<T>? items, int maxCount, string fieldName)
    {
        if (items is not null && items.Count > maxCount)
            return $"{fieldName} cannot contain more than {maxCount} items.";

        return null;
    }
}
