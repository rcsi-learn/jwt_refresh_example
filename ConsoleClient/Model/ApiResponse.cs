namespace ConsoleClient.Model;
public class ApiResponse {
    public object? Value { get; set; }
    public object[]? Formatters { get; set; }
    public object[]? ContentTypes { get; set; }
    public object? DeclaredType { get; set; }
    public int? StatusCode { get; set; }
}
