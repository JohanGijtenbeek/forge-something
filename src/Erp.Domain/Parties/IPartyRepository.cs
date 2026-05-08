namespace Erp.Domain.Parties;

public interface IPartyRepository
{
    Task<Party?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Party?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Party>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);
    Task<(IReadOnlyList<Party> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, bool includeInactive = false, CancellationToken ct = default);
    Task<IReadOnlyList<Party>> GetCustomersAsync(bool includeInactive = false, CancellationToken ct = default);
    Task<IReadOnlyList<Party>> GetSuppliersAsync(bool includeInactive = false, CancellationToken ct = default);
    Task AddAsync(Party party, CancellationToken ct = default);
    Task UpdateAsync(Party party, CancellationToken ct = default);
    Task DeactivateAsync(Guid id, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);

    // Partial update methods - worden direct weggeschreven
    Task<IReadOnlyList<PartyHistoryEntry>> GetHistoryAsync(Guid id, CancellationToken ct = default);
}
