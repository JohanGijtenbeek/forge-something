namespace Erp.Domain.Quotes;

public class Quote
{
    public Guid      Id             { get; private set; }
    public int       QuoteNumber    { get; private set; }
    public Guid?     CustomerId     { get; private set; }
    public string?   CustomerName   { get; private set; }
    public DateOnly  Date           { get; private set; }
    public string?   Reference      { get; private set; }
    public string?   ContactPerson  { get; private set; }
    public string?   DeliveryTime   { get; private set; }
    public decimal   HourlyRate     { get; private set; }
    public decimal   MaterialMargin { get; private set; }
    public decimal   StandardMargin { get; private set; }
    public decimal   SetupTime      { get; private set; }
    public string    Status         { get; private set; } = null!;
    public string?   Remarks        { get; private set; }
    public DateTime  CreatedAt      { get; private set; }
    public DateTime  UpdatedAt      { get; private set; }

    private Quote() { }

    public static Quote Create(
        Guid? customerId, string? customerName, DateOnly date,
        string? reference, string? contactPerson, string? deliveryTime,
        decimal hourlyRate, decimal materialMargin, decimal standardMargin, decimal setupTime)
    {
        return new Quote
        {
            Id             = Guid.NewGuid(),
            CustomerId     = customerId,
            CustomerName   = customerName,
            Date           = date,
            Reference      = reference,
            ContactPerson  = contactPerson,
            DeliveryTime   = deliveryTime,
            HourlyRate     = hourlyRate,
            MaterialMargin = materialMargin,
            StandardMargin = standardMargin,
            SetupTime      = setupTime,
            Status         = QuoteStatus.Draft,
            CreatedAt      = DateTime.UtcNow,
            UpdatedAt      = DateTime.UtcNow
        };
    }

    public void UpdateHeader(
        Guid? customerId, string? customerName, DateOnly date,
        string? reference, string? contactPerson, string? deliveryTime,
        decimal hourlyRate, decimal materialMargin, decimal standardMargin, decimal setupTime,
        string? remarks)
    {
        CustomerId     = customerId;
        CustomerName   = customerName;
        Date           = date;
        Reference      = reference;
        ContactPerson  = contactPerson;
        DeliveryTime   = deliveryTime;
        HourlyRate     = hourlyRate;
        MaterialMargin = materialMargin;
        StandardMargin = standardMargin;
        SetupTime      = setupTime;
        Remarks        = remarks;
        UpdatedAt      = DateTime.UtcNow;
    }

    public void UpdateStatus(string newStatus)
    {
        if (!QuoteStatus.CanTransitionTo(Status, newStatus))
            throw new InvalidOperationException($"Cannot transition quote from '{Status}' to '{newStatus}'.");
        Status    = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Quote Reconstitute(
        Guid id, int quoteNumber, Guid? customerId, string? customerName, DateOnly date,
        string? reference, string? contactPerson, string? deliveryTime,
        decimal hourlyRate, decimal materialMargin, decimal standardMargin, decimal setupTime,
        string status, string? remarks, DateTime createdAt, DateTime updatedAt) =>
        new()
        {
            Id             = id,
            QuoteNumber    = quoteNumber,
            CustomerId     = customerId,
            CustomerName   = customerName,
            Date           = date,
            Reference      = reference,
            ContactPerson  = contactPerson,
            DeliveryTime   = deliveryTime,
            HourlyRate     = hourlyRate,
            MaterialMargin = materialMargin,
            StandardMargin = standardMargin,
            SetupTime      = setupTime,
            Status         = status,
            Remarks        = remarks,
            CreatedAt      = createdAt,
            UpdatedAt      = updatedAt
        };
}
