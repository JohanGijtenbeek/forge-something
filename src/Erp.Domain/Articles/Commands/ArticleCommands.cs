using MediatR;

namespace Erp.Domain.Articles.Commands;

public record CreateArticleCommand(
    string Code,
    string Name,
    string ArticleType,
    string? Description,
    Guid? CategoryId,
    Guid? UnitOfMeasureId,
    decimal? PurchasePrice
) : IRequest<Guid>;

public record UpdateArticleCommand(
    Guid ArticleId,
    string Code,
    string Name,
    string ArticleType,
    string? Description,
    Guid? CategoryId,
    Guid? UnitOfMeasureId,
    decimal? PurchasePrice
) : IRequest;

public record DeactivateArticleCommand(
    Guid ArticleId
) : IRequest;

public record CreateArticleCategoryCommand(
    string Name,
    int SortOrder
) : IRequest<Guid>;

public record CreateUnitOfMeasureCommand(
    string Name,
    string Abbreviation
) : IRequest<Guid>;

public record AddBomComponentCommand(
    Guid ParentArticleId,
    Guid ChildArticleId,
    decimal Quantity,
    Guid? UnitOfMeasureId,
    int SortOrder
) : IRequest<Guid>;

public record UpdateBomComponentCommand(
    Guid BomLineId,
    decimal Quantity,
    Guid? UnitOfMeasureId,
    int SortOrder
) : IRequest;

public record RemoveBomComponentCommand(
    Guid BomLineId
) : IRequest;
