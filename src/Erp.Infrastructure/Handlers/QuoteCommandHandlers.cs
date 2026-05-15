using Dapper;
using Erp.Domain.Orders.Commands;
using Erp.Domain.Parties;
using Erp.Domain.Quotes;
using Erp.Domain.Quotes.Commands;
using Erp.Domain.Quotes.Events;
using Erp.Infrastructure.Persistence;
using MassTransit;
using MediatR;

namespace Erp.Infrastructure.Handlers;

public class CreateQuoteHandler : IRequestHandler<CreateQuoteCommand, Guid>
{
    private readonly IQuoteRepository _quoteRepo;
    private readonly IPartyRepository _partyRepo;
    private readonly IBus _bus;
    private readonly DbConnectionFactory _factory;

    public CreateQuoteHandler(IQuoteRepository quoteRepo, IPartyRepository partyRepo,
        IBus bus, DbConnectionFactory factory)
    {
        _quoteRepo = quoteRepo;
        _partyRepo = partyRepo;
        _bus = bus;
        _factory = factory;
    }

    public async Task<Guid> Handle(CreateQuoteCommand command, CancellationToken ct)
    {
        string? customerName = null;
        if (command.CustomerId.HasValue)
        {
            var party = await _partyRepo.GetByIdAsync(command.CustomerId.Value, ct)
                ?? throw new KeyNotFoundException($"Party {command.CustomerId.Value} not found.");
            customerName = party.Name;
        }

        using var conn = _factory.Create();
        var quoteNumber = await conn.QuerySingleAsync<int>("SELECT NEXT VALUE FOR mdata.seq_quote_number");

        var quote = Quote.Create(
            command.CustomerId, customerName, command.Date, command.Reference,
            command.ContactPerson, command.DeliveryTime, command.HourlyRate,
            command.MaterialMargin, command.StandardMargin, command.SetupTime);

        // Inject the sequence number (Quote.Create uses Guid.NewGuid for id, number comes from sequence)
        var quoteWithNumber = Quote.Reconstitute(
            quote.Id, quoteNumber, quote.CustomerId, quote.CustomerName, quote.Date,
            quote.Reference, quote.ContactPerson, quote.DeliveryTime,
            quote.HourlyRate, quote.MaterialMargin, quote.StandardMargin, quote.SetupTime,
            quote.Status, quote.Remarks, quote.CreatedAt, quote.UpdatedAt);

        await _quoteRepo.SaveAsync(quoteWithNumber);

        await _bus.Publish(new QuoteCreatedEvent(
            quoteWithNumber.Id, quoteNumber, command.CustomerId, customerName, DateTime.UtcNow), ct);

        return quoteWithNumber.Id;
    }
}

public class UpdateQuoteHeaderHandler : IRequestHandler<UpdateQuoteHeaderCommand>
{
    private readonly IQuoteRepository _quoteRepo;
    private readonly IPartyRepository _partyRepo;

    public UpdateQuoteHeaderHandler(IQuoteRepository quoteRepo, IPartyRepository partyRepo)
    {
        _quoteRepo = quoteRepo;
        _partyRepo = partyRepo;
    }

    public async Task Handle(UpdateQuoteHeaderCommand command, CancellationToken ct)
    {
        var quote = await _quoteRepo.GetByIdAsync(command.QuoteId)
            ?? throw new KeyNotFoundException($"Quote {command.QuoteId} not found.");

        if (QuoteStatus.IsTerminal(quote.Status))
            throw new InvalidOperationException($"Cannot update a {quote.Status} quote.");

        string? customerName = null;
        if (command.CustomerId.HasValue)
        {
            var party = await _partyRepo.GetByIdAsync(command.CustomerId.Value, ct)
                ?? throw new KeyNotFoundException($"Party {command.CustomerId.Value} not found.");
            customerName = party.Name;
        }

        quote.UpdateHeader(command.CustomerId, customerName, command.Date, command.Reference,
            command.ContactPerson, command.DeliveryTime, command.HourlyRate,
            command.MaterialMargin, command.StandardMargin, command.SetupTime, command.Remarks);

        await _quoteRepo.UpdateHeaderAsync(quote);
    }
}

