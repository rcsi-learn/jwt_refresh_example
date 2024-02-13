namespace ConsoleClient.Model;
public class AutorizationResponse {
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}
