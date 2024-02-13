namespace Domain;

public class RefreshTokenRequest
{
    public string? UserAccount { get; set; }
    public string? ExpiredToken { get; set; }
    public string? RefreshToken { get; set; }

}
