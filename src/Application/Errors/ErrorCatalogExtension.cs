using IntegrationImport.Application.Dtos;
using IntegrationImport.Application.Interfaces;

namespace IntegrationImport.Application.Errors;

public static class ErrorCatalogExtensions
{
    public static Result<T> Fail<T>(this IErrorCatalog errors, string code)
    {
        var e = errors.GetError(code);
        return Result<T>.Fail(errorCode: e.Code, message: e.Message);
    }

    /// <summary>
    /// Same as <see cref="Fail{T}(IErrorCatalog, string)"/> but appends an additional
    /// human-readable detail to the catalog's canonical message — e.g. when a
    /// validator returns a list of specific problems.
    /// </summary>
    public static Result<T> Fail<T>(this IErrorCatalog errors, string code, string detail)
    {
        var e = errors.GetError(code);
        var combinedMessage = string.IsNullOrWhiteSpace(detail)
            ? e.Message
            : $"{e.Message} — {detail}";
        return Result<T>.Fail(errorCode: e.Code, message: combinedMessage);
    }
}
