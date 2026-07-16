using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace WorkPulse.Infrastructure.Persistence.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260703181249_CollaborationAnalyticsSearchFiles")]
public class CollaborationAnalyticsSearchFiles : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable("notifications", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> user_id = table.Column<Guid>("uuid");
			int? maxLength = 50;
			OperationBuilder<AddColumnOperation> type = table.Column<string>("character varying(50)", null, maxLength);
			maxLength = 200;
			OperationBuilder<AddColumnOperation> title = table.Column<string>("character varying(200)", null, maxLength);
			maxLength = 2000;
			OperationBuilder<AddColumnOperation> body = table.Column<string>("character varying(2000)", null, maxLength);
			OperationBuilder<AddColumnOperation> is_read = table.Column<bool>("boolean");
			maxLength = 50;
			return new
			{
				id = id,
				user_id = user_id,
				type = type,
				title = title,
				body = body,
				is_read = is_read,
				related_entity_type = table.Column<string>("character varying(50)", null, maxLength, rowVersion: false, null, nullable: true),
				related_entity_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				actor_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				created_at_utc = table.Column<DateTime>("timestamp with time zone"),
				created_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				updated_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true),
				updated_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				tenant_id = table.Column<Guid>("uuid"),
				is_deleted = table.Column<bool>("boolean"),
				deleted_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true),
				deleted_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("p_k_notifications", x => x.id);
		});
		migrationBuilder.CreateTable("stored_files", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			int? maxLength = 255;
			OperationBuilder<AddColumnOperation> file_name = table.Column<string>("character varying(255)", null, maxLength);
			maxLength = 100;
			OperationBuilder<AddColumnOperation> content_type = table.Column<string>("character varying(100)", null, maxLength);
			OperationBuilder<AddColumnOperation> size_bytes = table.Column<long>("bigint");
			maxLength = 500;
			OperationBuilder<AddColumnOperation> storage_key = table.Column<string>("character varying(500)", null, maxLength);
			OperationBuilder<AddColumnOperation> uploaded_by_id = table.Column<Guid>("uuid");
			maxLength = 50;
			return new
			{
				id = id,
				file_name = file_name,
				content_type = content_type,
				size_bytes = size_bytes,
				storage_key = storage_key,
				uploaded_by_id = uploaded_by_id,
				entity_type = table.Column<string>("character varying(50)", null, maxLength),
				entity_id = table.Column<Guid>("uuid"),
				created_at_utc = table.Column<DateTime>("timestamp with time zone"),
				created_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				updated_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true),
				updated_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				tenant_id = table.Column<Guid>("uuid"),
				is_deleted = table.Column<bool>("boolean"),
				deleted_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true),
				deleted_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("p_k_stored_files", x => x.id);
		});
		migrationBuilder.CreateTable("task_activities", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> task_id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> actor_id = table.Column<Guid>("uuid");
			int? maxLength = 50;
			OperationBuilder<AddColumnOperation> type = table.Column<string>("character varying(50)", null, maxLength);
			maxLength = 500;
			return new
			{
				id = id,
				task_id = task_id,
				actor_id = actor_id,
				type = type,
				summary = table.Column<string>("character varying(500)", null, maxLength, rowVersion: false, null, nullable: true),
				metadata_json = table.Column<string>("jsonb", null, null, rowVersion: false, null, nullable: true),
				created_at_utc = table.Column<DateTime>("timestamp with time zone"),
				created_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				updated_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true),
				updated_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				tenant_id = table.Column<Guid>("uuid"),
				is_deleted = table.Column<bool>("boolean"),
				deleted_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true),
				deleted_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("p_k_task_activities", x => x.id);
		});
		migrationBuilder.CreateTable("task_comments", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> task_id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> author_id = table.Column<Guid>("uuid");
			int? maxLength = 10000;
			return new
			{
				id = id,
				task_id = task_id,
				author_id = author_id,
				body = table.Column<string>("character varying(10000)", null, maxLength),
				mentioned_user_ids = table.Column<string>("jsonb"),
				is_edited = table.Column<bool>("boolean"),
				created_at_utc = table.Column<DateTime>("timestamp with time zone"),
				created_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				updated_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true),
				updated_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				tenant_id = table.Column<Guid>("uuid"),
				is_deleted = table.Column<bool>("boolean"),
				deleted_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true),
				deleted_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("p_k_task_comments", x => x.id);
		});
		migrationBuilder.CreateTable("task_watchers", (ColumnsBuilder table) => new
		{
			id = table.Column<Guid>("uuid"),
			task_id = table.Column<Guid>("uuid"),
			user_id = table.Column<Guid>("uuid"),
			created_at_utc = table.Column<DateTime>("timestamp with time zone"),
			created_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
			updated_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true),
			updated_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
			tenant_id = table.Column<Guid>("uuid"),
			is_deleted = table.Column<bool>("boolean"),
			deleted_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true),
			deleted_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("p_k_task_watchers", x => x.id);
		});
		migrationBuilder.CreateIndex("i_x_tasks_tenant_id_assignee_id", "tasks", new string[2] { "tenant_id", "assignee_id" });
		migrationBuilder.CreateIndex("i_x_tasks_tenant_id_due_date", "tasks", new string[2] { "tenant_id", "due_date" });
		migrationBuilder.CreateIndex("i_x_tasks_tenant_id_project_id_workflow_state_id", "tasks", new string[3] { "tenant_id", "project_id", "workflow_state_id" });
		migrationBuilder.CreateIndex("i_x_tasks_tenant_id_team_id_created_at_utc", "tasks", new string[3] { "tenant_id", "team_id", "created_at_utc" });
		migrationBuilder.CreateIndex("i_x_notifications_tenant_id_user_id", "notifications", new string[2] { "tenant_id", "user_id" });
		migrationBuilder.CreateIndex("i_x_notifications_user_id_is_read_created_at_utc", "notifications", new string[3] { "user_id", "is_read", "created_at_utc" });
		migrationBuilder.CreateIndex("i_x_stored_files_entity_type_entity_id_created_at_utc", "stored_files", new string[3] { "entity_type", "entity_id", "created_at_utc" });
		migrationBuilder.CreateIndex("i_x_stored_files_storage_key", "stored_files", "storage_key", null, unique: true);
		migrationBuilder.CreateIndex("i_x_task_activities_actor_id", "task_activities", "actor_id");
		migrationBuilder.CreateIndex("i_x_task_activities_task_id_created_at_utc", "task_activities", new string[2] { "task_id", "created_at_utc" });
		migrationBuilder.CreateIndex("i_x_task_comments_author_id", "task_comments", "author_id");
		migrationBuilder.CreateIndex("i_x_task_comments_task_id_created_at_utc", "task_comments", new string[2] { "task_id", "created_at_utc" });
		migrationBuilder.CreateIndex("i_x_task_watchers_task_id_user_id", "task_watchers", new string[2] { "task_id", "user_id" }, null, unique: true);
		migrationBuilder.CreateIndex("i_x_task_watchers_user_id", "task_watchers", "user_id");
		migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
		migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS idx_tasks_title_trgm ON tasks USING gin (title gin_trgm_ops);");
		migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS idx_tasks_description_trgm ON tasks USING gin (description gin_trgm_ops);");
		migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS idx_projects_name_trgm ON projects USING gin (name gin_trgm_ops);");
		migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS idx_teams_name_trgm ON teams USING gin (name gin_trgm_ops);");
		migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS idx_task_comments_body_trgm ON task_comments USING gin (body gin_trgm_ops);");
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable("notifications");
		migrationBuilder.DropTable("stored_files");
		migrationBuilder.DropTable("task_activities");
		migrationBuilder.DropTable("task_comments");
		migrationBuilder.DropTable("task_watchers");
		migrationBuilder.DropIndex("i_x_tasks_tenant_id_assignee_id", "tasks");
		migrationBuilder.DropIndex("i_x_tasks_tenant_id_due_date", "tasks");
		migrationBuilder.DropIndex("i_x_tasks_tenant_id_project_id_workflow_state_id", "tasks");
		migrationBuilder.DropIndex("i_x_tasks_tenant_id_team_id_created_at_utc", "tasks");
	}

	protected override void BuildTargetModel(ModelBuilder modelBuilder)
	{
		modelBuilder.HasAnnotation("ProductVersion", "10.0.0").HasAnnotation("Relational:MaxIdentifierLength", 63);
		modelBuilder.UseIdentityByDefaultColumns();
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer")
				.HasColumnName("id");
			b.Property<int>("Id").UseIdentityByDefaultColumn();
			b.Property<string>("ClaimType").HasColumnType("text").HasColumnName("claim_type");
			b.Property<string>("ClaimValue").HasColumnType("text").HasColumnName("claim_value");
			b.Property<Guid>("RoleId").HasColumnType("uuid").HasColumnName("role_id");
			b.HasKey("Id").HasName("p_k_asp_net_role_claims");
			b.HasIndex("RoleId").HasDatabaseName("i_x_asp_net_role_claims_role_id");
			b.ToTable("asp_net_role_claims", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer")
				.HasColumnName("id");
			b.Property<int>("Id").UseIdentityByDefaultColumn();
			b.Property<string>("ClaimType").HasColumnType("text").HasColumnName("claim_type");
			b.Property<string>("ClaimValue").HasColumnType("text").HasColumnName("claim_value");
			b.Property<Guid>("UserId").HasColumnType("uuid").HasColumnName("user_id");
			b.HasKey("Id").HasName("p_k_asp_net_user_claims");
			b.HasIndex("UserId").HasDatabaseName("i_x_asp_net_user_claims_user_id");
			b.ToTable("asp_net_user_claims", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.Property<string>("LoginProvider").HasColumnType("text").HasColumnName("login_provider");
			b.Property<string>("ProviderKey").HasColumnType("text").HasColumnName("provider_key");
			b.Property<string>("ProviderDisplayName").HasColumnType("text").HasColumnName("provider_display_name");
			b.Property<Guid>("UserId").HasColumnType("uuid").HasColumnName("user_id");
			b.HasKey("LoginProvider", "ProviderKey").HasName("p_k_asp_net_user_logins");
			b.HasIndex("UserId").HasDatabaseName("i_x_asp_net_user_logins_user_id");
			b.ToTable("asp_net_user_logins", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("UserId").HasColumnType("uuid").HasColumnName("user_id");
			b.Property<Guid>("RoleId").HasColumnType("uuid").HasColumnName("role_id");
			b.HasKey("UserId", "RoleId").HasName("p_k_asp_net_user_roles");
			b.HasIndex("RoleId").HasDatabaseName("i_x_asp_net_user_roles_role_id");
			b.ToTable("asp_net_user_roles", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("UserId").HasColumnType("uuid").HasColumnName("user_id");
			b.Property<string>("LoginProvider").HasColumnType("text").HasColumnName("login_provider");
			b.Property<string>("Name").HasColumnType("text").HasColumnName("name");
			b.Property<string>("Value").HasColumnType("text").HasColumnName("value");
			b.HasKey("UserId", "LoginProvider", "Name").HasName("p_k_asp_net_user_tokens");
			b.ToTable("asp_net_user_tokens", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.Company", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<string>("Description").HasMaxLength(2000).HasColumnType("character varying(2000)")
				.HasColumnName("description");
			b.Property<bool>("IsActive").HasColumnType("boolean").HasColumnName("is_active");
			b.Property<string>("LogoUrl").HasMaxLength(500).HasColumnType("character varying(500)")
				.HasColumnName("logo_url");
			b.Property<string>("Name").IsRequired().HasMaxLength(200)
				.HasColumnType("character varying(200)")
				.HasColumnName("name");
			b.Property<string>("Slug").IsRequired().HasMaxLength(200)
				.HasColumnType("character varying(200)")
				.HasColumnName("slug");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.HasKey("Id").HasName("p_k_companies");
			b.HasIndex("Slug").IsUnique().HasDatabaseName("i_x_companies_slug");
			b.ToTable("companies", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.CompanyMember", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<bool>("IsActive").HasColumnType("boolean").HasColumnName("is_active");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<string>("Role").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("role");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.Property<Guid>("UserId").HasColumnType("uuid").HasColumnName("user_id");
			b.HasKey("Id").HasName("p_k_company_members");
			b.HasIndex("UserId").HasDatabaseName("i_x_company_members_user_id");
			b.HasIndex("TenantId", "UserId").IsUnique().HasDatabaseName("i_x_company_members_tenant_id_user_id");
			b.ToTable("company_members", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.Notification", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<Guid?>("ActorId").HasColumnType("uuid").HasColumnName("actor_id");
			b.Property<string>("Body").IsRequired().HasMaxLength(2000)
				.HasColumnType("character varying(2000)")
				.HasColumnName("body");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<bool>("IsRead").HasColumnType("boolean").HasColumnName("is_read");
			b.Property<Guid?>("RelatedEntityId").HasColumnType("uuid").HasColumnName("related_entity_id");
			b.Property<string>("RelatedEntityType").HasMaxLength(50).HasColumnType("character varying(50)")
				.HasColumnName("related_entity_type");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<string>("Title").IsRequired().HasMaxLength(200)
				.HasColumnType("character varying(200)")
				.HasColumnName("title");
			b.Property<string>("Type").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("type");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.Property<Guid>("UserId").HasColumnType("uuid").HasColumnName("user_id");
			b.HasKey("Id").HasName("p_k_notifications");
			b.HasIndex("TenantId", "UserId").HasDatabaseName("i_x_notifications_tenant_id_user_id");
			b.HasIndex("UserId", "IsRead", "CreatedAtUtc").HasDatabaseName("i_x_notifications_user_id_is_read_created_at_utc");
			b.ToTable("notifications", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.Project", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<string>("Description").HasMaxLength(5000).HasColumnType("character varying(5000)")
				.HasColumnName("description");
			b.Property<bool>("IsArchived").HasColumnType("boolean").HasColumnName("is_archived");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<string>("Key").IsRequired().HasMaxLength(10)
				.HasColumnType("character varying(10)")
				.HasColumnName("key");
			b.Property<Guid?>("LeadId").HasColumnType("uuid").HasColumnName("lead_id");
			b.Property<string>("Name").IsRequired().HasMaxLength(200)
				.HasColumnType("character varying(200)")
				.HasColumnName("name");
			b.Property<DateOnly?>("StartDate").HasColumnType("date").HasColumnName("start_date");
			b.Property<string>("Status").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("status");
			b.Property<DateOnly?>("TargetDate").HasColumnType("date").HasColumnName("target_date");
			b.Property<Guid>("TeamId").HasColumnType("uuid").HasColumnName("team_id");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.HasKey("Id").HasName("p_k_projects");
			b.HasIndex("TeamId").HasDatabaseName("i_x_projects_team_id");
			b.HasIndex("TenantId", "Key").IsUnique().HasDatabaseName("i_x_projects_tenant_id_key");
			b.ToTable("projects", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.StoredFile", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<string>("ContentType").IsRequired().HasMaxLength(100)
				.HasColumnType("character varying(100)")
				.HasColumnName("content_type");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<Guid>("EntityId").HasColumnType("uuid").HasColumnName("entity_id");
			b.Property<string>("EntityType").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("entity_type");
			b.Property<string>("FileName").IsRequired().HasMaxLength(255)
				.HasColumnType("character varying(255)")
				.HasColumnName("file_name");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<long>("SizeBytes").HasColumnType("bigint").HasColumnName("size_bytes");
			b.Property<string>("StorageKey").IsRequired().HasMaxLength(500)
				.HasColumnType("character varying(500)")
				.HasColumnName("storage_key");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.Property<Guid>("UploadedById").HasColumnType("uuid").HasColumnName("uploaded_by_id");
			b.HasKey("Id").HasName("p_k_stored_files");
			b.HasIndex("StorageKey").IsUnique().HasDatabaseName("i_x_stored_files_storage_key");
			b.HasIndex("EntityType", "EntityId", "CreatedAtUtc").HasDatabaseName("i_x_stored_files_entity_type_entity_id_created_at_utc");
			b.ToTable("stored_files", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.TaskActivity", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<Guid>("ActorId").HasColumnType("uuid").HasColumnName("actor_id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<string>("MetadataJson").HasColumnType("jsonb").HasColumnName("metadata_json");
			b.Property<string>("Summary").HasMaxLength(500).HasColumnType("character varying(500)")
				.HasColumnName("summary");
			b.Property<Guid>("TaskId").HasColumnType("uuid").HasColumnName("task_id");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<string>("Type").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("type");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.HasKey("Id").HasName("p_k_task_activities");
			b.HasIndex("ActorId").HasDatabaseName("i_x_task_activities_actor_id");
			b.HasIndex("TaskId", "CreatedAtUtc").HasDatabaseName("i_x_task_activities_task_id_created_at_utc");
			b.ToTable("task_activities", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.TaskComment", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<Guid>("AuthorId").HasColumnType("uuid").HasColumnName("author_id");
			b.Property<string>("Body").IsRequired().HasMaxLength(10000)
				.HasColumnType("character varying(10000)")
				.HasColumnName("body");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<bool>("IsEdited").HasColumnType("boolean").HasColumnName("is_edited");
			b.Property<string>("MentionedUserIds").IsRequired().HasColumnType("jsonb")
				.HasColumnName("mentioned_user_ids");
			b.Property<Guid>("TaskId").HasColumnType("uuid").HasColumnName("task_id");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.HasKey("Id").HasName("p_k_task_comments");
			b.HasIndex("AuthorId").HasDatabaseName("i_x_task_comments_author_id");
			b.HasIndex("TaskId", "CreatedAtUtc").HasDatabaseName("i_x_task_comments_task_id_created_at_utc");
			b.ToTable("task_comments", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.TaskItem", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<Guid?>("AssigneeId").HasColumnType("uuid").HasColumnName("assignee_id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<Guid>("CreatorId").HasColumnType("uuid").HasColumnName("creator_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<string>("Description").HasMaxLength(10000).HasColumnType("character varying(10000)")
				.HasColumnName("description");
			b.Property<DateOnly?>("DueDate").HasColumnType("date").HasColumnName("due_date");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<int>("Number").HasColumnType("integer").HasColumnName("number");
			b.Property<Guid?>("ParentTaskId").HasColumnType("uuid").HasColumnName("parent_task_id");
			b.Property<string>("Priority").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("priority");
			b.Property<Guid?>("ProjectId").HasColumnType("uuid").HasColumnName("project_id");
			b.Property<int>("SortOrder").HasColumnType("integer").HasColumnName("sort_order");
			b.Property<Guid>("TeamId").HasColumnType("uuid").HasColumnName("team_id");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<string>("Title").IsRequired().HasMaxLength(500)
				.HasColumnType("character varying(500)")
				.HasColumnName("title");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.Property<Guid>("WorkflowStateId").HasColumnType("uuid").HasColumnName("workflow_state_id");
			b.HasKey("Id").HasName("p_k_tasks");
			b.HasIndex("AssigneeId").HasDatabaseName("i_x_tasks_assignee_id");
			b.HasIndex("ParentTaskId").HasDatabaseName("i_x_tasks_parent_task_id");
			b.HasIndex("ProjectId").HasDatabaseName("i_x_tasks_project_id");
			b.HasIndex("WorkflowStateId").HasDatabaseName("i_x_tasks_workflow_state_id");
			b.HasIndex("TeamId", "Number").IsUnique().HasDatabaseName("i_x_tasks_team_id_number");
			b.HasIndex("TenantId", "AssigneeId").HasDatabaseName("i_x_tasks_tenant_id_assignee_id");
			b.HasIndex("TenantId", "DueDate").HasDatabaseName("i_x_tasks_tenant_id_due_date");
			b.HasIndex("TenantId", "ProjectId", "WorkflowStateId").HasDatabaseName("i_x_tasks_tenant_id_project_id_workflow_state_id");
			b.HasIndex("TenantId", "TeamId", "CreatedAtUtc").HasDatabaseName("i_x_tasks_tenant_id_team_id_created_at_utc");
			b.ToTable("tasks", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.TaskWatcher", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<Guid>("TaskId").HasColumnType("uuid").HasColumnName("task_id");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.Property<Guid>("UserId").HasColumnType("uuid").HasColumnName("user_id");
			b.HasKey("Id").HasName("p_k_task_watchers");
			b.HasIndex("UserId").HasDatabaseName("i_x_task_watchers_user_id");
			b.HasIndex("TaskId", "UserId").IsUnique().HasDatabaseName("i_x_task_watchers_task_id_user_id");
			b.ToTable("task_watchers", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.Team", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<string>("Color").HasMaxLength(20).HasColumnType("character varying(20)")
				.HasColumnName("color");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<string>("Description").HasMaxLength(2000).HasColumnType("character varying(2000)")
				.HasColumnName("description");
			b.Property<string>("Icon").HasMaxLength(50).HasColumnType("character varying(50)")
				.HasColumnName("icon");
			b.Property<bool>("IsArchived").HasColumnType("boolean").HasColumnName("is_archived");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<string>("Key").IsRequired().HasMaxLength(6)
				.HasColumnType("character varying(6)")
				.HasColumnName("key");
			b.Property<string>("Name").IsRequired().HasMaxLength(200)
				.HasColumnType("character varying(200)")
				.HasColumnName("name");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.HasKey("Id").HasName("p_k_teams");
			b.HasIndex("TenantId", "Key").IsUnique().HasDatabaseName("i_x_teams_tenant_id_key");
			b.ToTable("teams", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.TeamIssueCounter", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("TeamId").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("team_id");
			b.Property<int>("LastNumber").HasColumnType("integer").HasColumnName("last_number");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.HasKey("TeamId").HasName("p_k_team_issue_counters");
			b.HasIndex("TenantId").HasDatabaseName("i_x_team_issue_counters_tenant_id");
			b.ToTable("team_issue_counters", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.TeamMember", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<string>("Role").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("role");
			b.Property<Guid>("TeamId").HasColumnType("uuid").HasColumnName("team_id");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.Property<Guid>("UserId").HasColumnType("uuid").HasColumnName("user_id");
			b.HasKey("Id").HasName("p_k_team_members");
			b.HasIndex("UserId").HasDatabaseName("i_x_team_members_user_id");
			b.HasIndex("TeamId", "UserId").IsUnique().HasDatabaseName("i_x_team_members_team_id_user_id");
			b.ToTable("team_members", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.Workflow", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<bool>("IsDefault").HasColumnType("boolean").HasColumnName("is_default");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<string>("Name").IsRequired().HasMaxLength(100)
				.HasColumnType("character varying(100)")
				.HasColumnName("name");
			b.Property<Guid>("TeamId").HasColumnType("uuid").HasColumnName("team_id");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.HasKey("Id").HasName("p_k_workflows");
			b.HasIndex("TeamId", "IsDefault").HasDatabaseName("i_x_workflows_team_id_is_default");
			b.ToTable("workflows", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.WorkflowState", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<string>("Color").IsRequired().HasMaxLength(20)
				.HasColumnType("character varying(20)")
				.HasColumnName("color");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<bool>("IsDefault").HasColumnType("boolean").HasColumnName("is_default");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<string>("Name").IsRequired().HasMaxLength(100)
				.HasColumnType("character varying(100)")
				.HasColumnName("name");
			b.Property<int>("Position").HasColumnType("integer").HasColumnName("position");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<string>("Type").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("type");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.Property<Guid>("WorkflowId").HasColumnType("uuid").HasColumnName("workflow_id");
			b.HasKey("Id").HasName("p_k_workflow_states");
			b.HasIndex("WorkflowId", "Position").HasDatabaseName("i_x_workflow_states_workflow_id_position");
			b.ToTable("workflow_states", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Infrastructure.Persistence.ApplicationRole", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<string>("ConcurrencyStamp").IsConcurrencyToken().HasColumnType("text")
				.HasColumnName("concurrency_stamp");
			b.Property<string>("Description").HasColumnType("text").HasColumnName("description");
			b.Property<string>("Name").HasMaxLength(256).HasColumnType("character varying(256)")
				.HasColumnName("name");
			b.Property<string>("NormalizedName").HasMaxLength(256).HasColumnType("character varying(256)")
				.HasColumnName("normalized_name");
			b.Property<Guid?>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.HasKey("Id").HasName("p_k_asp_net_roles");
			b.HasIndex("NormalizedName").IsUnique().HasDatabaseName("role_name_index");
			b.ToTable("asp_net_roles", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Infrastructure.Persistence.ApplicationUser", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<int>("AccessFailedCount").HasColumnType("integer").HasColumnName("access_failed_count");
			b.Property<string>("AvatarUrl").HasColumnType("text").HasColumnName("avatar_url");
			b.Property<string>("ConcurrencyStamp").IsConcurrencyToken().HasColumnType("text")
				.HasColumnName("concurrency_stamp");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CurrentTenantId").HasColumnType("uuid").HasColumnName("current_tenant_id");
			b.Property<string>("Email").HasMaxLength(256).HasColumnType("character varying(256)")
				.HasColumnName("email");
			b.Property<bool>("EmailConfirmed").HasColumnType("boolean").HasColumnName("email_confirmed");
			b.Property<string>("FirstName").IsRequired().HasColumnType("text")
				.HasColumnName("first_name");
			b.Property<bool>("IsActive").HasColumnType("boolean").HasColumnName("is_active");
			b.Property<DateTime?>("LastLoginAtUtc").HasColumnType("timestamp with time zone").HasColumnName("last_login_at_utc");
			b.Property<string>("LastName").IsRequired().HasColumnType("text")
				.HasColumnName("last_name");
			b.Property<bool>("LockoutEnabled").HasColumnType("boolean").HasColumnName("lockout_enabled");
			b.Property<DateTimeOffset?>("LockoutEnd").HasColumnType("timestamp with time zone").HasColumnName("lockout_end");
			b.Property<string>("NormalizedEmail").HasMaxLength(256).HasColumnType("character varying(256)")
				.HasColumnName("normalized_email");
			b.Property<string>("NormalizedUserName").HasMaxLength(256).HasColumnType("character varying(256)")
				.HasColumnName("normalized_user_name");
			b.Property<string>("PasswordHash").HasColumnType("text").HasColumnName("password_hash");
			b.Property<string>("PhoneNumber").HasColumnType("text").HasColumnName("phone_number");
			b.Property<bool>("PhoneNumberConfirmed").HasColumnType("boolean").HasColumnName("phone_number_confirmed");
			b.Property<string>("SecurityStamp").HasColumnType("text").HasColumnName("security_stamp");
			b.Property<bool>("TwoFactorEnabled").HasColumnType("boolean").HasColumnName("two_factor_enabled");
			b.Property<string>("UserName").HasMaxLength(256).HasColumnType("character varying(256)")
				.HasColumnName("user_name");
			b.HasKey("Id").HasName("p_k_asp_net_users");
			b.HasIndex("NormalizedEmail").HasDatabaseName("email_index");
			b.HasIndex("NormalizedUserName").IsUnique().HasDatabaseName("user_name_index");
			b.ToTable("asp_net_users", (string?)null);
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.HasOne("WorkPulse.Infrastructure.Persistence.ApplicationRole", null).WithMany().HasForeignKey("RoleId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired()
				.HasConstraintName("f_k_asp_net_role_claims__asp_net_roles_role_id");
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.HasOne("WorkPulse.Infrastructure.Persistence.ApplicationUser", null).WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired()
				.HasConstraintName("f_k_asp_net_user_claims__asp_net_users_user_id");
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.HasOne("WorkPulse.Infrastructure.Persistence.ApplicationUser", null).WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired()
				.HasConstraintName("f_k_asp_net_user_logins__asp_net_users_user_id");
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.HasOne("WorkPulse.Infrastructure.Persistence.ApplicationRole", null).WithMany().HasForeignKey("RoleId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired()
				.HasConstraintName("f_k_asp_net_user_roles__asp_net_roles_role_id");
			b.HasOne("WorkPulse.Infrastructure.Persistence.ApplicationUser", null).WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired()
				.HasConstraintName("f_k_asp_net_user_roles__asp_net_users_user_id");
		});
		modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", delegate(EntityTypeBuilder b)
		{
			b.HasOne("WorkPulse.Infrastructure.Persistence.ApplicationUser", null).WithMany().HasForeignKey("UserId")
				.OnDelete(DeleteBehavior.Cascade)
				.IsRequired()
				.HasConstraintName("f_k_asp_net_user_tokens__asp_net_users_user_id");
		});
	}
}
