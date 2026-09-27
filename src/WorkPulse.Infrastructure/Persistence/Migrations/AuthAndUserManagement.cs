using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace WorkPulse.Infrastructure.Persistence.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260703181945_AuthAndUserManagement")]
public class AuthAndUserManagement : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<Guid>("assigned_team_id", "tasks", "uuid", null, null, rowVersion: false, null, nullable: true);
		int? maxLength = 2000;
		migrationBuilder.AddColumn<string>("blocked_reason", "tasks", "character varying(2000)", null, maxLength, rowVersion: false, null, nullable: true);
		migrationBuilder.AddColumn<Guid>("epic_id", "tasks", "uuid", null, null, rowVersion: false, null, nullable: true);
		maxLength = 10;
		int? scale = 2;
		migrationBuilder.AddColumn<decimal>("estimated_hours", "tasks", "numeric(10,2)", null, null, rowVersion: false, null, nullable: true, null, null, null, null, null, null, maxLength, scale);
		migrationBuilder.AddColumn<bool>("is_blocked", "tasks", "boolean", null, null, rowVersion: false, null, nullable: false, false);
		scale = 10;
		maxLength = 2;
		migrationBuilder.AddColumn<decimal>("logged_hours", "tasks", "numeric(10,2)", null, null, rowVersion: false, null, nullable: false, 0m, null, null, null, null, null, scale, maxLength);
		object defaultValue = new byte[0];
		migrationBuilder.AddColumn<byte[]>("row_version", "tasks", "bytea", null, null, rowVersion: true, null, nullable: false, defaultValue);
		migrationBuilder.AddColumn<Guid>("sprint_id", "tasks", "uuid", null, null, rowVersion: false, null, nullable: true);
		migrationBuilder.AddColumn<int>("story_points", "tasks", "integer", null, null, rowVersion: false, null, nullable: true);
		migrationBuilder.CreateTable("audit_log_entries", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			int? maxLength2 = 200;
			OperationBuilder<AddColumnOperation> entity_type = table.Column<string>("character varying(200)", null, maxLength2);
			OperationBuilder<AddColumnOperation> entity_id = table.Column<Guid>("uuid");
			maxLength2 = 100;
			return new
			{
				id = id,
				entity_type = entity_type,
				entity_id = entity_id,
				action = table.Column<string>("character varying(100)", null, maxLength2),
				user_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				changes_json = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true),
				timestamp = table.Column<DateTime>("timestamp with time zone"),
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
			table.PrimaryKey("p_k_audit_log_entries", x => x.id);
		});
		migrationBuilder.CreateTable("automation_rules", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			int? maxLength2 = 200;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("character varying(200)", null, maxLength2);
			maxLength2 = 50;
			OperationBuilder<AddColumnOperation> trigger_type = table.Column<string>("character varying(50)", null, maxLength2);
			OperationBuilder<AddColumnOperation> trigger_config_json = table.Column<string>("text");
			maxLength2 = 50;
			return new
			{
				id = id,
				name = name,
				trigger_type = trigger_type,
				trigger_config_json = trigger_config_json,
				action_type = table.Column<string>("character varying(50)", null, maxLength2),
				action_config_json = table.Column<string>("text"),
				is_enabled = table.Column<bool>("boolean"),
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
			table.PrimaryKey("p_k_automation_rules", x => x.id);
		});
		migrationBuilder.CreateTable("epics", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> team_id = table.Column<Guid>("uuid");
			int? maxLength2 = 500;
			OperationBuilder<AddColumnOperation> title = table.Column<string>("character varying(500)", null, maxLength2);
			maxLength2 = 10000;
			OperationBuilder<AddColumnOperation> description = table.Column<string>("character varying(10000)", null, maxLength2, rowVersion: false, null, nullable: true);
			maxLength2 = 50;
			return new
			{
				id = id,
				team_id = team_id,
				title = title,
				description = description,
				status = table.Column<string>("character varying(50)", null, maxLength2),
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
			table.PrimaryKey("p_k_epics", x => x.id);
		});
		migrationBuilder.CreateTable("labels", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			int? maxLength2 = 100;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("character varying(100)", null, maxLength2);
			maxLength2 = 20;
			return new
			{
				id = id,
				name = name,
				color = table.Column<string>("character varying(20)", null, maxLength2),
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
			table.PrimaryKey("p_k_labels", x => x.id);
		});
		migrationBuilder.CreateTable("outbox_messages", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			int? maxLength2 = 500;
			OperationBuilder<AddColumnOperation> event_type = table.Column<string>("character varying(500)", null, maxLength2);
			OperationBuilder<AddColumnOperation> payload = table.Column<string>("text");
			OperationBuilder<AddColumnOperation> created_at_utc = table.Column<DateTime>("timestamp with time zone");
			OperationBuilder<AddColumnOperation> processed_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true);
			maxLength2 = 4000;
			return new
			{
				id = id,
				event_type = event_type,
				payload = payload,
				created_at_utc = created_at_utc,
				processed_at_utc = processed_at_utc,
				error = table.Column<string>("character varying(4000)", null, maxLength2, rowVersion: false, null, nullable: true),
				retry_count = table.Column<int>("integer")
			};
		}, null, table =>
		{
			table.PrimaryKey("p_k_outbox_messages", x => x.id);
		});
		migrationBuilder.CreateTable("refresh_tokens", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> user_id = table.Column<Guid>("uuid");
			int? maxLength2 = 256;
			return new
			{
				id = id,
				user_id = user_id,
				token = table.Column<string>("character varying(256)", null, maxLength2),
				expires_at_utc = table.Column<DateTime>("timestamp with time zone"),
				revoked_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true),
				created_at_utc = table.Column<DateTime>("timestamp with time zone")
			};
		}, null, table =>
		{
			table.PrimaryKey("p_k_refresh_tokens", x => x.id);
		});
		migrationBuilder.CreateTable("saved_views", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> user_id = table.Column<Guid>("uuid");
			int? maxLength2 = 200;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("character varying(200)", null, maxLength2);
			maxLength2 = 50;
			OperationBuilder<AddColumnOperation> entity_type = table.Column<string>("character varying(50)", null, maxLength2);
			maxLength2 = 10000;
			OperationBuilder<AddColumnOperation> filters_json = table.Column<string>("character varying(10000)", null, maxLength2);
			maxLength2 = 5000;
			return new
			{
				id = id,
				user_id = user_id,
				name = name,
				entity_type = entity_type,
				filters_json = filters_json,
				sort_json = table.Column<string>("character varying(5000)", null, maxLength2),
				is_shared = table.Column<bool>("boolean"),
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
			table.PrimaryKey("p_k_saved_views", x => x.id);
		});
		migrationBuilder.CreateTable("sprints", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> team_id = table.Column<Guid>("uuid");
			int? maxLength2 = 200;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("character varying(200)", null, maxLength2);
			maxLength2 = 2000;
			OperationBuilder<AddColumnOperation> goal = table.Column<string>("character varying(2000)", null, maxLength2, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> start_date = table.Column<DateOnly>("date");
			OperationBuilder<AddColumnOperation> end_date = table.Column<DateOnly>("date");
			maxLength2 = 50;
			return new
			{
				id = id,
				team_id = team_id,
				name = name,
				goal = goal,
				start_date = start_date,
				end_date = end_date,
				status = table.Column<string>("character varying(50)", null, maxLength2),
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
			table.PrimaryKey("p_k_sprints", x => x.id);
		});
		migrationBuilder.CreateTable("task_assignees", (ColumnsBuilder table) => new
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
			table.PrimaryKey("p_k_task_assignees", x => x.id);
		});
		migrationBuilder.CreateTable("task_dependencies", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> task_id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> depends_on_task_id = table.Column<Guid>("uuid");
			int? maxLength2 = 50;
			return new
			{
				id = id,
				task_id = task_id,
				depends_on_task_id = depends_on_task_id,
				type = table.Column<string>("character varying(50)", null, maxLength2),
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
			table.PrimaryKey("p_k_task_dependencies", x => x.id);
		});
		migrationBuilder.CreateTable("task_labels", (ColumnsBuilder table) => new
		{
			id = table.Column<Guid>("uuid"),
			task_id = table.Column<Guid>("uuid"),
			label_id = table.Column<Guid>("uuid"),
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
			table.PrimaryKey("p_k_task_labels", x => x.id);
		});
		migrationBuilder.CreateTable("user_invitations", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			int? maxLength2 = 256;
			OperationBuilder<AddColumnOperation> email = table.Column<string>("character varying(256)", null, maxLength2);
			maxLength2 = 50;
			OperationBuilder<AddColumnOperation> role = table.Column<string>("character varying(50)", null, maxLength2);
			maxLength2 = 256;
			OperationBuilder<AddColumnOperation> token = table.Column<string>("character varying(256)", null, maxLength2);
			OperationBuilder<AddColumnOperation> expires_at_utc = table.Column<DateTime>("timestamp with time zone");
			OperationBuilder<AddColumnOperation> accepted_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> invited_by_id = table.Column<Guid>("uuid");
			maxLength2 = 50;
			return new
			{
				id = id,
				email = email,
				role = role,
				token = token,
				expires_at_utc = expires_at_utc,
				accepted_at_utc = accepted_at_utc,
				invited_by_id = invited_by_id,
				status = table.Column<string>("character varying(50)", null, maxLength2),
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
			table.PrimaryKey("p_k_user_invitations", x => x.id);
		});
		migrationBuilder.CreateTable("work_logs", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> task_id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> user_id = table.Column<Guid>("uuid");
			int? precision = 10;
			int? scale2 = 2;
			OperationBuilder<AddColumnOperation> hours = table.Column<decimal>("numeric(10,2)", null, null, rowVersion: false, null, nullable: false, null, null, null, null, null, null, precision, scale2);
			scale2 = 2000;
			return new
			{
				id = id,
				task_id = task_id,
				user_id = user_id,
				hours = hours,
				description = table.Column<string>("character varying(2000)", null, scale2, rowVersion: false, null, nullable: true),
				logged_date = table.Column<DateOnly>("date"),
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
			table.PrimaryKey("p_k_work_logs", x => x.id);
		});
		migrationBuilder.CreateIndex("i_x_tasks_assigned_team_id", "tasks", "assigned_team_id");
		migrationBuilder.CreateIndex("i_x_tasks_epic_id", "tasks", "epic_id");
		migrationBuilder.CreateIndex("i_x_tasks_sprint_id", "tasks", "sprint_id");
		migrationBuilder.CreateIndex("i_x_audit_log_entries_tenant_id_entity_type_entity_id", "audit_log_entries", new string[3] { "tenant_id", "entity_type", "entity_id" });
		migrationBuilder.CreateIndex("i_x_audit_log_entries_tenant_id_timestamp", "audit_log_entries", new string[2] { "tenant_id", "timestamp" });
		migrationBuilder.CreateIndex("i_x_automation_rules_tenant_id_is_enabled", "automation_rules", new string[2] { "tenant_id", "is_enabled" });
		migrationBuilder.CreateIndex("i_x_epics_team_id", "epics", "team_id");
		migrationBuilder.CreateIndex("i_x_epics_tenant_id_team_id_status", "epics", new string[3] { "tenant_id", "team_id", "status" });
		migrationBuilder.CreateIndex("i_x_labels_tenant_id_name", "labels", new string[2] { "tenant_id", "name" }, null, unique: true);
		migrationBuilder.CreateIndex("i_x_outbox_messages_processed_at_utc_created_at_utc", "outbox_messages", new string[2] { "processed_at_utc", "created_at_utc" });
		migrationBuilder.CreateIndex("i_x_refresh_tokens_token", "refresh_tokens", "token", null, unique: true);
		migrationBuilder.CreateIndex("i_x_refresh_tokens_user_id", "refresh_tokens", "user_id");
		migrationBuilder.CreateIndex("i_x_saved_views_tenant_id_user_id_entity_type", "saved_views", new string[3] { "tenant_id", "user_id", "entity_type" });
		migrationBuilder.CreateIndex("i_x_saved_views_user_id", "saved_views", "user_id");
		migrationBuilder.CreateIndex("i_x_sprints_team_id", "sprints", "team_id");
		migrationBuilder.CreateIndex("i_x_sprints_tenant_id_team_id_status", "sprints", new string[3] { "tenant_id", "team_id", "status" });
		migrationBuilder.CreateIndex("i_x_task_assignees_task_id_user_id", "task_assignees", new string[2] { "task_id", "user_id" }, null, unique: true);
		migrationBuilder.CreateIndex("i_x_task_assignees_user_id", "task_assignees", "user_id");
		migrationBuilder.CreateIndex("i_x_task_dependencies_depends_on_task_id", "task_dependencies", "depends_on_task_id");
		migrationBuilder.CreateIndex("i_x_task_dependencies_task_id_depends_on_task_id_type", "task_dependencies", new string[3] { "task_id", "depends_on_task_id", "type" }, null, unique: true);
		migrationBuilder.CreateIndex("i_x_task_labels_label_id", "task_labels", "label_id");
		migrationBuilder.CreateIndex("i_x_task_labels_task_id_label_id", "task_labels", new string[2] { "task_id", "label_id" }, null, unique: true);
		migrationBuilder.CreateIndex("i_x_user_invitations_tenant_id_email_status", "user_invitations", new string[3] { "tenant_id", "email", "status" });
		migrationBuilder.CreateIndex("i_x_user_invitations_token", "user_invitations", "token", null, unique: true);
		migrationBuilder.CreateIndex("i_x_work_logs_task_id", "work_logs", "task_id");
		migrationBuilder.CreateIndex("i_x_work_logs_tenant_id_task_id_logged_date", "work_logs", new string[3] { "tenant_id", "task_id", "logged_date" });
		migrationBuilder.CreateIndex("i_x_work_logs_user_id", "work_logs", "user_id");
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable("audit_log_entries");
		migrationBuilder.DropTable("automation_rules");
		migrationBuilder.DropTable("epics");
		migrationBuilder.DropTable("labels");
		migrationBuilder.DropTable("outbox_messages");
		migrationBuilder.DropTable("refresh_tokens");
		migrationBuilder.DropTable("saved_views");
		migrationBuilder.DropTable("sprints");
		migrationBuilder.DropTable("task_assignees");
		migrationBuilder.DropTable("task_dependencies");
		migrationBuilder.DropTable("task_labels");
		migrationBuilder.DropTable("user_invitations");
		migrationBuilder.DropTable("work_logs");
		migrationBuilder.DropIndex("i_x_tasks_assigned_team_id", "tasks");
		migrationBuilder.DropIndex("i_x_tasks_epic_id", "tasks");
		migrationBuilder.DropIndex("i_x_tasks_sprint_id", "tasks");
		migrationBuilder.DropColumn("assigned_team_id", "tasks");
		migrationBuilder.DropColumn("blocked_reason", "tasks");
		migrationBuilder.DropColumn("epic_id", "tasks");
		migrationBuilder.DropColumn("estimated_hours", "tasks");
		migrationBuilder.DropColumn("is_blocked", "tasks");
		migrationBuilder.DropColumn("logged_hours", "tasks");
		migrationBuilder.DropColumn("row_version", "tasks");
		migrationBuilder.DropColumn("sprint_id", "tasks");
		migrationBuilder.DropColumn("story_points", "tasks");
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
		modelBuilder.Entity("WorkPulse.Domain.Entities.AuditLogEntry", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<string>("Action").IsRequired().HasMaxLength(100)
				.HasColumnType("character varying(100)")
				.HasColumnName("action");
			b.Property<string>("ChangesJson").HasColumnType("text").HasColumnName("changes_json");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<Guid>("EntityId").HasColumnType("uuid").HasColumnName("entity_id");
			b.Property<string>("EntityType").IsRequired().HasMaxLength(200)
				.HasColumnType("character varying(200)")
				.HasColumnName("entity_type");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime>("Timestamp").HasColumnType("timestamp with time zone").HasColumnName("timestamp");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.Property<Guid?>("UserId").HasColumnType("uuid").HasColumnName("user_id");
			b.HasKey("Id").HasName("p_k_audit_log_entries");
			b.HasIndex("TenantId", "Timestamp").HasDatabaseName("i_x_audit_log_entries_tenant_id_timestamp");
			b.HasIndex("TenantId", "EntityType", "EntityId").HasDatabaseName("i_x_audit_log_entries_tenant_id_entity_type_entity_id");
			b.ToTable("audit_log_entries", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.AutomationRule", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<string>("ActionConfigJson").IsRequired().HasColumnType("text")
				.HasColumnName("action_config_json");
			b.Property<string>("ActionType").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("action_type");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<bool>("IsEnabled").HasColumnType("boolean").HasColumnName("is_enabled");
			b.Property<string>("Name").IsRequired().HasMaxLength(200)
				.HasColumnType("character varying(200)")
				.HasColumnName("name");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<string>("TriggerConfigJson").IsRequired().HasColumnType("text")
				.HasColumnName("trigger_config_json");
			b.Property<string>("TriggerType").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("trigger_type");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.HasKey("Id").HasName("p_k_automation_rules");
			b.HasIndex("TenantId", "IsEnabled").HasDatabaseName("i_x_automation_rules_tenant_id_is_enabled");
			b.ToTable("automation_rules", (string?)null);
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
		modelBuilder.Entity("WorkPulse.Domain.Entities.Epic", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<string>("Description").HasMaxLength(10000).HasColumnType("character varying(10000)")
				.HasColumnName("description");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<string>("Status").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("status");
			b.Property<Guid>("TeamId").HasColumnType("uuid").HasColumnName("team_id");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<string>("Title").IsRequired().HasMaxLength(500)
				.HasColumnType("character varying(500)")
				.HasColumnName("title");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.HasKey("Id").HasName("p_k_epics");
			b.HasIndex("TeamId").HasDatabaseName("i_x_epics_team_id");
			b.HasIndex("TenantId", "TeamId", "Status").HasDatabaseName("i_x_epics_tenant_id_team_id_status");
			b.ToTable("epics", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.Label", delegate(EntityTypeBuilder b)
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
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<string>("Name").IsRequired().HasMaxLength(100)
				.HasColumnType("character varying(100)")
				.HasColumnName("name");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.HasKey("Id").HasName("p_k_labels");
			b.HasIndex("TenantId", "Name").IsUnique().HasDatabaseName("i_x_labels_tenant_id_name");
			b.ToTable("labels", (string?)null);
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
		modelBuilder.Entity("WorkPulse.Domain.Entities.RefreshToken", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<DateTime>("ExpiresAtUtc").HasColumnType("timestamp with time zone").HasColumnName("expires_at_utc");
			b.Property<DateTime?>("RevokedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("revoked_at_utc");
			b.Property<string>("Token").IsRequired().HasMaxLength(256)
				.HasColumnType("character varying(256)")
				.HasColumnName("token");
			b.Property<Guid>("UserId").HasColumnType("uuid").HasColumnName("user_id");
			b.HasKey("Id").HasName("p_k_refresh_tokens");
			b.HasIndex("Token").IsUnique().HasDatabaseName("i_x_refresh_tokens_token");
			b.HasIndex("UserId").HasDatabaseName("i_x_refresh_tokens_user_id");
			b.ToTable("refresh_tokens", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.SavedView", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<string>("EntityType").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("entity_type");
			b.Property<string>("FiltersJson").IsRequired().HasMaxLength(10000)
				.HasColumnType("character varying(10000)")
				.HasColumnName("filters_json");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<bool>("IsShared").HasColumnType("boolean").HasColumnName("is_shared");
			b.Property<string>("Name").IsRequired().HasMaxLength(200)
				.HasColumnType("character varying(200)")
				.HasColumnName("name");
			b.Property<string>("SortJson").IsRequired().HasMaxLength(5000)
				.HasColumnType("character varying(5000)")
				.HasColumnName("sort_json");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.Property<Guid>("UserId").HasColumnType("uuid").HasColumnName("user_id");
			b.HasKey("Id").HasName("p_k_saved_views");
			b.HasIndex("UserId").HasDatabaseName("i_x_saved_views_user_id");
			b.HasIndex("TenantId", "UserId", "EntityType").HasDatabaseName("i_x_saved_views_tenant_id_user_id_entity_type");
			b.ToTable("saved_views", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.Sprint", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<DateOnly>("EndDate").HasColumnType("date").HasColumnName("end_date");
			b.Property<string>("Goal").HasMaxLength(2000).HasColumnType("character varying(2000)")
				.HasColumnName("goal");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<string>("Name").IsRequired().HasMaxLength(200)
				.HasColumnType("character varying(200)")
				.HasColumnName("name");
			b.Property<DateOnly>("StartDate").HasColumnType("date").HasColumnName("start_date");
			b.Property<string>("Status").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("status");
			b.Property<Guid>("TeamId").HasColumnType("uuid").HasColumnName("team_id");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.HasKey("Id").HasName("p_k_sprints");
			b.HasIndex("TeamId").HasDatabaseName("i_x_sprints_team_id");
			b.HasIndex("TenantId", "TeamId", "Status").HasDatabaseName("i_x_sprints_tenant_id_team_id_status");
			b.ToTable("sprints", (string?)null);
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
		modelBuilder.Entity("WorkPulse.Domain.Entities.TaskAssignee", delegate(EntityTypeBuilder b)
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
			b.HasKey("Id").HasName("p_k_task_assignees");
			b.HasIndex("UserId").HasDatabaseName("i_x_task_assignees_user_id");
			b.HasIndex("TaskId", "UserId").IsUnique().HasDatabaseName("i_x_task_assignees_task_id_user_id");
			b.ToTable("task_assignees", (string?)null);
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
		modelBuilder.Entity("WorkPulse.Domain.Entities.TaskDependency", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<Guid>("DependsOnTaskId").HasColumnType("uuid").HasColumnName("depends_on_task_id");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<Guid>("TaskId").HasColumnType("uuid").HasColumnName("task_id");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<string>("Type").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("type");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.HasKey("Id").HasName("p_k_task_dependencies");
			b.HasIndex("DependsOnTaskId").HasDatabaseName("i_x_task_dependencies_depends_on_task_id");
			b.HasIndex("TaskId", "DependsOnTaskId", "Type").IsUnique().HasDatabaseName("i_x_task_dependencies_task_id_depends_on_task_id_type");
			b.ToTable("task_dependencies", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.TaskItem", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<Guid?>("AssignedTeamId").HasColumnType("uuid").HasColumnName("assigned_team_id");
			b.Property<Guid?>("AssigneeId").HasColumnType("uuid").HasColumnName("assignee_id");
			b.Property<string>("BlockedReason").HasMaxLength(2000).HasColumnType("character varying(2000)")
				.HasColumnName("blocked_reason");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<Guid>("CreatorId").HasColumnType("uuid").HasColumnName("creator_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<string>("Description").HasMaxLength(10000).HasColumnType("character varying(10000)")
				.HasColumnName("description");
			b.Property<DateOnly?>("DueDate").HasColumnType("date").HasColumnName("due_date");
			b.Property<Guid?>("EpicId").HasColumnType("uuid").HasColumnName("epic_id");
			b.Property<decimal?>("EstimatedHours").HasPrecision(10, 2).HasColumnType("numeric(10,2)")
				.HasColumnName("estimated_hours");
			b.Property<bool>("IsBlocked").HasColumnType("boolean").HasColumnName("is_blocked");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<decimal>("LoggedHours").HasPrecision(10, 2).HasColumnType("numeric(10,2)")
				.HasColumnName("logged_hours");
			b.Property<int>("Number").HasColumnType("integer").HasColumnName("number");
			b.Property<Guid?>("ParentTaskId").HasColumnType("uuid").HasColumnName("parent_task_id");
			b.Property<string>("Priority").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("priority");
			b.Property<Guid?>("ProjectId").HasColumnType("uuid").HasColumnName("project_id");
			b.Property<byte[]>("RowVersion").IsConcurrencyToken().IsRequired()
				.ValueGeneratedOnAddOrUpdate()
				.HasColumnType("bytea")
				.HasColumnName("row_version");
			b.Property<int>("SortOrder").HasColumnType("integer").HasColumnName("sort_order");
			b.Property<Guid?>("SprintId").HasColumnType("uuid").HasColumnName("sprint_id");
			b.Property<int?>("StoryPoints").HasColumnType("integer").HasColumnName("story_points");
			b.Property<Guid>("TeamId").HasColumnType("uuid").HasColumnName("team_id");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<string>("Title").IsRequired().HasMaxLength(500)
				.HasColumnType("character varying(500)")
				.HasColumnName("title");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.Property<Guid>("WorkflowStateId").HasColumnType("uuid").HasColumnName("workflow_state_id");
			b.HasKey("Id").HasName("p_k_tasks");
			b.HasIndex("AssignedTeamId").HasDatabaseName("i_x_tasks_assigned_team_id");
			b.HasIndex("AssigneeId").HasDatabaseName("i_x_tasks_assignee_id");
			b.HasIndex("EpicId").HasDatabaseName("i_x_tasks_epic_id");
			b.HasIndex("ParentTaskId").HasDatabaseName("i_x_tasks_parent_task_id");
			b.HasIndex("ProjectId").HasDatabaseName("i_x_tasks_project_id");
			b.HasIndex("SprintId").HasDatabaseName("i_x_tasks_sprint_id");
			b.HasIndex("WorkflowStateId").HasDatabaseName("i_x_tasks_workflow_state_id");
			b.HasIndex("TeamId", "Number").IsUnique().HasDatabaseName("i_x_tasks_team_id_number");
			b.HasIndex("TenantId", "AssigneeId").HasDatabaseName("i_x_tasks_tenant_id_assignee_id");
			b.HasIndex("TenantId", "DueDate").HasDatabaseName("i_x_tasks_tenant_id_due_date");
			b.HasIndex("TenantId", "ProjectId", "WorkflowStateId").HasDatabaseName("i_x_tasks_tenant_id_project_id_workflow_state_id");
			b.HasIndex("TenantId", "TeamId", "CreatedAtUtc").HasDatabaseName("i_x_tasks_tenant_id_team_id_created_at_utc");
			b.ToTable("tasks", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.TaskLabel", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<Guid>("LabelId").HasColumnType("uuid").HasColumnName("label_id");
			b.Property<Guid>("TaskId").HasColumnType("uuid").HasColumnName("task_id");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.HasKey("Id").HasName("p_k_task_labels");
			b.HasIndex("LabelId").HasDatabaseName("i_x_task_labels_label_id");
			b.HasIndex("TaskId", "LabelId").IsUnique().HasDatabaseName("i_x_task_labels_task_id_label_id");
			b.ToTable("task_labels", (string?)null);
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
		modelBuilder.Entity("WorkPulse.Domain.Entities.UserInvitation", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime?>("AcceptedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("accepted_at_utc");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<string>("Email").IsRequired().HasMaxLength(256)
				.HasColumnType("character varying(256)")
				.HasColumnName("email");
			b.Property<DateTime>("ExpiresAtUtc").HasColumnType("timestamp with time zone").HasColumnName("expires_at_utc");
			b.Property<Guid>("InvitedById").HasColumnType("uuid").HasColumnName("invited_by_id");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<string>("Role").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("role");
			b.Property<string>("Status").IsRequired().HasMaxLength(50)
				.HasColumnType("character varying(50)")
				.HasColumnName("status");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<string>("Token").IsRequired().HasMaxLength(256)
				.HasColumnType("character varying(256)")
				.HasColumnName("token");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.HasKey("Id").HasName("p_k_user_invitations");
			b.HasIndex("Token").IsUnique().HasDatabaseName("i_x_user_invitations_token");
			b.HasIndex("TenantId", "Email", "Status").HasDatabaseName("i_x_user_invitations_tenant_id_email_status");
			b.ToTable("user_invitations", (string?)null);
		});
		modelBuilder.Entity("WorkPulse.Domain.Entities.WorkLog", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<Guid?>("CreatedById").HasColumnType("uuid").HasColumnName("created_by_id");
			b.Property<DateTime?>("DeletedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("deleted_at_utc");
			b.Property<Guid?>("DeletedById").HasColumnType("uuid").HasColumnName("deleted_by_id");
			b.Property<string>("Description").HasMaxLength(2000).HasColumnType("character varying(2000)")
				.HasColumnName("description");
			b.Property<decimal>("Hours").HasPrecision(10, 2).HasColumnType("numeric(10,2)")
				.HasColumnName("hours");
			b.Property<bool>("IsDeleted").HasColumnType("boolean").HasColumnName("is_deleted");
			b.Property<DateOnly>("LoggedDate").HasColumnType("date").HasColumnName("logged_date");
			b.Property<Guid>("TaskId").HasColumnType("uuid").HasColumnName("task_id");
			b.Property<Guid>("TenantId").HasColumnType("uuid").HasColumnName("tenant_id");
			b.Property<DateTime?>("UpdatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("updated_at_utc");
			b.Property<Guid?>("UpdatedById").HasColumnType("uuid").HasColumnName("updated_by_id");
			b.Property<Guid>("UserId").HasColumnType("uuid").HasColumnName("user_id");
			b.HasKey("Id").HasName("p_k_work_logs");
			b.HasIndex("TaskId").HasDatabaseName("i_x_work_logs_task_id");
			b.HasIndex("UserId").HasDatabaseName("i_x_work_logs_user_id");
			b.HasIndex("TenantId", "TaskId", "LoggedDate").HasDatabaseName("i_x_work_logs_tenant_id_task_id_logged_date");
			b.ToTable("work_logs", (string?)null);
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
		modelBuilder.Entity("WorkPulse.Infrastructure.Persistence.Outbox.OutboxMessage", delegate(EntityTypeBuilder b)
		{
			b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid")
				.HasColumnName("id");
			b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
			b.Property<string>("Error").HasMaxLength(4000).HasColumnType("character varying(4000)")
				.HasColumnName("error");
			b.Property<string>("EventType").IsRequired().HasMaxLength(500)
				.HasColumnType("character varying(500)")
				.HasColumnName("event_type");
			b.Property<string>("Payload").IsRequired().HasColumnType("text")
				.HasColumnName("payload");
			b.Property<DateTime?>("ProcessedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("processed_at_utc");
			b.Property<int>("RetryCount").HasColumnType("integer").HasColumnName("retry_count");
			b.HasKey("Id").HasName("p_k_outbox_messages");
			b.HasIndex("ProcessedAtUtc", "CreatedAtUtc").HasDatabaseName("i_x_outbox_messages_processed_at_utc_created_at_utc");
			b.ToTable("outbox_messages", (string?)null);
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
