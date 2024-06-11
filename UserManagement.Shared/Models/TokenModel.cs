namespace UserManagement.Shared.Models;

public class TokenModel
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public bool? IsAuthenticated { get; set; }
}