public class UpdateQuoteStatusHandler : IRequestHandler<UpdateQuoteStatusCommand>
{
    private readonly IQuoteRepository _quoteRepo;
    private readonly IBus _bus;

    public UpdateQuoteStatusHandler(IQuoteRepository quoteRepo, IBus bus)
    {
        _quoteRepo = quoteRepo;
        _bus = bus;
    }

    public async Task Handle(UpdateQuoteStatusCommand command, CancellationToken ct)
    {
        if (!QuoteStatus.IsValid(command.NewStatus))
            throw new InvalidOperationException($"Invalid status: {command.NewStatus}.");

        var quote = await _quoteRepo.GetByIdAsync(command.QuoteId)
            ?? throw new KeyNotFoundException($"Quote {command.QuoteId} not found.");

        var oldStatus = quote.Status;
        quote.UpdateStatus(command.NewStatus);
        await _quoteRepo.UpdateStatusAsync(quote);

        await _bus.Publish(new QuoteStatusChangedEvent(
            quote.Id, quote.QuoteNumber, oldStatus, command.NewStatus, DateTime.UtcNow), ct);
    }
}

public class AddQuoteLineHandler : IRequestHandler<AddQuoteLineCommand, Guid>
{
    private readonly IQuoteRepository _quoteRepo;

    public AddQuoteLineHandler(IQuoteRepository quoteRepo) => _quoteRepo = quoteRepo;

    public async Task<Guid> Handle(AddQuoteLineCommand command, CancellationToken ct)
    {
        var quote = await _quoteRepo.GetByIdAsync(command.QuoteId)
            ?? throw new KeyNotFoundException($"Quote {command.QuoteId} not found.");

        if (QuoteStatus.IsTerminal(quote.Status))
            throw new InvalidOperationException($"Cannot add lines to a {quote.Status} quote.");

        var line = QuoteLine.Create(
            command.QuoteId, command.SortOrder, command.PartName, command.PartNumber,
            command.Quantity, command.ArticleId,
            command.MaterialType, command.MaterialCode, command.MaterialCode2,
            command.MaterialGeometry, command.MaterialSizeMm, command.MaterialLengthMm,
            command.MaterialQuantity, command.MaterialPrice, command.MaterialSource,
            command.OperationCount, command.OperationTimeMinutes,
            command.SubcontractingCount, command.SubcontractingPrice,
            command.IsManualPrice, command.ManualPrice, command.Remarks);

        if (!command.IsManualPrice)
        {
            var calculated = line.CalculateTotalPricePerUnit(
                quote.HourlyRate, quote.MaterialMargin, quote.StandardMargin, quote.SetupTime);
            line.SetCalculatedPrice(calculated);
        }

        await _quoteRepo.AddLineAsync(line);
        return line.Id;
    }
}

public class UpdateQuoteLineHandler : IRequestHandler<UpdateQuoteLineCommand>
{
    private readonly IQuoteRepository _quoteRepo;

    public UpdateQuoteLineHandler(IQuoteRepository quoteRepo) => _quoteRepo = quoteRepo;

    public async Task Handle(UpdateQuoteLineCommand command, CancellationToken ct)
    {
        var line = await _quoteRepo.GetLineAsync(command.LineId)
            ?? throw new KeyNotFoundException($"Quote line {command.LineId} not found.");

        var quote = await _quoteRepo.GetByIdAsync(line.QuoteId)
            ?? throw new KeyNotFoundException($"Quote {line.QuoteId} not found.");

        if (QuoteStatus.IsTerminal(quote.Status))
            throw new InvalidOperationException($"Cannot edit lines on a {quote.Status} quote.");

        line.Update(
            command.SortOrder, command.PartName, command.PartNumber, command.Quantity,
            command.ArticleId, command.MaterialType, command.MaterialCode, command.MaterialCode2,
            command.MaterialGeometry, command.MaterialSizeMm, command.MaterialLengthMm,
            command.MaterialQuantity, command.MaterialPrice, command.MaterialSource,
            command.OperationCount, command.OperationTimeMinutes,
            command.SubcontractingCount, command.SubcontractingPrice,
            command.IsManualPrice, command.ManualPrice, command.Remarks);

        if (!command.IsManualPrice)
        {
            var calculated = line.CalculateTotalPricePerUnit(
                quote.HourlyRate, quote.MaterialMargin, quote.StandardMargin, quote.SetupTime);
            line.SetCalculatedPrice(calculated);
        }

        await _quoteRepo.UpdateLineAsync(line);
    }
}

