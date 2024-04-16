namespace Application.Common.Exceptions;

public class CustomInvalidOperationException : Exception
{
	public CustomInvalidOperationException(string message): base(message)
	{

	}
}
