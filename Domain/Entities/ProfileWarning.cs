using Domain.Common;

namespace Domain.Entities;

public class ProfileWarning : BaseAuditableEntity
{
    private ProfileWarning(string createdBy, string issuedUser, string title, string description, DateTime created)
    {
        Title = title;
        Description = description;
        IssuedUser = issuedUser;
        CreatedBy = createdBy;
        Created = created;
    }

    //Properties
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string IssuedUser { get; private set; } = null!;

    //Factory Method
    public static ProfileWarning Create(string createdBy, string issuedUser, string title, string description, DateTime created)
    {
        return new ProfileWarning(createdBy, issuedUser, title, description, created);
    }
}