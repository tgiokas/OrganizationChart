namespace IntegrationImport.Application.Validation;

/// <summary>
/// Result of running an import-payload validator. Carries the full list of
/// problems found, separated into:
///   - <see cref="GlobalErrors"/> — batch-level issues (e.g. missing referenceId,
///     too many items) that doom the whole request.
///   - <see cref="ItemErrors"/> — per-item issues, keyed by the index of the
///     offending user/orgunit inside the original payload. Used by partial-success
///     ingestion to filter out only the invalid items while accepting the rest.
/// </summary>
public sealed class ValidationResult
{
    public List<string> GlobalErrors { get; } = new();
    public Dictionary<int, List<string>> ItemErrors { get; } = new();

    /// <summary>True when neither global nor per-item errors exist.</summary>
    public bool IsValid => GlobalErrors.Count == 0 && ItemErrors.Count == 0;

    public bool HasGlobalErrors => GlobalErrors.Count > 0;

    /// <summary>Indexes of items that have at least one error.</summary>
    public IReadOnlySet<int> InvalidIndexes =>
        ItemErrors.Keys.ToHashSet();

    /// <summary>Flat enumeration of every error (global + per-item) for logging.</summary>
    public IEnumerable<string> AllErrors =>
        GlobalErrors.Concat(ItemErrors.SelectMany(
            kv => kv.Value.Select(e => $"[{kv.Key}] {e}")));

    public int TotalErrorCount =>
        GlobalErrors.Count + ItemErrors.Values.Sum(v => v.Count);

    public void AddGlobal(string? error)
    {
        if (!string.IsNullOrWhiteSpace(error))
            GlobalErrors.Add(error);
    }

    public void AddItem(int index, string? error)
    {
        if (string.IsNullOrWhiteSpace(error)) return;

        if (!ItemErrors.TryGetValue(index, out var list))
        {
            list = new List<string>();
            ItemErrors[index] = list;
        }
        list.Add(error);
    }

    /// <summary>
    /// Backwards-compat: adds a free-form error to <see cref="GlobalErrors"/>.
    /// Used by older validators that haven't migrated to the per-item API.
    /// </summary>
    public void Add(string? error) => AddGlobal(error);

    public static ValidationResult Valid() => new();

    /// <summary>
    /// Joins all errors (global + per-item) with "; " for one-line display.
    /// </summary>
    public string ToMessage() => string.Join("; ", AllErrors);
}
