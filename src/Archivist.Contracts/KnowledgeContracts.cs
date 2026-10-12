namespace Archivist.Contracts;

public sealed record KnowledgeSearchResponse(
    Guid DocumentVersionId,
    string Title,
    string Kind,
    DateTimeOffset UpdatedAt,
    string Excerpt);

public sealed record KnowledgeSearchPageResponse(
    IReadOnlyList<KnowledgeSearchResponse> Items,
    int TotalCount);
