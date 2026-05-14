namespace Erp.Domain.Articles;

public class MachineType
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }

    private MachineType() { Name = null!; }

    public static MachineType Reconstitute(Guid id, string name, bool isActive) =>
        new() { Id = id, Name = name, IsActive = isActive };
}
