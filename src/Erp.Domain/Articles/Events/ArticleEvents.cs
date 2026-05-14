namespace Erp.Domain.Articles.Events;

public record ArticleCreatedEvent(
    Guid ArticleId,
    string Code,
    string Name,
    string ArticleType,
    DateTime OccurredAt
);

public record ArticleUpdatedEvent(
    Guid ArticleId,
    string Code,
    string Name,
    string ArticleType,
    DateTime OccurredAt
);

public record ArticleDeactivatedEvent(
    Guid ArticleId,
    string Code,
    string Name,
    DateTime OccurredAt
);
