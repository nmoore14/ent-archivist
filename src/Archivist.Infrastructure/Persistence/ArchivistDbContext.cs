using Archivist.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Archivist.Infrastructure.Persistence;

public sealed class ArchivistDbContext(DbContextOptions<ArchivistDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<ExternalIdentity> ExternalIdentities => Set<ExternalIdentity>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<WorkspaceRoleGrant> WorkspaceRoleGrants => Set<WorkspaceRoleGrant>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();
    public DbSet<ChunkEmbedding> ChunkEmbeddings => Set<ChunkEmbedding>();
    public DbSet<IngestionJob> IngestionJobs => Set<IngestionJob>();
    public DbSet<IngestionJobAttempt> IngestionJobAttempts => Set<IngestionJobAttempt>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<MessageCitation> MessageCitations => Set<MessageCitation>();
    public DbSet<WorkspaceNote> WorkspaceNotes => Set<WorkspaceNote>();
    public DbSet<NoteMessageLink> NoteMessageLinks => Set<NoteMessageLink>();
    public DbSet<ContentJob> ContentJobs => Set<ContentJob>();
    public DbSet<ContentJobSource> ContentJobSources => Set<ContentJobSource>();
    public DbSet<ContentJobRun> ContentJobRuns => Set<ContentJobRun>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");

        ConfigureIdentity(modelBuilder);
        ConfigureKnowledge(modelBuilder);
        ConfigureConversation(modelBuilder);
        ConfigureContentJobs(modelBuilder);
        ConfigureAudit(modelBuilder);
        ApplyRelationalConventions(modelBuilder);
    }

    private static void ConfigureIdentity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            ConfigureAuditable(entity);
            entity.Property(x => x.DisplayName).HasMaxLength(256);
            entity.Property(x => x.Email).HasMaxLength(320);
            entity.Property(x => x.Status).HasMaxLength(32);
            entity.ToTable(table => table.HasCheckConstraint("ck_users_status", "[status] IN ('active', 'disabled')"));
            entity.HasIndex(x => x.Email).IsUnique().HasFilter("[email] IS NOT NULL");
            entity.HasIndex(x => x.Status).HasDatabaseName("ix_users_status");
        });

        modelBuilder.Entity<ExternalIdentity>(entity =>
        {
            entity.Property(x => x.Provider).HasMaxLength(64);
            entity.Property(x => x.Issuer).HasMaxLength(512);
            entity.Property(x => x.TenantId).HasMaxLength(128);
            entity.Property(x => x.SubjectId).HasMaxLength(256);
            entity.HasIndex(x => new { x.Provider, x.Issuer, x.SubjectId }).IsUnique();
            entity.HasIndex(x => x.UserId);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(128);
            entity.Property(x => x.Name).HasMaxLength(256);
            entity.Property(x => x.Description).HasMaxLength(1024);
            entity.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.Property(x => x.Code).HasMaxLength(128);
            entity.Property(x => x.Description).HasMaxLength(1024);
            entity.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(x => new { x.RoleId, x.PermissionId });
            entity.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Permission>().WithMany().HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureKnowledge(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Workspace>(entity =>
        {
            ConfigureSoftDeletable(entity);
            entity.Property(x => x.Code).HasMaxLength(128);
            entity.Property(x => x.Name).HasMaxLength(256);
            entity.Property(x => x.Lifecycle).HasMaxLength(32);
            entity.Property(x => x.RetrievalProfile).HasMaxLength(128);
            entity.ToTable(table => table.HasCheckConstraint("ck_workspaces_lifecycle", "[lifecycle] IN ('draft', 'published', 'archived')"));
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => new { x.Lifecycle, x.Code });
        });

        modelBuilder.Entity<WorkspaceRoleGrant>(entity =>
        {
            entity.HasOne<Workspace>().WithMany().HasForeignKey(x => x.WorkspaceId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.GrantedByUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.WorkspaceId, x.UserId, x.RoleId }).IsUnique().HasFilter("[revoked_at] IS NULL");
            entity.HasIndex(x => new { x.UserId, x.RevokedAt });
        });

        modelBuilder.Entity<Document>(entity =>
        {
            ConfigureSoftDeletable(entity);
            entity.Property(x => x.Title).HasMaxLength(512);
            entity.Property(x => x.Kind).HasMaxLength(64);
            entity.HasOne<Workspace>().WithMany().HasForeignKey(x => x.WorkspaceId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.OwnerUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<DocumentVersion>().WithMany().HasForeignKey(x => x.CurrentVersionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.WorkspaceId, x.DeletedAt, x.Title });
        });

        modelBuilder.Entity<DocumentVersion>(entity =>
        {
            ConfigureAuditable(entity);
            entity.Property(x => x.Status).HasMaxLength(32);
            entity.Property(x => x.SourceUri).HasMaxLength(2048);
            entity.Property(x => x.BlobKey).HasMaxLength(1024);
            entity.Property(x => x.ContentHash).HasMaxLength(128);
            entity.Property(x => x.MimeType).HasMaxLength(256);
            entity.Property(x => x.SourceMetadata).HasColumnType("nvarchar(max)");
            entity.ToTable(table => table.HasCheckConstraint("ck_document_versions_status", "[status] IN ('pending', 'processing', 'ready', 'failed', 'superseded', 'deleted')"));
            entity.ToTable(table => table.HasCheckConstraint("ck_document_versions_source_metadata_json", "[source_metadata] IS NULL OR ISJSON([source_metadata]) = 1"));
            entity.HasOne<Document>().WithMany().HasForeignKey(x => x.DocumentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.DocumentId, x.VersionNumber }).IsUnique();
            entity.HasIndex(x => new { x.DocumentId, x.ContentHash }).IsUnique();
            entity.HasIndex(x => x.Status).HasFilter("[status] = 'ready'");
        });

        modelBuilder.Entity<DocumentChunk>(entity =>
        {
            entity.Property(x => x.Text).HasColumnType("nvarchar(max)");
            entity.Property(x => x.SectionLabel).HasMaxLength(512);
            entity.Property(x => x.ContentHash).HasMaxLength(128);
            entity.HasOne<DocumentVersion>().WithMany().HasForeignKey(x => x.DocumentVersionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.DocumentVersionId, x.Ordinal }).IsUnique();
        });

        modelBuilder.Entity<ChunkEmbedding>(entity =>
        {
            entity.Property(x => x.Model).HasMaxLength(256);
            // EF Core 10 does not yet expose a native SQL Server vector mapping. The
            // migration promotes this generated nvarchar column to vector(1536);
            // SQL Server transparently presents vectors as JSON to this string property.
            entity.Property(x => x.Embedding).HasColumnType("nvarchar(max)");
            entity.HasOne<DocumentChunk>().WithMany().HasForeignKey(x => x.DocumentChunkId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.DocumentChunkId, x.Model }).IsUnique();
        });

        modelBuilder.Entity<IngestionJob>(entity =>
        {
            ConfigureAuditable(entity);
            entity.Property(x => x.Kind).HasMaxLength(64);
            entity.Property(x => x.Status).HasMaxLength(32);
            entity.Property(x => x.IdempotencyKey).HasMaxLength(256);
            entity.Property(x => x.ErrorCode).HasMaxLength(128);
            entity.Property(x => x.ErrorDetail).HasColumnType("nvarchar(max)");
            entity.ToTable(table => table.HasCheckConstraint("ck_ingestion_jobs_status", "[status] IN ('queued', 'running', 'succeeded', 'failed', 'cancelled')"));
            entity.HasOne<DocumentVersion>().WithMany().HasForeignKey(x => x.DocumentVersionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => x.IdempotencyKey).IsUnique();
            entity.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<IngestionJobAttempt>(entity =>
        {
            entity.Property(x => x.Outcome).HasMaxLength(32);
            entity.Property(x => x.Detail).HasColumnType("nvarchar(max)");
            entity.HasOne<IngestionJob>().WithMany().HasForeignKey(x => x.IngestionJobId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.IngestionJobId, x.AttemptNumber }).IsUnique();
        });
    }

    private static void ConfigureConversation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatSession>(entity =>
        {
            ConfigureSoftDeletable(entity);
            entity.Property(x => x.Title).HasMaxLength(512);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.UserId, x.UpdatedAt }).IsDescending(false, true);
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.Property(x => x.Role).HasMaxLength(16);
            entity.Property(x => x.Content).HasColumnType("nvarchar(max)");
            entity.Property(x => x.RetentionState).HasMaxLength(16);
            entity.ToTable(table => table.HasCheckConstraint("ck_chat_messages_role", "[role] IN ('user', 'assistant', 'system')"));
            entity.ToTable(table => table.HasCheckConstraint("ck_chat_messages_retention_state", "[retention_state] IN ('stored', 'redacted', 'excluded')"));
            entity.HasOne<ChatSession>().WithMany().HasForeignKey(x => x.ChatSessionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.ChatSessionId, x.SequenceNumber }).IsUnique();
        });

        modelBuilder.Entity<MessageCitation>(entity =>
        {
            entity.Property(x => x.Excerpt).HasColumnType("nvarchar(max)");
            entity.HasOne<ChatMessage>().WithMany().HasForeignKey(x => x.ChatMessageId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<DocumentChunk>().WithMany().HasForeignKey(x => x.DocumentChunkId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.ChatMessageId, x.Ordinal }).IsUnique();
        });

        modelBuilder.Entity<WorkspaceNote>(entity =>
        {
            ConfigureAuditable(entity);
            entity.Property(x => x.Content).HasColumnType("nvarchar(max)");
            entity.HasOne<Workspace>().WithMany().HasForeignKey(x => x.WorkspaceId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.WorkspaceId, x.UserId }).IsUnique();
        });

        modelBuilder.Entity<NoteMessageLink>(entity =>
        {
            entity.HasKey(x => new { x.WorkspaceNoteId, x.ChatMessageId });
            entity.HasOne<WorkspaceNote>().WithMany().HasForeignKey(x => x.WorkspaceNoteId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ChatMessage>().WithMany().HasForeignKey(x => x.ChatMessageId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureContentJobs(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContentJob>(entity =>
        {
            ConfigureAuditable(entity);
            entity.Property(x => x.Name).HasMaxLength(256);
            entity.Property(x => x.Schedule).HasMaxLength(256);
            entity.Property(x => x.Status).HasMaxLength(32);
            entity.Property(x => x.SourceType).HasMaxLength(64);
            entity.Property(x => x.SourceConfig).HasColumnType("nvarchar(max)");
            entity.ToTable(table => table.HasCheckConstraint("ck_content_jobs_status", "[status] IN ('queued', 'running', 'succeeded', 'failed', 'cancelled')"));
            entity.ToTable(table => table.HasCheckConstraint("ck_content_jobs_source_config_json", "ISJSON([source_config]) = 1"));
            entity.HasOne<Workspace>().WithMany().HasForeignKey(x => x.WorkspaceId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.WorkspaceId, x.Name }).IsUnique();
            entity.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<ContentJobSource>(entity =>
        {
            entity.Property(x => x.SourceUri).HasMaxLength(2048);
            entity.Property(x => x.SourceMetadata).HasColumnType("nvarchar(max)");
            entity.ToTable(table => table.HasCheckConstraint("ck_content_job_sources_source_metadata_json", "[source_metadata] IS NULL OR ISJSON([source_metadata]) = 1"));
            entity.HasOne<ContentJob>().WithMany().HasForeignKey(x => x.ContentJobId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.ContentJobId, x.SourceUri }).IsUnique();
        });

        modelBuilder.Entity<ContentJobRun>(entity =>
        {
            entity.Property(x => x.Status).HasMaxLength(32);
            entity.Property(x => x.Trigger).HasMaxLength(32);
            entity.Property(x => x.Detail).HasColumnType("nvarchar(max)");
            entity.ToTable(table => table.HasCheckConstraint("ck_content_job_runs_status", "[status] IN ('queued', 'running', 'succeeded', 'failed', 'cancelled')"));
            entity.HasOne<ContentJob>().WithMany().HasForeignKey(x => x.ContentJobId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<IngestionJob>().WithMany().HasForeignKey(x => x.IngestionJobId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.ContentJobId, x.StartedAt }).IsDescending(false, true);
        });
    }

    private static void ConfigureAudit(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditEvent>(entity =>
        {
            entity.Property(x => x.Action).HasMaxLength(128);
            entity.Property(x => x.Outcome).HasMaxLength(32);
            entity.Property(x => x.EntityType).HasMaxLength(128);
            entity.Property(x => x.Metadata).HasColumnType("nvarchar(max)");
            entity.ToTable(table => table.HasCheckConstraint("ck_audit_events_metadata_json", "[metadata] IS NULL OR ISJSON([metadata]) = 1"));
            entity.HasOne<User>().WithMany().HasForeignKey(x => x.ActorUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => x.OccurredAt).IsDescending();
            entity.HasIndex(x => new { x.ActorUserId, x.OccurredAt }).IsDescending(false, true);
            entity.HasIndex(x => new { x.EntityType, x.EntityId });
        });
    }

    private static void ConfigureAuditable<TEntity>(EntityTypeBuilder<TEntity> entity)
        where TEntity : AuditableEntity
    {
        entity.Property(x => x.CreatedAt).HasPrecision(7);
        entity.Property(x => x.UpdatedAt).HasPrecision(7);
    }

    private static void ConfigureSoftDeletable<TEntity>(EntityTypeBuilder<TEntity> entity)
        where TEntity : SoftDeletableEntity
    {
        ConfigureAuditable(entity);
        entity.Property(x => x.DeletedAt).HasPrecision(7);
    }

    private static void ApplyRelationalConventions(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.SetTableName(ToSnakeCase(entityType.GetTableName() ?? entityType.ClrType.Name));

            foreach (var property in entityType.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }
        }
    }

    private static string ToSnakeCase(string value)
    {
        return string.Concat(value.Select((character, index) =>
            index > 0 && char.IsUpper(character)
                ? $"_{char.ToLowerInvariant(character)}"
                : char.ToLowerInvariant(character).ToString()));
    }
}