public class RemoveQuoteLineHandler : IRequestHandler<RemoveQuoteLineCommand>
{
    private readonly IQuoteRepository _quoteRepo;

    public RemoveQuoteLineHandler(IQuoteRepository quoteRepo) => _quoteRepo = quoteRepo;

    public async Task Handle(RemoveQuoteLineCommand command, CancellationToken ct)
    {
        var line = await _quoteRepo.GetLineAsync(command.LineId)
            ?? throw new KeyNotFoundException($"Quote line {command.LineId} not found.");

        var quote = await _quoteRepo.GetByIdAsync(line.QuoteId)
            ?? throw new KeyNotFoundException($"Quote {line.QuoteId} not found.");

        if (QuoteStatus.IsTerminal(quote.Status))
            throw new InvalidOperationException($"Cannot remove lines from a {quote.Status} quote.");

        await _quoteRepo.RemoveLineAsync(command.LineId);
    }
}

public class AcceptQuoteLineHandler : IRequestHandler<AcceptQuoteLineCommand>
{
    private readonly IQuoteRepository _quoteRepo;

    public AcceptQuoteLineHandler(IQuoteRepository quoteRepo) => _quoteRepo = quoteRepo;

    public async Task Handle(AcceptQuoteLineCommand command, CancellationToken ct)
    {
        var line = await _quoteRepo.GetLineAsync(command.LineId)
            ?? throw new KeyNotFoundException($"Quote line {command.LineId} not found.");

        line.Accept();
        await _quoteRepo.UpdateLineAsync(line);
    }
}

public class ConvertQuoteToOrdersHandler : IRequestHandler<ConvertQuoteToOrdersCommand, IReadOnlyList<Guid>>
{
    private readonly IQuoteRepository _quoteRepo;
    private readonly IMediator _mediator;
    private readonly IBus _bus;

    public ConvertQuoteToOrdersHandler(IQuoteRepository quoteRepo, IMediator mediator, IBus bus)
    {
        _quoteRepo = quoteRepo;
        _mediator = mediator;
        _bus = bus;
    }

    public async Task<IReadOnlyList<Guid>> Handle(ConvertQuoteToOrdersCommand command, CancellationToken ct)
    {
        var quote = await _quoteRepo.GetByIdAsync(command.QuoteId)
            ?? throw new KeyNotFoundException($"Quote {command.QuoteId} not found.");

        var lines = (await _quoteRepo.GetLinesAsync(command.QuoteId)).ToList();
        var acceptedLines = lines.Where(l => l.IsAccepted).ToList();

        if (acceptedLines.Count == 0)
            throw new InvalidOperationException("No accepted lines to convert.");

        var missingArticle = acceptedLines.Where(l => l.ArticleId is null).ToList();
        if (missingArticle.Count > 0)
        {
            var names = string.Join(", ", missingArticle.Select(l => l.PartName));
            throw new InvalidOperationException(
                $"The following accepted lines have no article linked and cannot be converted: {names}. " +
                "Link each line to an article before converting.");
        }

        var orderIds = new List<Guid>();
        foreach (var line in acceptedLines)
        {
            var orderId = await _mediator.Send(new CreateProductionOrderCommand(
                line.ArticleId!.Value,
                quote.CustomerId,
                line.Quantity,
                "st",
                null,
                null,
                quote.Id), ct);
            orderIds.Add(orderId);
        }

        await _bus.Publish(new QuoteConvertedEvent(
            quote.Id, quote.QuoteNumber, orderIds, DateTime.UtcNow), ct);

        return orderIds;
    }
}
