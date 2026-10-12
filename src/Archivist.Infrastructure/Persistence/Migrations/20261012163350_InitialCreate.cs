using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1825, CA1861 // EF-generated migration array arguments.

namespace Archivist.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "permissions",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    is_system = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    display_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.CheckConstraint("ck_users_status", "[status] IN ('active', 'disabled')");
                });

            migrationBuilder.CreateTable(
                name: "workspaces",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lifecycle = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    retrieval_profile = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    retrieval_count = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspaces", x => x.id);
                    table.CheckConstraint("ck_workspaces_lifecycle", "[lifecycle] IN ('draft', 'published', 'archived')");
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                schema: "dbo",
                columns: table => new
                {
                    role_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    permission_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permissions", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "FK_role_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalSchema: "dbo",
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "dbo",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "audit_events",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    actor_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    action = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    outcome = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    correlation_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    entity_type = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    entity_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_events", x => x.id);
                    table.CheckConstraint("ck_audit_events_metadata_json", "[metadata] IS NULL OR ISJSON([metadata]) = 1");
                    table.ForeignKey(
                        name: "FK_audit_events_users_actor_user_id",
                        column: x => x.actor_user_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chat_sessions",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_chat_sessions_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "external_identities",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    provider = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    issuer = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    tenant_id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    subject_id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_external_identities", x => x.id);
                    table.ForeignKey(
                        name: "FK_external_identities_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "content_jobs",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    schedule = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    source_type = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    source_config = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_content_jobs", x => x.id);
                    table.CheckConstraint("ck_content_jobs_source_config_json", "ISJSON([source_config]) = 1");
                    table.CheckConstraint("ck_content_jobs_status", "[status] IN ('queued', 'running', 'succeeded', 'failed', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_content_jobs_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_content_jobs_workspaces_workspace_id",
                        column: x => x.workspace_id,
                        principalSchema: "dbo",
                        principalTable: "workspaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "workspace_notes",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_notes", x => x.id);
                    table.ForeignKey(
                        name: "FK_workspace_notes_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_workspace_notes_workspaces_workspace_id",
                        column: x => x.workspace_id,
                        principalSchema: "dbo",
                        principalTable: "workspaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "workspace_role_grants",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    role_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    granted_by_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    granted_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_role_grants", x => x.id);
                    table.ForeignKey(
                        name: "FK_workspace_role_grants_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "dbo",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_workspace_role_grants_users_granted_by_user_id",
                        column: x => x.granted_by_user_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_workspace_role_grants_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_workspace_role_grants_workspaces_workspace_id",
                        column: x => x.workspace_id,
                        principalSchema: "dbo",
                        principalTable: "workspaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chat_messages",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    chat_session_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sequence_number = table.Column<int>(type: "int", nullable: false),
                    role = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    retention_state = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_messages", x => x.id);
                    table.CheckConstraint("ck_chat_messages_retention_state", "[retention_state] IN ('stored', 'redacted', 'excluded')");
                    table.CheckConstraint("ck_chat_messages_role", "[role] IN ('user', 'assistant', 'system')");
                    table.ForeignKey(
                        name: "FK_chat_messages_chat_sessions_chat_session_id",
                        column: x => x.chat_session_id,
                        principalSchema: "dbo",
                        principalTable: "chat_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "content_job_sources",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    content_job_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    source_uri = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    source_metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_content_job_sources", x => x.id);
                    table.CheckConstraint("ck_content_job_sources_source_metadata_json", "[source_metadata] IS NULL OR ISJSON([source_metadata]) = 1");
                    table.ForeignKey(
                        name: "FK_content_job_sources_content_jobs_content_job_id",
                        column: x => x.content_job_id,
                        principalSchema: "dbo",
                        principalTable: "content_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "note_message_links",
                schema: "dbo",
                columns: table => new
                {
                    workspace_note_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    chat_message_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_note_message_links", x => new { x.workspace_note_id, x.chat_message_id });
                    table.ForeignKey(
                        name: "FK_note_message_links_chat_messages_chat_message_id",
                        column: x => x.chat_message_id,
                        principalSchema: "dbo",
                        principalTable: "chat_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_note_message_links_workspace_notes_workspace_note_id",
                        column: x => x.workspace_note_id,
                        principalSchema: "dbo",
                        principalTable: "workspace_notes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chunk_embeddings",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    document_chunk_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    model = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    dimensions = table.Column<int>(type: "int", nullable: false),
                    embedding = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chunk_embeddings", x => x.id);
                });

            // EF Core currently represents the vector as JSON text in the model.
            // SQL Server 2025 accepts that representation while storing the column
            // in its native vector format for VECTOR_DISTANCE queries.
            migrationBuilder.Sql("ALTER TABLE [dbo].[chunk_embeddings] ALTER COLUMN [embedding] vector(1536) NOT NULL;");

            migrationBuilder.CreateTable(
                name: "content_job_runs",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    content_job_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ingestion_job_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    started_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    finished_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    trigger = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    detail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_content_job_runs", x => x.id);
                    table.CheckConstraint("ck_content_job_runs_status", "[status] IN ('queued', 'running', 'succeeded', 'failed', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_content_job_runs_content_jobs_content_job_id",
                        column: x => x.content_job_id,
                        principalSchema: "dbo",
                        principalTable: "content_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "document_chunks",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    document_version_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ordinal = table.Column<int>(type: "int", nullable: false),
                    text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    page_number = table.Column<int>(type: "int", nullable: true),
                    section_label = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    char_start = table.Column<int>(type: "int", nullable: true),
                    char_end = table.Column<int>(type: "int", nullable: true),
                    content_hash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_chunks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "message_citations",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    chat_message_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    document_chunk_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ordinal = table.Column<int>(type: "int", nullable: false),
                    excerpt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    retrieval_score = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message_citations", x => x.id);
                    table.ForeignKey(
                        name: "FK_message_citations_chat_messages_chat_message_id",
                        column: x => x.chat_message_id,
                        principalSchema: "dbo",
                        principalTable: "chat_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_message_citations_document_chunks_document_chunk_id",
                        column: x => x.document_chunk_id,
                        principalSchema: "dbo",
                        principalTable: "document_chunks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "document_versions",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    document_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    version_number = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    source_uri = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    blob_key = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    content_hash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    mime_type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    source_metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_versions", x => x.id);
                    table.CheckConstraint("ck_document_versions_source_metadata_json", "[source_metadata] IS NULL OR ISJSON([source_metadata]) = 1");
                    table.CheckConstraint("ck_document_versions_status", "[status] IN ('pending', 'processing', 'ready', 'failed', 'superseded', 'deleted')");
                    table.ForeignKey(
                        name: "FK_document_versions_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "documents",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    kind = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    owner_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    current_version_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documents", x => x.id);
                    table.ForeignKey(
                        name: "FK_documents_document_versions_current_version_id",
                        column: x => x.current_version_id,
                        principalSchema: "dbo",
                        principalTable: "document_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_documents_users_owner_user_id",
                        column: x => x.owner_user_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_documents_workspaces_workspace_id",
                        column: x => x.workspace_id,
                        principalSchema: "dbo",
                        principalTable: "workspaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ingestion_jobs",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    document_version_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    kind = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    idempotency_key = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    error_code = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    error_detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", precision: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingestion_jobs", x => x.id);
                    table.CheckConstraint("ck_ingestion_jobs_status", "[status] IN ('queued', 'running', 'succeeded', 'failed', 'cancelled')");
                    table.ForeignKey(
                        name: "FK_ingestion_jobs_document_versions_document_version_id",
                        column: x => x.document_version_id,
                        principalSchema: "dbo",
                        principalTable: "document_versions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ingestion_job_attempts",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ingestion_job_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    attempt_number = table.Column<int>(type: "int", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    finished_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    outcome = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    detail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingestion_job_attempts", x => x.id);
                    table.ForeignKey(
                        name: "FK_ingestion_job_attempts_ingestion_jobs_ingestion_job_id",
                        column: x => x.ingestion_job_id,
                        principalSchema: "dbo",
                        principalTable: "ingestion_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_events_actor_user_id_occurred_at",
                schema: "dbo",
                table: "audit_events",
                columns: new[] { "actor_user_id", "occurred_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_audit_events_entity_type_entity_id",
                schema: "dbo",
                table: "audit_events",
                columns: new[] { "entity_type", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_events_occurred_at",
                schema: "dbo",
                table: "audit_events",
                column: "occurred_at",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_chat_session_id_sequence_number",
                schema: "dbo",
                table: "chat_messages",
                columns: new[] { "chat_session_id", "sequence_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_chat_sessions_user_id_updated_at",
                schema: "dbo",
                table: "chat_sessions",
                columns: new[] { "user_id", "updated_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_chunk_embeddings_document_chunk_id_model",
                schema: "dbo",
                table: "chunk_embeddings",
                columns: new[] { "document_chunk_id", "model" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_content_job_runs_content_job_id_started_at",
                schema: "dbo",
                table: "content_job_runs",
                columns: new[] { "content_job_id", "started_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_content_job_runs_ingestion_job_id",
                schema: "dbo",
                table: "content_job_runs",
                column: "ingestion_job_id");

            migrationBuilder.CreateIndex(
                name: "IX_content_job_sources_content_job_id_source_uri",
                schema: "dbo",
                table: "content_job_sources",
                columns: new[] { "content_job_id", "source_uri" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_content_jobs_created_by_user_id",
                schema: "dbo",
                table: "content_jobs",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_content_jobs_status",
                schema: "dbo",
                table: "content_jobs",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_content_jobs_workspace_id_name",
                schema: "dbo",
                table: "content_jobs",
                columns: new[] { "workspace_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_document_chunks_document_version_id_ordinal",
                schema: "dbo",
                table: "document_chunks",
                columns: new[] { "document_version_id", "ordinal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_document_versions_created_by_user_id",
                schema: "dbo",
                table: "document_versions",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_document_versions_document_id_content_hash",
                schema: "dbo",
                table: "document_versions",
                columns: new[] { "document_id", "content_hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_document_versions_document_id_version_number",
                schema: "dbo",
                table: "document_versions",
                columns: new[] { "document_id", "version_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_document_versions_status",
                schema: "dbo",
                table: "document_versions",
                column: "status",
                filter: "[status] = 'ready'");

            migrationBuilder.CreateIndex(
                name: "IX_documents_current_version_id",
                schema: "dbo",
                table: "documents",
                column: "current_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_documents_owner_user_id",
                schema: "dbo",
                table: "documents",
                column: "owner_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_documents_workspace_id_deleted_at_title",
                schema: "dbo",
                table: "documents",
                columns: new[] { "workspace_id", "deleted_at", "title" });

            migrationBuilder.CreateIndex(
                name: "IX_external_identities_provider_issuer_subject_id",
                schema: "dbo",
                table: "external_identities",
                columns: new[] { "provider", "issuer", "subject_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_external_identities_user_id",
                schema: "dbo",
                table: "external_identities",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingestion_job_attempts_ingestion_job_id_attempt_number",
                schema: "dbo",
                table: "ingestion_job_attempts",
                columns: new[] { "ingestion_job_id", "attempt_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ingestion_jobs_document_version_id",
                schema: "dbo",
                table: "ingestion_jobs",
                column: "document_version_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingestion_jobs_idempotency_key",
                schema: "dbo",
                table: "ingestion_jobs",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ingestion_jobs_status",
                schema: "dbo",
                table: "ingestion_jobs",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_message_citations_chat_message_id_ordinal",
                schema: "dbo",
                table: "message_citations",
                columns: new[] { "chat_message_id", "ordinal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_message_citations_document_chunk_id",
                schema: "dbo",
                table: "message_citations",
                column: "document_chunk_id");

            migrationBuilder.CreateIndex(
                name: "IX_note_message_links_chat_message_id",
                schema: "dbo",
                table: "note_message_links",
                column: "chat_message_id");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_code",
                schema: "dbo",
                table: "permissions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_permission_id",
                schema: "dbo",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_code",
                schema: "dbo",
                table: "roles",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                schema: "dbo",
                table: "users",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_users_status",
                schema: "dbo",
                table: "users",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_notes_user_id",
                schema: "dbo",
                table: "workspace_notes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_notes_workspace_id_user_id",
                schema: "dbo",
                table: "workspace_notes",
                columns: new[] { "workspace_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_workspace_role_grants_granted_by_user_id",
                schema: "dbo",
                table: "workspace_role_grants",
                column: "granted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_role_grants_role_id",
                schema: "dbo",
                table: "workspace_role_grants",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_role_grants_user_id_revoked_at",
                schema: "dbo",
                table: "workspace_role_grants",
                columns: new[] { "user_id", "revoked_at" });

            migrationBuilder.CreateIndex(
                name: "IX_workspace_role_grants_workspace_id_user_id_role_id",
                schema: "dbo",
                table: "workspace_role_grants",
                columns: new[] { "workspace_id", "user_id", "role_id" },
                unique: true,
                filter: "[revoked_at] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_workspaces_code",
                schema: "dbo",
                table: "workspaces",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_workspaces_lifecycle_code",
                schema: "dbo",
                table: "workspaces",
                columns: new[] { "lifecycle", "code" });

            migrationBuilder.AddForeignKey(
                name: "FK_chunk_embeddings_document_chunks_document_chunk_id",
                schema: "dbo",
                table: "chunk_embeddings",
                column: "document_chunk_id",
                principalSchema: "dbo",
                principalTable: "document_chunks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_content_job_runs_ingestion_jobs_ingestion_job_id",
                schema: "dbo",
                table: "content_job_runs",
                column: "ingestion_job_id",
                principalSchema: "dbo",
                principalTable: "ingestion_jobs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_document_chunks_document_versions_document_version_id",
                schema: "dbo",
                table: "document_chunks",
                column: "document_version_id",
                principalSchema: "dbo",
                principalTable: "document_versions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_document_versions_documents_document_id",
                schema: "dbo",
                table: "document_versions",
                column: "document_id",
                principalSchema: "dbo",
                principalTable: "documents",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_document_versions_users_created_by_user_id",
                schema: "dbo",
                table: "document_versions");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_users_owner_user_id",
                schema: "dbo",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_workspaces_workspace_id",
                schema: "dbo",
                table: "documents");

            migrationBuilder.DropForeignKey(
                name: "FK_documents_document_versions_current_version_id",
                schema: "dbo",
                table: "documents");

            migrationBuilder.DropTable(
                name: "audit_events",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "chunk_embeddings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "content_job_runs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "content_job_sources",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "external_identities",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ingestion_job_attempts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "message_citations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "note_message_links",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "role_permissions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "workspace_role_grants",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "content_jobs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ingestion_jobs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "document_chunks",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "chat_messages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "workspace_notes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "permissions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "chat_sessions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "users",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "workspaces",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "document_versions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "documents",
                schema: "dbo");
        }
    }
}

#pragma warning restore CA1825, CA1861
