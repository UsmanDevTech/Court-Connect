using System;
namespace Domain.Exceptions;

public class UnProcessRequestException : Exception
{
    public UnProcessRequestException(string code)
        : base(code)
    {
    }
}
