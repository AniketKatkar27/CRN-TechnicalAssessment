namespace CRN.Application.DTOs;

public class ErrorResponse
{
    public int StatusCode { get; set; }

    public string Message { get; set; } = string.Empty;

    public object? Errors { get; set; }
}