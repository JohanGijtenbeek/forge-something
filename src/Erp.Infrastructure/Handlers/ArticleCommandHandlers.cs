using Erp.Domain.Articles;
using Erp.Domain.Articles.Commands;
using Erp.Domain.Articles.Events;
using MassTransit;
using MediatR;

namespace Erp.Infrastructure.Handlers;

public class CreateArticleHandler : IRequestHandler<CreateArticleCommand, Guid>
{
    private readonly IArticleRepository _repository;
    private readonly IBus _bus;

    public CreateArticleHandler(IArticleRepository repository, IBus bus)
    {
        _repository = repository;
        _bus = bus;
    }

    public async Task<Guid> Handle(CreateArticleCommand command, CancellationToken ct)
    {
        if (!ArticleType.IsValid(command.ArticleType))
            throw new ArgumentException($"Invalid article type: {command.ArticleType}");

        var article = new Article(command.Code, command.Name, command.ArticleType,
            command.Description, command.CategoryId, command.UnitOfMeasureId, command.PurchasePrice, command.Revision);

        await _repository.AddAsync(article, ct);

        await _bus.Publish(new ArticleCreatedEvent(
            article.Id, article.Code, article.Name, article.ArticleType, DateTime.UtcNow), ct);

        return article.Id;
    }
}

public class UpdateArticleHandler : IRequestHandler<UpdateArticleCommand>
{
    private readonly IArticleRepository _repository;
    private readonly IBus _bus;

    public UpdateArticleHandler(IArticleRepository repository, IBus bus)
    {
        _repository = repository;
        _bus = bus;
    }

    public async Task Handle(UpdateArticleCommand command, CancellationToken ct)
    {
        if (!ArticleType.IsValid(command.ArticleType))
            throw new ArgumentException($"Invalid article type: {command.ArticleType}");

        var article = await _repository.GetByIdAsync(command.ArticleId, ct)
            ?? throw new KeyNotFoundException($"Article {command.ArticleId} not found.");

        article.Update(command.Code, command.Name, command.ArticleType,
            command.Description, command.CategoryId, command.UnitOfMeasureId, command.PurchasePrice, command.Revision);

        await _repository.UpdateAsync(article, ct);

        await _bus.Publish(new ArticleUpdatedEvent(
            article.Id, article.Code, article.Name, article.ArticleType, DateTime.UtcNow), ct);
    }
}

public class DeactivateArticleHandler : IRequestHandler<DeactivateArticleCommand>
{
    private readonly IArticleRepository _repository;
    private readonly IBus _bus;

    public DeactivateArticleHandler(IArticleRepository repository, IBus bus)
    {
        _repository = repository;
        _bus = bus;
    }

    public async Task Handle(DeactivateArticleCommand command, CancellationToken ct)
    {
        var article = await _repository.GetByIdAsync(command.ArticleId, ct)
            ?? throw new KeyNotFoundException($"Article {command.ArticleId} not found.");

        await _repository.DeactivateAsync(command.ArticleId, ct);

        await _bus.Publish(new ArticleDeactivatedEvent(
            article.Id, article.Code, article.Name, DateTime.UtcNow), ct);
    }
}

public class CreateArticleCategoryHandler : IRequestHandler<CreateArticleCategoryCommand, Guid>
{
    private readonly IArticleRepository _repository;

    public CreateArticleCategoryHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateArticleCategoryCommand command, CancellationToken ct)
    {
        var category = new ArticleCategory(command.Name, command.SortOrder);
        return await _repository.AddCategoryAsync(category, ct);
    }
}

public class CreateUnitOfMeasureHandler : IRequestHandler<CreateUnitOfMeasureCommand, Guid>
{
    private readonly IArticleRepository _repository;

    public CreateUnitOfMeasureHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateUnitOfMeasureCommand command, CancellationToken ct)
    {
        var uom = new UnitOfMeasure(command.Name, command.Abbreviation);
        return await _repository.AddUnitOfMeasureAsync(uom, ct);
    }
}

public class AddBomComponentHandler : IRequestHandler<AddBomComponentCommand, Guid>
{
    private readonly IArticleRepository _repository;

