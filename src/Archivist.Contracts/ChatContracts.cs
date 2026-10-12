namespace Archivist.Contracts;

/// <summary>Request body for an authorized, grounded chat query.</summary>
public sealed record ChatQueryRequest(string Question, Guid? SessionId = null);

/// <summary>An immutable source location supporting a grounded answer.</summary>
public sealed record CitationResponse(
    Guid DocumentVersionId,
    string DocumentTitle,
    string SourceLocation,
    string Excerpt);

/// <summary>The final form of a chat response. Streaming will use this shape as its terminal event.</summary>
public sealed record ChatQueryResponse(
    Guid SessionId,
    string Answer,
    IReadOnlyList<CitationResponse> Citations);

public sealed record ChatSessionSummaryResponse(
    Guid Id,
    string Title,
    DateTimeOffset UpdatedAt);
