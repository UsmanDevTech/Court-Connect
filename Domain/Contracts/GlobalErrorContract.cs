namespace Domain.Contracts;

public sealed class GlobalErrorContract
{
    internal GlobalErrorContract(int statusCode,string code,string? message,string? uri, IDictionary<string, string[]>? errors)
    {
        this.status = statusCode;
        this.uri=uri;
        this.type = code;
        this.errors=errors;
        this.title = message;
    }
    public static GlobalErrorContract ReturnError(int statusCode,string code, string? message, string? uri, IDictionary<string, string[]>? errors)
    {
        return new GlobalErrorContract(statusCode, code,message,uri, errors);
    }
    public int status { get; set; }
    public string? type { get; set; }
    public string? title { get; set; }
    public string? uri { get; set; }
    public IDictionary<string, string[]> errors { get; }

}
