namespace Erp.Domain.Articles;

public class OperationType
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public bool IsSubcontracted { get; private set; }
    public Guid? MachineTypeId { get; private set; }
    public string? MachineTypeName { get; private set; }
    public bool IsActive { get; private set; }

    private OperationType() { Name = null!; }

    public static OperationType Reconstitute(
        Guid id, string name, bool isSubcontracted,
        Guid? machineTypeId, string? machineTypeName, bool isActive) =>
        new()
        {
            Id = id,
            Name = name,
            IsSubcontracted = isSubcontracted,
            MachineTypeId = machineTypeId,
            MachineTypeName = machineTypeName,
            IsActive = isActive
        };
}
