namespace ExternalIntegrations.OrganizationChart.Application.Dtos;

public class Result<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }

    public static Result<T> Ok(T? data = default, string? message = null)
    {
        return new Result<T> { Success = true, Data = data, Message = message };
    }

    public static Result<T> Fail(string errorCode, string message)
    {
        return new Result<T> { Success = false, ErrorCode = errorCode, Message = message };
    }
}