    public AddBomComponentHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(AddBomComponentCommand command, CancellationToken ct)
    {
        var parent = await _repository.GetByIdAsync(command.ParentArticleId, ct)
            ?? throw new KeyNotFoundException($"Article {command.ParentArticleId} not found.");

        if (parent.ArticleType != ArticleType.Manufactured)
            throw new InvalidOperationException("BOM components can only be added to articles of type 'manufactured'.");

        if (command.ChildArticleId == command.ParentArticleId)
            throw new InvalidOperationException("An article cannot reference itself in a BOM.");

        _ = await _repository.GetByIdAsync(command.ChildArticleId, ct)
            ?? throw new KeyNotFoundException($"Article {command.ChildArticleId} not found.");

        return await _repository.AddBomComponentAsync(
            command.ParentArticleId, command.ChildArticleId,
            command.Quantity, command.UnitOfMeasureId, command.SortOrder, ct);
    }
}

public class UpdateBomComponentHandler : IRequestHandler<UpdateBomComponentCommand>
{
    private readonly IArticleRepository _repository;

    public UpdateBomComponentHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateBomComponentCommand command, CancellationToken ct)
    {
        _ = await _repository.GetBomLineAsync(command.BomLineId, ct)
            ?? throw new KeyNotFoundException($"BOM line {command.BomLineId} not found.");

        await _repository.UpdateBomComponentAsync(
            command.BomLineId, command.Quantity, command.UnitOfMeasureId, command.SortOrder, ct);
    }
}

public class RemoveBomComponentHandler : IRequestHandler<RemoveBomComponentCommand>
{
    private readonly IArticleRepository _repository;

    public RemoveBomComponentHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(RemoveBomComponentCommand command, CancellationToken ct)
    {
        _ = await _repository.GetBomLineAsync(command.BomLineId, ct)
            ?? throw new KeyNotFoundException($"BOM line {command.BomLineId} not found.");

        await _repository.RemoveBomComponentAsync(command.BomLineId, ct);
    }
}

public class AddArticleOperationHandler : IRequestHandler<AddArticleOperationCommand, Guid>
{
    private readonly IArticleRepository _repository;

    public AddArticleOperationHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(AddArticleOperationCommand command, CancellationToken ct)
    {
        var article = await _repository.GetByIdAsync(command.ArticleId, ct)
            ?? throw new KeyNotFoundException($"Article {command.ArticleId} not found.");

        if (article.ArticleType != ArticleType.Manufactured)
            throw new InvalidOperationException("Operations can only be added to articles of type 'manufactured'.");

        var opType = await _repository.GetOperationTypeAsync(command.OperationTypeId, ct)
            ?? throw new KeyNotFoundException($"OperationType {command.OperationTypeId} not found.");

        var op = new ArticleOperation(
            command.ArticleId, command.SequenceNumber, opType.Id, opType.Name, opType.IsSubcontracted,
            command.EstimatedMinutes, command.Notes, command.IsConditional);

        await _repository.AddOperationAsync(op, ct);
        return op.Id;
    }
}

public class UpdateArticleOperationHandler : IRequestHandler<UpdateArticleOperationCommand>
{
    private readonly IArticleRepository _repository;

    public UpdateArticleOperationHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(UpdateArticleOperationCommand command, CancellationToken ct)
    {
        var op = await _repository.GetOperationAsync(command.OperationId, ct)
            ?? throw new KeyNotFoundException($"ArticleOperation {command.OperationId} not found.");

        op.Update(command.SequenceNumber, command.EstimatedMinutes, command.Notes, command.IsConditional);
        await _repository.UpdateOperationAsync(op, ct);
    }
}

public class RemoveArticleOperationHandler : IRequestHandler<RemoveArticleOperationCommand>
{
    private readonly IArticleRepository _repository;

    public RemoveArticleOperationHandler(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(RemoveArticleOperationCommand command, CancellationToken ct)
    {
        _ = await _repository.GetOperationAsync(command.OperationId, ct)
            ?? throw new KeyNotFoundException($"ArticleOperation {command.OperationId} not found.");

        await _repository.RemoveOperationAsync(command.OperationId, ct);
    }
}
