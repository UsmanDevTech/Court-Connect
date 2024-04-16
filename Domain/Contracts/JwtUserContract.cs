namespace Domain.Contracts;

public class JwtUserContract
{
    public string id { get; set; } = null!;
    public int loginRole { get; set; }
    public string? userName { get; set; }
    public string? email { get; set; }
}
