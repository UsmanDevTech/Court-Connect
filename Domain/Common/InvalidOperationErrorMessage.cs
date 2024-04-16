namespace Domain.Common;

public static class InvalidOperationErrorMessage
{
    public const string otp_code_not_found = "Verification failed! Invalid One Time Password (OTP), please check your code and try again";
    public const string otp_code_has_expired = "Expired code! One Time Password (OTP) has expired, please try again";
    public const string invalid_password = "Invalid password, please try again with valid password";
    public static string MustInBetweenErrorMessage(string name,string range)
    {
        return $"{name} must be in between {range}.";
    }
    public static string IsRequiredErrorMessage(string name)
    {
        return $"{name} is required.";
    }
    public static string MinLengthErrorMessage(string name,int length)
    {
        return $"{name} must have atleast {length} characters.";
    }
    public static string MaxLengthErrorMessage(string name, int length)
    {
        return $"{name} must not exceed {length} characters.";
    }
    public static string AlreadyExistsErrorMessage(string entity,string name)
    {
        return $"{entity} with same {name} already exists. Please try with another one.";
    }
    public static string AlreadyTakenErrorMessage(string entity,string name)
    {
        return $"{entity} ( {name} ) already taken by some user. Please try with different {entity}.";
    }
    public static string EntityNotFound(string entity)
    {
        return $"{entity} does not exists in database.";
    }
    public static string AccountNotVerified(string name)
    {
        return $"{name} is not verified.";
    }
    public static string InvalidDateFormat(string name)
    {
        return $"Make sure {name} are in the right format.Required format MM/DD/YYYY";
    }
    public static string InvalidTimeFormat(string name)
    {
        return $"Make sure {name} are in the right format.Required format hh:mm:ss";
    }
    public static string InvalidEmailFormat(string email)
    {
        return $"Email is badly formatted";
    }
    public static string InvalidTrainerCode(string code)
    {
        return $"Team lead with {code} does not belong to your organization";
    }
}
