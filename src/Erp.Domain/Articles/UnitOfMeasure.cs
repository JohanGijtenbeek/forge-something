namespace Erp.Domain.Articles;

public class UnitOfMeasure
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Abbreviation { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private UnitOfMeasure() { Name = null!; Abbreviation = null!; }

    public UnitOfMeasure(string name, string abbreviation)
    {
        Id = Guid.NewGuid();
        Name = name;
        Abbreviation = abbreviation;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static UnitOfMeasure Reconstitute(
        Guid id, string name, string abbreviation, bool isActive, DateTime createdAt, DateTime updatedAt)
    {
        return new UnitOfMeasure
        {
            Id = id,
            Name = name,
            Abbreviation = abbreviation,
            IsActive = isActive,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }
}
