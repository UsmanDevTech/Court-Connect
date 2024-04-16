
using Domain.Enum;

namespace Domain.Contracts;

public sealed class FilterContract
{
    public ActionFilterEnum actionType { get; set; }
    public string val { get; set; } = null!;
}

public sealed class RequestBaseContract
{
    public List<FilterContract>? filters { get; set; } = new();
   
}