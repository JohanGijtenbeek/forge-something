using Erp.Domain.Articles;
using Erp.Domain.Articles.Commands;
using Erp.Domain.Articles.Events;
using MediatR;

namespace Erp.Infrastructure.Handlers;

public class CreateArticleHandler : IRequestHandler<CreateArticleCommand, Guid>
{
    private readonly IArticleRepository _repository;
    private readonly IPublisher _publisher;

    public CreateArticleHandler(IArticleRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<Guid> Handle(CreateArticleCommand command, CancellationToken ct)
    {
        if (!ArticleType.IsValid(command.ArticleType))
            throw new ArgumentException($"Invalid article type: {command.ArticleType}");

        var article = new Article(command.Code, command.Name, command.ArticleType,
            command.Description, command.CategoryId, command.UnitOfMeasureId, command.PurchasePrice);

        await _repository.AddAsync(article, ct);

        await _publisher.Publish(new ArticleCreatedEvent(
            article.Id, article.Code, article.Name, article.ArticleType, DateTime.UtcNow), ct);

        return article.Id;
    }
}

public class UpdateArticleHandler : IRequestHandler<UpdateArticleCommand>
{
    private readonly IArticleRepository _repository;
    private readonly IPublisher _publisher;

    public UpdateArticleHandler(IArticleRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task Handle(UpdateArticleCommand command, CancellationToken ct)
    {
        if (!ArticleType.IsValid(command.ArticleType))
            throw new ArgumentException($"Invalid article type: {command.ArticleType}");

        var article = await _repository.GetByIdAsync(command.ArticleId, ct)
            ?? throw new KeyNotFoundException($"Article {command.ArticleId} not found.");

        article.Update(command.Code, command.Name, command.ArticleType,
            command.Description, command.CategoryId, command.UnitOfMeasureId, command.PurchasePrice);

        await _repository.UpdateAsync(article, ct);

        await _publisher.Publish(new ArticleUpdatedEvent(
            article.Id, article.Code, article.Name, article.ArticleType, DateTime.UtcNow), ct);
    }
}

public class DeactivateArticleHandler : IRequestHandler<DeactivateArticleCommand>
{
    private readonly IArticleRepository _repository;
    private readonly IPublisher _publisher;

    public DeactivateArticleHandler(IArticleRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task Handle(DeactivateArticleCommand command, CancellationToken ct)
    {
        var article = await _repository.GetByIdAsync(command.ArticleId, ct)
            ?? throw new KeyNotFoundException($"Article {command.ArticleId} not found.");

        await _repository.DeactivateAsync(command.ArticleId, ct);

        await _publisher.Publish(new ArticleDeactivatedEvent(
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
