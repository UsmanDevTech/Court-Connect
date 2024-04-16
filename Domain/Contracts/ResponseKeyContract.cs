using Domain.Enum;

namespace Domain.Contracts;
public class ResponseKey
{
    public ResponseKey()
    {

    }
    public ResponseKey(string key)
    {
        this.key = key;
    }
    public string? key { get; set; }
}
public class NotificationCounterContract
{
    public NotificationCounterContract(int counter)
    {
        this.counter = counter;
    }
    public int counter { get; set; }
}
public class ResponseKeyContract : ResponseKey
{
    public string? secretKey { get; set; }
    public bool emailConfirmationRequired { get; set; }   
}

public class ResultContract
{
    public int id { get; set; }
}