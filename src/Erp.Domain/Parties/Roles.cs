namespace Erp.Domain.Parties;

public partial class CustomerRole
{
    public Guid PartyId { get; private set; }
    public int DebtorNumber { get; private set; }
    public decimal Discount { get; private set; }
    public bool IsVatShifted { get; private set; }
    public short PaymentTermDays { get; private set; }
    public decimal? CreditLimit { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private CustomerRole() { }

    internal CustomerRole(Guid partyId, int debtorNumber, decimal discount, bool isVatShifted, short paymentTermDays, decimal? creditLimit)
    {
        PartyId = partyId;
        DebtorNumber = debtorNumber;
        Discount = discount;
        IsVatShifted = isVatShifted;
        PaymentTermDays = paymentTermDays;
        CreditLimit = creditLimit;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(decimal discount, bool isVatShifted, short paymentTermDays, decimal? creditLimit)
    {
        Discount = discount;
        IsVatShifted = isVatShifted;
        PaymentTermDays = paymentTermDays;
        CreditLimit = creditLimit;
        UpdatedAt = DateTime.UtcNow;
    }
}

public partial class SupplierRole
{
    public Guid PartyId { get; private set; }
    public int SupplierNumber { get; private set; }
    public bool IsVatShifted { get; private set; }
    public short PaymentTermDays { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private SupplierRole() { }

    internal SupplierRole(Guid partyId, int supplierNumber, bool isVatShifted, short paymentTermDays)
    {
        PartyId = partyId;
        SupplierNumber = supplierNumber;
        IsVatShifted = isVatShifted;
        PaymentTermDays = paymentTermDays;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(bool isVatShifted, short paymentTermDays)
    {
        IsVatShifted = isVatShifted;
        PaymentTermDays = paymentTermDays;
        UpdatedAt = DateTime.UtcNow;
    }
}

public partial class CustomerRole
{
    public static CustomerRole Reconstitute(Guid partyId, int debtorNumber, decimal discount,
        bool isVatShifted, short paymentTermDays, decimal? creditLimit,
        DateTime createdAt, DateTime updatedAt) =>
        new() { PartyId = partyId, DebtorNumber = debtorNumber, Discount = discount,
                IsVatShifted = isVatShifted, PaymentTermDays = paymentTermDays,
                CreditLimit = creditLimit, CreatedAt = createdAt, UpdatedAt = updatedAt };
}

public partial class SupplierRole
{
    public static SupplierRole Reconstitute(Guid partyId, int supplierNumber, bool isVatShifted,
        short paymentTermDays, DateTime createdAt, DateTime updatedAt) =>
        new() { PartyId = partyId, SupplierNumber = supplierNumber, IsVatShifted = isVatShifted,
                PaymentTermDays = paymentTermDays, CreatedAt = createdAt, UpdatedAt = updatedAt };
}
