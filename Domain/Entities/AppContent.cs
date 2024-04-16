using Domain.Common;
using Domain.Enum;

namespace Domain.Entities;
public sealed class AppContent: BaseAuditableEntity
{
    private AppContent(string? name, string? icon, string value, AppContentTypeEnum type, DateTime created)
    {
        Name = name;
        Icon = icon;
        Value = value;
        Type = type;
        Created = created;
    }


    //Properties
    public string? Name { get; private set; }
    public string? Icon { get; private set; }
    public string? Value { get; private set; }

    //Enums
    public AppContentTypeEnum Type { get; private set; }  
    
    //Methods
    public void UpdateValue(string value)
    {
        Value = value;
    }
    public void UpdateIcon(string icon)
    {
        Icon = icon;
    }
    public void UpdateName(string name)
    {
        Name = name;
    }

    //Factory Method
    public static AppContent Create(string? name, string? icon, string value, AppContentTypeEnum type, DateTime created)
    {
        return new AppContent(name,icon, value, type, created);
    }
}
