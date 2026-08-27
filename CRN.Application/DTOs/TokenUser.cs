namespace CRN.Application.DTOs;

public class TokenUser
{
    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public IEnumerable<string> Roles { get; set; } = [];
}