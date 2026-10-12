namespace Archivist.Domain;

public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public abstract class SoftDeletableEntity : AuditableEntity
{
    public DateTimeOffset? DeletedAt { get; set; }
}

public sealed class User : AuditableEntity
{
    public required string DisplayName { get; set; }
    public string? Email { get; set; }
    public required string Status { get; set; }
}

public sealed class ExternalIdentity : Entity
{
    public Guid UserId { get; set; }
    public required string Provider { get; set; }
    public required string Issuer { get; set; }
    public required string TenantId { get; set; }
    public required string SubjectId { get; set; }
}

public sealed class Role : Entity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Permission is the established domain term.")]
public sealed class Permission : Entity
{
    public required string Code { get; set; }
    public required string Description { get; set; }
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "The name describes the role-to-permission association.")]
public sealed class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}

public sealed class Workspace : SoftDeletableEntity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Lifecycle { get; set; }
    public string? RetrievalProfile { get; set; }
    public int RetrievalCount { get; set; }
}

public sealed class WorkspaceRoleGrant : Entity
{
    public Guid WorkspaceId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid GrantedByUserId { get; set; }
    public DateTimeOffset GrantedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
}

public sealed class Document : SoftDeletableEntity
{
    public Guid WorkspaceId { get; set; }
    public required string Title { get; set; }
    public required string Kind { get; set; }
    public Guid OwnerUserId { get; set; }
    public Guid? CurrentVersionId { get; set; }
}

public sealed class DocumentVersion : AuditableEntity
{
    public Guid DocumentId { get; set; }
    public int VersionNumber { get; set; }
    public required string Status { get; set; }
    public string? SourceUri { get; set; }
    public string? BlobKey { get; set; }
    public required string ContentHash { get; set; }
    public string? MimeType { get; set; }
    public string? SourceMetadata { get; set; }
    public Guid CreatedByUserId { get; set; }
}

public sealed class DocumentChunk : Entity
{
    public Guid DocumentVersionId { get; set; }
    public int Ordinal { get; set; }
    public required string Text { get; set; }
    public int? PageNumber { get; set; }
    public string? SectionLabel { get; set; }
    public int? CharStart { get; set; }
    public int? CharEnd { get; set; }
    public required string ContentHash { get; set; }
}

public sealed class ChunkEmbedding : Entity
{
    public Guid DocumentChunkId { get; set; }
    public required string Model { get; set; }
    public int Dimensions { get; set; }
    public required string Embedding { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class IngestionJob : AuditableEntity
{
    public Guid DocumentVersionId { get; set; }
    public required string Kind { get; set; }
    public required string Status { get; set; }
    public required string IdempotencyKey { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorDetail { get; set; }
}

public sealed class IngestionJobAttempt : Entity
{
    public Guid IngestionJobId { get; set; }
    public int AttemptNumber { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public required string Outcome { get; set; }
    public string? Detail { get; set; }
}

public sealed class ChatSession : SoftDeletableEntity
{
    public Guid UserId { get; set; }
    public string? Title { get; set; }
}

public sealed class ChatMessage : Entity
{
    public Guid ChatSessionId { get; set; }
    public int SequenceNumber { get; set; }
    public required string Role { get; set; }
    public string? Content { get; set; }
    public required string RetentionState { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class MessageCitation : Entity
{
    public Guid ChatMessageId { get; set; }
    public Guid DocumentChunkId { get; set; }
    public int Ordinal { get; set; }
    public string? Excerpt { get; set; }
    public double? RetrievalScore { get; set; }
}

public sealed class WorkspaceNote : AuditableEntity
{
    public Guid WorkspaceId { get; set; }
    public Guid UserId { get; set; }
    public required string Content { get; set; }
}

public sealed class NoteMessageLink
{
    public Guid WorkspaceNoteId { get; set; }
    public Guid ChatMessageId { get; set; }
}

public sealed class ContentJob : AuditableEntity
{
    public Guid WorkspaceId { get; set; }
    public required string Name { get; set; }
    public required string Schedule { get; set; }
    public required string Status { get; set; }
    public required string SourceType { get; set; }
    public required string SourceConfig { get; set; }
    public Guid CreatedByUserId { get; set; }
}

public sealed class ContentJobSource : Entity
{
    public Guid ContentJobId { get; set; }
    public required string SourceUri { get; set; }
    public string? SourceMetadata { get; set; }
    public bool Enabled { get; set; }
}

public sealed class ContentJobRun : Entity
{
    public Guid ContentJobId { get; set; }
    public Guid? IngestionJobId { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public required string Status { get; set; }
    public required string Trigger { get; set; }
    public string? Detail { get; set; }
}

public sealed class AuditEvent : Entity
{
    public DateTimeOffset OccurredAt { get; set; }
    public Guid? ActorUserId { get; set; }
    public required string Action { get; set; }
    public required string Outcome { get; set; }
    public Guid CorrelationId { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? Metadata { get; set; }
}
