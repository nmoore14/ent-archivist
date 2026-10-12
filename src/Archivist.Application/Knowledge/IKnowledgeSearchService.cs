namespace Archivist.Application.Knowledge;

/// <summary>
/// Searches document metadata visible to an already-authenticated and authorized caller.
/// This is deliberately separate from retrieval used by chat.
/// </summary>
public interface IKnowledgeSearchService
{
    Task<KnowledgeSearchPage> SearchAsync(KnowledgeSearchQuery query, CancellationToken cancellationToken);
}

public sealed record KnowledgeSearchQuery(Guid CallerId, string? Text, string? Kind);

public sealed record KnowledgeSearchPage(IReadOnlyList<KnowledgeSearchResult> Items, int TotalCount);

public sealed record KnowledgeSearchResult(
    Guid DocumentVersionId,
    string Title,
    string Kind,
    DateTimeOffset UpdatedAt,
    string Excerpt);
