namespace Erp.Domain.Quotes;

public interface IQuoteRepository
{
    Task<Quote?>                    GetByIdAsync(Guid id);
    Task<IEnumerable<QuoteLine>>    GetLinesAsync(Guid quoteId);
    Task<QuoteLine?>                GetLineAsync(Guid lineId);
    Task<(IEnumerable<Quote> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, string? status);
    Task<IEnumerable<QuoteHistoryEntry>> GetHistoryAsync(Guid quoteId);

    Task SaveAsync(Quote quote);
    Task UpdateHeaderAsync(Quote quote);
    Task UpdateStatusAsync(Quote quote);

    Task AddLineAsync(QuoteLine line);
    Task UpdateLineAsync(QuoteLine line);
    Task RemoveLineAsync(Guid lineId);
}

public record QuoteHistoryEntry(long Id, string EventType, string Summary, string ChangedBy, DateTime ChangedAt);
