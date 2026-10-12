namespace Archivist.Application.Chat;

/// <summary>
/// Executes a grounded chat query for an already-authenticated and authorized caller.
/// Implementations must enforce the workspace access predicate before retrieval.
/// </summary>
public interface IChatQueryService
{
    Task<ChatAnswer> QueryAsync(ChatQuestion question, CancellationToken cancellationToken);
}

public sealed record ChatQuestion(Guid CallerId, string Text, Guid? SessionId);

public sealed record ChatAnswer(
    Guid SessionId,
    string Text,
    IReadOnlyList<ChatCitation> Citations);

public sealed record ChatCitation(
    Guid DocumentVersionId,
    string DocumentTitle,
    string SourceLocation,
    string Excerpt);
