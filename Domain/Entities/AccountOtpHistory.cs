using Domain.Common;
using Domain.Enum;

namespace Domain.Entities;
public class AccountOtpHistory : BaseAuditableEntity
{
    private AccountOtpHistory(string code, DateTime expiryDateTime, OtpMediaTypeEnum otpMediaType, string? replacementValue, string? originalValue)
    {
        Code = code;
        ExpiryDateTime = expiryDateTime;
        OtpMediaType = otpMediaType;
        ReplacementValue = replacementValue;
        OriginalValue = originalValue;
    }
    private AccountOtpHistory(string code, DateTime expiryDateTime, OtpMediaTypeEnum otpMediaType, string? replacementValue, string? originalValue, string? creadtedBy)
    {
        Code = code;
        ExpiryDateTime = expiryDateTime;
        OtpMediaType = otpMediaType;
        ReplacementValue = replacementValue;
        OriginalValue = originalValue;
        CreatedBy = creadtedBy;
    }
    public string Code { get; private set; } = null!;
    public DateTime ExpiryDateTime { get; private set; }
    public OtpMediaTypeEnum OtpMediaType { get; private set; }
    public string? ReplacementValue { get; private set; }
    public string? OriginalValue { get; private set; }

    //Factory Method
    public static AccountOtpHistory Create(string code, DateTime expiryDateTime, OtpMediaTypeEnum otpMediaType, string? replacementValue, string? originalValue, string? createdBy = null)
    {
        if (string.IsNullOrEmpty(createdBy))
            return new AccountOtpHistory(code, expiryDateTime, otpMediaType, replacementValue, originalValue);

        return new AccountOtpHistory(code, expiryDateTime, otpMediaType, replacementValue, originalValue, createdBy);

    }
}
