using Domain.Common;

namespace Domain.Entities;

public class UserInterest: BaseAuditableEntity
{
    private UserInterest(string createdBy, string name, DateTime created)
    {
        Name = name;
        CreatedBy = createdBy;
        Created = created;
    }

    //Properties
    public string Name { get; private set; } = null!;
  
    //Factory Method
    public static UserInterest Create(string createdBy, string name, DateTime created)
    {
        return new UserInterest(createdBy, name, created);
    }
}