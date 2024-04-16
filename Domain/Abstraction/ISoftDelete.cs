namespace Domain.Abstraction;

public interface ISoftDelete
{
    public bool Deleted { get; set; }
}
