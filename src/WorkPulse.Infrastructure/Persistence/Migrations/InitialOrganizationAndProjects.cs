using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WorkPulse.Infrastructure.Persistence.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260703180422_InitialOrganizationAndProjects")]
public class InitialOrganizationAndProjects : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable("asp_net_roles", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> description = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> tenant_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true);
			int? maxLength = 256;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("character varying(256)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 256;
			return new
			{
				id = id,
				description = description,
				tenant_id = tenant_id,
				name = name,
				normalized_name = table.Column<string>("character varying(256)", null, maxLength, rowVersion: false, null, nullable: true),
				concurrency_stamp = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("p_k_asp_net_roles", x => x.id);
		});
		migrationBuilder.CreateTable("asp_net_users", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> first_name = table.Column<string>("text");
			OperationBuilder<AddColumnOperation> last_name = table.Column<string>("text");
			OperationBuilder<AddColumnOperation> avatar_url = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> current_tenant_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> created_at_utc = table.Column<DateTime>("timestamp with time zone");
			OperationBuilder<AddColumnOperation> last_login_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> is_active = table.Column<bool>("boolean");
			int? maxLength = 256;
			OperationBuilder<AddColumnOperation> user_name = table.Column<string>("character varying(256)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 256;
			OperationBuilder<AddColumnOperation> normalized_user_name = table.Column<string>("character varying(256)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 256;
			OperationBuilder<AddColumnOperation> email = table.Column<string>("character varying(256)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 256;
			return new
			{
				id = id,
				first_name = first_name,
				last_name = last_name,
				avatar_url = avatar_url,
				current_tenant_id = current_tenant_id,
				created_at_utc = created_at_utc,
				last_login_at_utc = last_login_at_utc,
				is_active = is_active,
				user_name = user_name,
				normalized_user_name = normalized_user_name,
				email = email,
				normalized_email = table.Column<string>("character varying(256)", null, maxLength, rowVersion: false, null, nullable: true),
				email_confirmed = table.Column<bool>("boolean"),
				password_hash = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true),
				security_stamp = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true),
				concurrency_stamp = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true),
				phone_number = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true),
				phone_number_confirmed = table.Column<bool>("boolean"),
				two_factor_enabled = table.Column<bool>("boolean"),
				lockout_end = table.Column<DateTimeOffset>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true),
				lockout_enabled = table.Column<bool>("boolean"),
				access_failed_count = table.Column<int>("integer")
			};
		}, null, table =>
		{
			table.PrimaryKey("p_k_asp_net_users", x => x.id);
		});
		migrationBuilder.CreateTable("companies", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			int? maxLength = 200;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("character varying(200)", null, maxLength);
			maxLength = 200;
			OperationBuilder<AddColumnOperation> slug = table.Column<string>("character varying(200)", null, maxLength);
			maxLength = 500;
			OperationBuilder<AddColumnOperation> logo_url = table.Column<string>("character varying(500)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 2000;
			return new
			{
				id = id,
				name = name,
				slug = slug,
				logo_url = logo_url,
				description = table.Column<string>("character varying(2000)", null, maxLength, rowVersion: false, null, nullable: true),
				is_active = table.Column<bool>("boolean"),
				created_at_utc = table.Column<DateTime>("timestamp with time zone"),
				created_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				updated_at_utc = table.Column<DateTime>("timestamp with time zone", null, null, rowVersion: false, null, nullable: true),
				updated_by_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true)
			};
		}, null, table =>
		{
			table.PrimaryKey("p_k_companies", x => x.id);
		});
		migrationBuilder.CreateTable("company_members", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> user_id = table.Column<Guid>("uuid");
			int? maxLength = 50;
			return new
			{
				id = id,
				user_id = user_id,
				role = table.Column<string>("character varying(50)", null, maxLength),
				is_active = table.Column<bool>("boolean"),
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
			table.PrimaryKey("p_k_company_members", x => x.id);
		});
		migrationBuilder.CreateTable("projects", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> team_id = table.Column<Guid>("uuid");
			int? maxLength = 200;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("character varying(200)", null, maxLength);
			maxLength = 10;
			OperationBuilder<AddColumnOperation> key = table.Column<string>("character varying(10)", null, maxLength);
			maxLength = 5000;
			OperationBuilder<AddColumnOperation> description = table.Column<string>("character varying(5000)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 50;
			return new
			{
				id = id,
				team_id = team_id,
				name = name,
				key = key,
				description = description,
				status = table.Column<string>("character varying(50)", null, maxLength),
				lead_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				start_date = table.Column<DateOnly>("date", null, null, rowVersion: false, null, nullable: true),
				target_date = table.Column<DateOnly>("date", null, null, rowVersion: false, null, nullable: true),
				is_archived = table.Column<bool>("boolean"),
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
			table.PrimaryKey("p_k_projects", x => x.id);
		});
		migrationBuilder.CreateTable("tasks", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> team_id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> project_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true);
			OperationBuilder<AddColumnOperation> workflow_state_id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> number = table.Column<int>("integer");
			int? maxLength = 500;
			OperationBuilder<AddColumnOperation> title = table.Column<string>("character varying(500)", null, maxLength);
			maxLength = 10000;
			OperationBuilder<AddColumnOperation> description = table.Column<string>("character varying(10000)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 50;
			return new
			{
				id = id,
				team_id = team_id,
				project_id = project_id,
				workflow_state_id = workflow_state_id,
				number = number,
				title = title,
				description = description,
				priority = table.Column<string>("character varying(50)", null, maxLength),
				assignee_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				creator_id = table.Column<Guid>("uuid"),
				due_date = table.Column<DateOnly>("date", null, null, rowVersion: false, null, nullable: true),
				parent_task_id = table.Column<Guid>("uuid", null, null, rowVersion: false, null, nullable: true),
				sort_order = table.Column<int>("integer"),
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
			table.PrimaryKey("p_k_tasks", x => x.id);
		});
		migrationBuilder.CreateTable("team_issue_counters", (ColumnsBuilder table) => new
		{
			team_id = table.Column<Guid>("uuid"),
			tenant_id = table.Column<Guid>("uuid"),
			last_number = table.Column<int>("integer")
		}, null, table =>
		{
			table.PrimaryKey("p_k_team_issue_counters", x => x.team_id);
		});
		migrationBuilder.CreateTable("team_members", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> team_id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> user_id = table.Column<Guid>("uuid");
			int? maxLength = 50;
			return new
			{
				id = id,
				team_id = team_id,
				user_id = user_id,
				role = table.Column<string>("character varying(50)", null, maxLength),
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
			table.PrimaryKey("p_k_team_members", x => x.id);
		});
		migrationBuilder.CreateTable("teams", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			int? maxLength = 200;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("character varying(200)", null, maxLength);
			maxLength = 6;
			OperationBuilder<AddColumnOperation> key = table.Column<string>("character varying(6)", null, maxLength);
			maxLength = 2000;
			OperationBuilder<AddColumnOperation> description = table.Column<string>("character varying(2000)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 50;
			OperationBuilder<AddColumnOperation> icon = table.Column<string>("character varying(50)", null, maxLength, rowVersion: false, null, nullable: true);
			maxLength = 20;
			return new
			{
				id = id,
				name = name,
				key = key,
				description = description,
				icon = icon,
				color = table.Column<string>("character varying(20)", null, maxLength, rowVersion: false, null, nullable: true),
				is_archived = table.Column<bool>("boolean"),
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
			table.PrimaryKey("p_k_teams", x => x.id);
		});
		migrationBuilder.CreateTable("workflow_states", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> workflow_id = table.Column<Guid>("uuid");
			int? maxLength = 100;
			OperationBuilder<AddColumnOperation> name = table.Column<string>("character varying(100)", null, maxLength);
			maxLength = 50;
			OperationBuilder<AddColumnOperation> type = table.Column<string>("character varying(50)", null, maxLength);
			maxLength = 20;
			return new
			{
				id = id,
				workflow_id = workflow_id,
				name = name,
				type = type,
				color = table.Column<string>("character varying(20)", null, maxLength),
				position = table.Column<int>("integer"),
				is_default = table.Column<bool>("boolean"),
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
			table.PrimaryKey("p_k_workflow_states", x => x.id);
		});
		migrationBuilder.CreateTable("workflows", delegate(ColumnsBuilder table)
		{
			OperationBuilder<AddColumnOperation> id = table.Column<Guid>("uuid");
			OperationBuilder<AddColumnOperation> team_id = table.Column<Guid>("uuid");
			int? maxLength = 100;
			return new
			{
				id = id,
				team_id = team_id,
				name = table.Column<string>("character varying(100)", null, maxLength),
				is_default = table.Column<bool>("boolean"),
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
			table.PrimaryKey("p_k_workflows", x => x.id);
		});
		migrationBuilder.CreateTable("asp_net_role_claims", (ColumnsBuilder table) => new
		{
			id = table.Column<int>("integer").Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
			role_id = table.Column<Guid>("uuid"),
			claim_type = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true),
			claim_value = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("p_k_asp_net_role_claims", x => x.id);
			table.ForeignKey("f_k_asp_net_role_claims__asp_net_roles_role_id", x => x.role_id, "asp_net_roles", "id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("asp_net_user_claims", (ColumnsBuilder table) => new
		{
			id = table.Column<int>("integer").Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
			user_id = table.Column<Guid>("uuid"),
			claim_type = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true),
			claim_value = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("p_k_asp_net_user_claims", x => x.id);
			table.ForeignKey("f_k_asp_net_user_claims__asp_net_users_user_id", x => x.user_id, "asp_net_users", "id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("asp_net_user_logins", (ColumnsBuilder table) => new
		{
			login_provider = table.Column<string>("text"),
			provider_key = table.Column<string>("text"),
			provider_display_name = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true),
			user_id = table.Column<Guid>("uuid")
		}, null, table =>
		{
			table.PrimaryKey("p_k_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
			table.ForeignKey("f_k_asp_net_user_logins__asp_net_users_user_id", x => x.user_id, "asp_net_users", "id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("asp_net_user_roles", (ColumnsBuilder table) => new
		{
			user_id = table.Column<Guid>("uuid"),
			role_id = table.Column<Guid>("uuid")
		}, null, table =>
		{
			table.PrimaryKey("p_k_asp_net_user_roles", x => new { x.user_id, x.role_id });
			table.ForeignKey("f_k_asp_net_user_roles__asp_net_roles_role_id", x => x.role_id, "asp_net_roles", "id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
			table.ForeignKey("f_k_asp_net_user_roles__asp_net_users_user_id", x => x.user_id, "asp_net_users", "id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateTable("asp_net_user_tokens", (ColumnsBuilder table) => new
		{
			user_id = table.Column<Guid>("uuid"),
			login_provider = table.Column<string>("text"),
			name = table.Column<string>("text"),
			value = table.Column<string>("text", null, null, rowVersion: false, null, nullable: true)
		}, null, table =>
		{
			table.PrimaryKey("p_k_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
			table.ForeignKey("f_k_asp_net_user_tokens__asp_net_users_user_id", x => x.user_id, "asp_net_users", "id", null, ReferentialAction.NoAction, ReferentialAction.Cascade);
		});
		migrationBuilder.CreateIndex("i_x_asp_net_role_claims_role_id", "asp_net_role_claims", "role_id");
		migrationBuilder.CreateIndex("role_name_index", "asp_net_roles", "normalized_name", null, unique: true);
		migrationBuilder.CreateIndex("i_x_asp_net_user_claims_user_id", "asp_net_user_claims", "user_id");
		migrationBuilder.CreateIndex("i_x_asp_net_user_logins_user_id", "asp_net_user_logins", "user_id");
		migrationBuilder.CreateIndex("i_x_asp_net_user_roles_role_id", "asp_net_user_roles", "role_id");
		migrationBuilder.CreateIndex("email_index", "asp_net_users", "normalized_email");
		migrationBuilder.CreateIndex("user_name_index", "asp_net_users", "normalized_user_name", null, unique: true);
		migrationBuilder.CreateIndex("i_x_companies_slug", "companies", "slug", null, unique: true);
		migrationBuilder.CreateIndex("i_x_company_members_tenant_id_user_id", "company_members", new string[2] { "tenant_id", "user_id" }, null, unique: true);
		migrationBuilder.CreateIndex("i_x_company_members_user_id", "company_members", "user_id");
		migrationBuilder.CreateIndex("i_x_projects_team_id", "projects", "team_id");
		migrationBuilder.CreateIndex("i_x_projects_tenant_id_key", "projects", new string[2] { "tenant_id", "key" }, null, unique: true);
		migrationBuilder.CreateIndex("i_x_tasks_assignee_id", "tasks", "assignee_id");
		migrationBuilder.CreateIndex("i_x_tasks_parent_task_id", "tasks", "parent_task_id");
		migrationBuilder.CreateIndex("i_x_tasks_project_id", "tasks", "project_id");
		migrationBuilder.CreateIndex("i_x_tasks_team_id_number", "tasks", new string[2] { "team_id", "number" }, null, unique: true);
		migrationBuilder.CreateIndex("i_x_tasks_workflow_state_id", "tasks", "workflow_state_id");
		migrationBuilder.CreateIndex("i_x_team_issue_counters_tenant_id", "team_issue_counters", "tenant_id");
		migrationBuilder.CreateIndex("i_x_team_members_team_id_user_id", "team_members", new string[2] { "team_id", "user_id" }, null, unique: true);
		migrationBuilder.CreateIndex("i_x_team_members_user_id", "team_members", "user_id");
		migrationBuilder.CreateIndex("i_x_teams_tenant_id_key", "teams", new string[2] { "tenant_id", "key" }, null, unique: true);
		migrationBuilder.CreateIndex("i_x_workflow_states_workflow_id_position", "workflow_states", new string[2] { "workflow_id", "position" });
		migrationBuilder.CreateIndex("i_x_workflows_team_id_is_default", "workflows", new string[2] { "team_id", "is_default" });
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable("asp_net_role_claims");
		migrationBuilder.DropTable("asp_net_user_claims");
		migrationBuilder.DropTable("asp_net_user_logins");
		migrationBuilder.DropTable("asp_net_user_roles");
		migrationBuilder.DropTable("asp_net_user_tokens");
		migrationBuilder.DropTable("companies");
		migrationBuilder.DropTable("company_members");
		migrationBuilder.DropTable("projects");
		migrationBuilder.DropTable("tasks");
		migrationBuilder.DropTable("team_issue_counters");
		migrationBuilder.DropTable("team_members");
		migrationBuilder.DropTable("teams");
		migrationBuilder.DropTable("workflow_states");
		migrationBuilder.DropTable("workflows");
		migrationBuilder.DropTable("asp_net_roles");
		migrationBuilder.DropTable("asp_net_users");
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
			b.ToTable("tasks", (string?)null);
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
