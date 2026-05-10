using MediatR;

namespace Erp.Domain.Articles.Events;

public record ArticleCreatedEvent(
    Guid ArticleId,
    string Code,
    string Name,
    string ArticleType,
    DateTime OccurredAt
) : INotification;

public record ArticleUpdatedEvent(
    Guid ArticleId,
    string Code,
    string Name,
    string ArticleType,
    DateTime OccurredAt
) : INotification;

public record ArticleDeactivatedEvent(
    Guid ArticleId,
    string Code,
    string Name,
    DateTime OccurredAt
) : INotification;
