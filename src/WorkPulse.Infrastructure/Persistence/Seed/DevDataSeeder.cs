using Microsoft.AspNetCore.Identity;
using WorkPulse.Application.Organizations.Services;
using WorkPulse.Domain.Common;

namespace WorkPulse.Infrastructure.Persistence.Seed;

public sealed class DevDataSeeder(
	ApplicationDbContext dbContext,
	UserManager<ApplicationUser> userManager,
	ILogger<DevDataSeeder> logger)
{
	public const string DemoCompanySlug = "workpulse-demo";

	public const string DemoEmail = "demo@workpulse.local";

	public const string DemoPassword = "Demo1234!";

	private readonly DateTime _now = DateTime.UtcNow;

	private readonly Random _random = new(20260704);

	private readonly List<(AuditableEntity Entity, DateTime CreatedAtUtc)> _createdAt = [];

	private DateOnly Today => DateOnly.FromDateTime(_now);

	public async Task SeedAsync(CancellationToken cancellationToken = default)
	{
		if (await dbContext.Companies.AnyAsync(company => company.Slug == DemoCompanySlug, cancellationToken))
		{
			logger.LogInformation("Dev seed skipped: demo company already exists.");
			return;
		}

		logger.LogInformation("Seeding development demo data…");
		Guid companyId = Guid.NewGuid();
		Dictionary<string, SeedUser> users = await CreateUsersAsync(companyId);
		CreateCompany(companyId, users);
		Dictionary<string, SeedTeam> teams = CreateTeams(companyId, users);
		Dictionary<string, Project> projects = CreateProjects(companyId, teams, users);
		Dictionary<string, Epic> epics = CreateEpics(companyId, teams);
		Dictionary<string, Sprint[]> sprints = CreateSprints(companyId, teams);
		Dictionary<string, Label> labels = CreateLabels(companyId);
		Dictionary<string, TaskItem> tasks = CreateTasks(companyId, teams, projects, epics, sprints, labels, users);
		CreateCollaboration(companyId, tasks, users);
		CreateWorkLogs(companyId, tasks, users);
		CreateNotifications(companyId, tasks, users);
		CreateSavedViews(companyId, users);
		CreateAutomationRules(companyId);
		CreateAuditLog(companyId, tasks, teams, users);
		await dbContext.SaveChangesAsync(cancellationToken);
		await ApplyOrganicTimestampsAsync(companyId, cancellationToken);
		logger.LogInformation(
			"Dev seed complete: company '{Slug}', {Users} users, {Teams} teams, {Projects} projects, {Tasks} tasks. Login: {Email} / {Password}",
			DemoCompanySlug,
			users.Count,
			teams.Count,
			projects.Count,
			tasks.Count,
			DemoEmail,
			DemoPassword);
	}

	private async Task<Dictionary<string, SeedUser>> CreateUsersAsync(Guid companyId)
	{
		(string Key, string Email, string First, string Last, CompanyMemberRole Role, int AgeDays)[] userSpecs =
		[
			("demo", DemoEmail, "Demo", "Anderson", CompanyMemberRole.Owner, 58),
			("alice", "alice.johnson@workpulse.local", "Alice", "Johnson", CompanyMemberRole.Admin, 56),
			("bob", "bob.martinez@workpulse.local", "Bob", "Martinez", CompanyMemberRole.Member, 54),
			("carol", "carol.chen@workpulse.local", "Carol", "Chen", CompanyMemberRole.Member, 52),
			("david", "david.kim@workpulse.local", "David", "Kim", CompanyMemberRole.Member, 49),
			("emma", "emma.wilson@workpulse.local", "Emma", "Wilson", CompanyMemberRole.Admin, 47)
		];

		Dictionary<string, SeedUser> usersByKey = [];
		foreach (var (key, email, firstName, lastName, role, ageDays) in userSpecs)
		{
			ApplicationUser user = new ApplicationUser
			{
				Id = Guid.NewGuid(),
				UserName = email,
				Email = email,
				FirstName = firstName,
				LastName = lastName,
				EmailConfirmed = true,
				IsActive = true,
				CurrentTenantId = companyId,
				CreatedAtUtc = _now.AddDays(-ageDays),
				LastLoginAtUtc = _now.AddDays(-_random.Next(0, 5)).AddHours(-_random.Next(0, 12))
			};
			IdentityResult identityResult = await userManager.CreateAsync(user, DemoPassword);
			if (!identityResult.Succeeded)
			{
				throw new InvalidOperationException(
					"Dev seed failed to create user " + email + ": " +
					string.Join("; ", identityResult.Errors.Select(error => error.Description)));
			}
			usersByKey[key] = new SeedUser(key, user, role);
		}
		return usersByKey;
	}

	private void CreateCompany(Guid companyId, Dictionary<string, SeedUser> users)
	{
		Company company = new Company
		{
			Id = companyId,
			Name = "WorkPulse Demo",
			Slug = "workpulse-demo",
			Description = "A fully populated demo workspace showing WorkPulse features.",
			IsActive = true
		};
		dbContext.Companies.Add(company);
		_createdAt.Add((company, _now.AddDays(-58.0)));
		foreach (SeedUser value in users.Values)
		{
			CompanyMember companyMember = new CompanyMember
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				UserId = value.User.Id,
				Role = value.Role,
				IsActive = true
			};
			dbContext.CompanyMembers.Add(companyMember);
			_createdAt.Add((companyMember, value.User.CreatedAtUtc.AddMinutes(30.0)));
		}
	}

	private Dictionary<string, SeedTeam> CreateTeams(Guid companyId, Dictionary<string, SeedUser> users)
	{
		(string, string, string, string, int, (string, TeamMemberRole)[])[] array = new(string, string, string, string, int, (string, TeamMemberRole)[])[3]
		{
			("Engineering", "ENG", "#6366F1", "⚙", 55, new(string, TeamMemberRole)[4]
			{
				("demo", TeamMemberRole.Lead),
				("bob", TeamMemberRole.Member),
				("carol", TeamMemberRole.Member),
				("david", TeamMemberRole.Member)
			}),
			("Design", "DES", "#EC4899", "✏", 53, new(string, TeamMemberRole)[3]
			{
				("alice", TeamMemberRole.Lead),
				("emma", TeamMemberRole.Member),
				("carol", TeamMemberRole.Member)
			}),
			("Marketing", "MKT", "#F59E0B", "\ud83d\udce3", 50, new(string, TeamMemberRole)[3]
			{
				("emma", TeamMemberRole.Lead),
				("alice", TeamMemberRole.Member),
				("demo", TeamMemberRole.Member)
			})
		};
		Dictionary<string, SeedTeam> dictionary = new Dictionary<string, SeedTeam>();
		(string, string, string, string, int, (string, TeamMemberRole)[])[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			(string, string, string, string, int, (string, TeamMemberRole)[]) tuple = array2[i];
			string item = tuple.Item1;
			string item2 = tuple.Item2;
			string item3 = tuple.Item3;
			string item4 = tuple.Item4;
			int item5 = tuple.Item5;
			(string, TeamMemberRole)[] item6 = tuple.Item6;
			DateTime item7 = _now.AddDays(-item5);
			Team team = new Team
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				Name = item,
				Key = item2,
				Description = item + " team of the demo workspace.",
				Color = item3,
				Icon = item4
			};
			dbContext.Teams.Add(team);
			_createdAt.Add((team, item7));
			TeamIssueCounter teamIssueCounter = new TeamIssueCounter
			{
				TeamId = team.Id,
				TenantId = companyId,
				LastNumber = 0
			};
			dbContext.TeamIssueCounters.Add(teamIssueCounter);
			var (workflow, readOnlyList) = DefaultWorkflowFactory.Create(companyId, team.Id);
			dbContext.Workflows.Add(workflow);
			dbContext.WorkflowStates.AddRange(readOnlyList);
			_createdAt.Add((workflow, item7));
			foreach (WorkflowState item10 in readOnlyList)
			{
				_createdAt.Add((item10, item7));
			}
			(string, TeamMemberRole)[] array3 = item6;
			for (int j = 0; j < array3.Length; j++)
			{
				(string, TeamMemberRole) tuple3 = array3[j];
				string item8 = tuple3.Item1;
				TeamMemberRole item9 = tuple3.Item2;
				TeamMember teamMember = new TeamMember
				{
					Id = Guid.NewGuid(),
					TenantId = companyId,
					TeamId = team.Id,
					UserId = users[item8].User.Id,
					Role = item9
				};
				dbContext.TeamMembers.Add(teamMember);
				_createdAt.Add((teamMember, item7.AddHours(2.0)));
			}
			dictionary[item2] = new SeedTeam(team, readOnlyList.ToDictionary((WorkflowState s) => s.Name), teamIssueCounter);
		}
		return dictionary;
	}

	private Dictionary<string, Project> CreateProjects(Guid companyId, Dictionary<string, SeedTeam> teams, Dictionary<string, SeedUser> users)
	{
		(string, string, string, ProjectStatus, string, int, int?, int)[] array = new(string, string, string, ProjectStatus, string, int, int?, int)[6]
		{
			("ENG", "Platform Rewrite", "PLAT", ProjectStatus.Active, "demo", 45, 40, 46),
			("ENG", "Mobile App", "MOB", ProjectStatus.Active, "bob", 30, 25, 32),
			("ENG", "API v2", "APIV2", ProjectStatus.Planned, "carol", -7, 70, 20),
			("DES", "Design System", "DSYS", ProjectStatus.Active, "alice", 40, 20, 42),
			("MKT", "Website Refresh", "WEB", ProjectStatus.Completed, "emma", 55, -5, 52),
			("MKT", "Q3 Campaign", "Q3C", ProjectStatus.Planned, "emma", -10, 80, 12)
		};
		Dictionary<string, Project> dictionary = new Dictionary<string, Project>();
		(string, string, string, ProjectStatus, string, int, int?, int)[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			(string, string, string, ProjectStatus, string, int, int?, int) tuple = array2[i];
			string item = tuple.Item1;
			string item2 = tuple.Item2;
			string item3 = tuple.Item3;
			ProjectStatus item4 = tuple.Item4;
			string item5 = tuple.Item5;
			int item6 = tuple.Item6;
			int? item7 = tuple.Item7;
			int item8 = tuple.Rest.Item1;
			Project project = new Project
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				TeamId = teams[item].Team.Id,
				Name = item2,
				Key = item3,
				Description = item2 + " initiative.",
				Status = item4,
				LeadId = users[item5].User.Id,
				StartDate = Today.AddDays(-item6),
				TargetDate = (item7.HasValue ? new DateOnly?(Today.AddDays(item7.Value)) : ((DateOnly?)null))
			};
			dbContext.Projects.Add(project);
			_createdAt.Add((project, _now.AddDays(-item8)));
			dictionary[item3] = project;
		}
		return dictionary;
	}

	private Dictionary<string, Epic> CreateEpics(Guid companyId, Dictionary<string, SeedTeam> teams)
	{
		(string, string, string, EpicStatus, int)[] array = new(string, string, string, EpicStatus, int)[7]
		{
			("ENG", "auth", "Authentication & Security", EpicStatus.InProgress, 44),
			("ENG", "perf", "Performance Overhaul", EpicStatus.Open, 35),
			("ENG", "dx", "Developer Experience", EpicStatus.Open, 25),
			("DES", "components", "Component Library", EpicStatus.InProgress, 40),
			("DES", "brand", "Brand Refresh", EpicStatus.Done, 48),
			("MKT", "content", "Content Engine", EpicStatus.InProgress, 30),
			("MKT", "launch", "Launch Campaign", EpicStatus.Open, 15)
		};
		Dictionary<string, Epic> dictionary = new Dictionary<string, Epic>();
		(string, string, string, EpicStatus, int)[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			(string, string, string, EpicStatus, int) tuple = array2[i];
			string item = tuple.Item1;
			string item2 = tuple.Item2;
			string item3 = tuple.Item3;
			EpicStatus item4 = tuple.Item4;
			int item5 = tuple.Item5;
			Epic epic = new Epic
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				TeamId = teams[item].Team.Id,
				Title = item3,
				Description = "Epic: " + item3 + ".",
				Status = item4
			};
			dbContext.Epics.Add(epic);
			_createdAt.Add((epic, _now.AddDays(-item5)));
			dictionary[item + ":" + item2] = epic;
		}
		return dictionary;
	}

	private Dictionary<string, Sprint[]> CreateSprints(Guid companyId, Dictionary<string, SeedTeam> teams)
	{
		Dictionary<string, Sprint[]> dictionary = new Dictionary<string, Sprint[]>();
		foreach (KeyValuePair<string, SeedTeam> team in teams)
		{
			team.Deconstruct(out var key, out var value);
			string text = key;
			SeedTeam seedTeam = value;
			Sprint sprint = new Sprint
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				TeamId = seedTeam.Team.Id,
				Name = text + " Sprint 1",
				Goal = "Ship the foundation work.",
				StartDate = Today.AddDays(-26),
				EndDate = Today.AddDays(-12),
				Status = SprintStatus.Completed
			};
			Sprint sprint2 = new Sprint
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				TeamId = seedTeam.Team.Id,
				Name = text + " Sprint 2",
				Goal = "Deliver the current milestone.",
				StartDate = Today.AddDays(-5),
				EndDate = Today.AddDays(8),
				Status = SprintStatus.Active
			};
			dbContext.Sprints.AddRange(sprint, sprint2);
			_createdAt.Add((sprint, _now.AddDays(-28.0)));
			_createdAt.Add((sprint2, _now.AddDays(-7.0)));
			dictionary[text] = new Sprint[2] { sprint, sprint2 };
		}
		return dictionary;
	}

	private Dictionary<string, Label> CreateLabels(Guid companyId)
	{
		(string, string)[] array = new(string, string)[8]
		{
			("bug", "#EF4444"),
			("feature", "#3B82F6"),
			("tech-debt", "#F97316"),
			("urgent", "#DC2626"),
			("design", "#EC4899"),
			("docs", "#8B5CF6"),
			("backend", "#10B981"),
			("frontend", "#06B6D4")
		};
		Dictionary<string, Label> dictionary = new Dictionary<string, Label>();
		(string, string)[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			(string, string) tuple = array2[i];
			string item = tuple.Item1;
			string item2 = tuple.Item2;
			Label label = new Label
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				Name = item,
				Color = item2
			};
			dbContext.Labels.Add(label);
			_createdAt.Add((label, _now.AddDays(-45 - _random.Next(0, 6))));
			dictionary[item] = label;
		}
		return dictionary;
	}

	private Dictionary<string, TaskItem> CreateTasks(Guid companyId, Dictionary<string, SeedTeam> teams, Dictionary<string, Project> projects, Dictionary<string, Epic> epics, Dictionary<string, Sprint[]> sprints, Dictionary<string, Label> labels, Dictionary<string, SeedUser> users)
	{
		TaskSeed[] array = new TaskSeed[50];
		int? points = 5;
		decimal? estHours = 12;
		string[] labels2 = new string[1] { "backend" };
		array[0] = new TaskSeed("ENG", "Set up CI pipeline for platform services", "Done", TaskPriority.High, "PLAT", "ENG:dx", 0, "bob", null, points, estHours, null, Blocked: false, null, labels2, null, 40);
		int? points2 = 8;
		decimal? estHours2 = 24;
		labels2 = new string[2] { "backend", "tech-debt" };
		array[1] = new TaskSeed("ENG", "Migrate user store to new identity schema", "Done", TaskPriority.Urgent, "PLAT", "ENG:auth", 0, "carol", null, points2, estHours2, null, Blocked: false, null, labels2, null, 38);
		int? points3 = 5;
		decimal? estHours3 = 10;
		labels2 = new string[1] { "backend" };
		array[2] = new TaskSeed("ENG", "Implement refresh token rotation", "Done", TaskPriority.High, "PLAT", "ENG:auth", 0, "demo", null, points3, estHours3, null, Blocked: false, null, labels2, null, 36);
		int? points4 = 2;
		decimal? estHours4 = 4;
		labels2 = new string[2] { "bug", "frontend" };
		array[3] = new TaskSeed("ENG", "Fix login redirect loop on expired session", "Done", TaskPriority.Urgent, "PLAT", "ENG:auth", 0, "david", null, points4, estHours4, null, Blocked: false, null, labels2, null, 34);
		int? points5 = 3;
		decimal? estHours5 = 8;
		labels2 = new string[1] { "backend" };
		array[4] = new TaskSeed("ENG", "Add request tracing across services", "Done", TaskPriority.Medium, "PLAT", "ENG:perf", 0, "bob", null, points5, estHours5, null, Blocked: false, null, labels2, null, 32);
		int? points6 = 2;
		decimal? estHours6 = 3;
		labels2 = new string[1] { "docs" };
		array[5] = new TaskSeed("ENG", "Document local environment setup", "Done", TaskPriority.Low, null, "ENG:dx", 0, "carol", null, points6, estHours6, null, Blocked: false, null, labels2, null, 30);
		array[6] = new TaskSeed("ENG", "Implement two-factor authentication", "In Progress", TaskPriority.Urgent, "PLAT", "ENG:auth", 1, "carol", new string[1] { "david" }, 8, 30, 6, Blocked: false, null, new string[2] { "backend", "feature" }, null, 12);
		array[7] = new TaskSeed("ENG", "Optimize dashboard query performance", "In Progress", TaskPriority.High, "PLAT", "ENG:perf", 1, "bob", null, 5, 16, 4, Blocked: false, null, new string[2] { "backend", "tech-debt" }, null, 10);
		array[8] = new TaskSeed("ENG", "Mobile push notification support", "In Review", TaskPriority.High, "MOB", null, 1, "david", null, 5, 20, 3, Blocked: false, null, new string[1] { "feature" }, null, 14);
		array[9] = new TaskSeed("ENG", "Fix task board drag-and-drop flicker", "In Review", TaskPriority.Medium, "MOB", null, 1, "demo", null, 2, 6, -2, Blocked: false, null, new string[2] { "bug", "frontend" }, null, 9);
		array[10] = new TaskSeed("ENG", "Rate limiting for public API", "Todo", TaskPriority.High, "APIV2", "ENG:auth", 1, "bob", null, 3, 10, 7, Blocked: false, null, new string[1] { "backend" }, null, 8);
		array[11] = new TaskSeed("ENG", "Upgrade EF Core and fix breaking changes", "Todo", TaskPriority.Medium, null, "ENG:dx", 1, "carol", null, 3, 8, 8, Blocked: false, null, new string[1] { "tech-debt" }, null, 7);
		array[12] = new TaskSeed("ENG", "Fix memory leak in realtime hub", "In Progress", TaskPriority.Urgent, "PLAT", "ENG:perf", 1, "demo", null, 5, 12, -1, Blocked: true, "Waiting on a heap dump from the ops team.", new string[2] { "bug", "backend" }, null, 11);
		int? points7 = 3;
		labels2 = new string[2] { "backend", "docs" };
		array[13] = new TaskSeed("ENG", "Design API v2 versioning strategy", "Backlog", TaskPriority.Medium, "APIV2", "ENG:dx", -1, "demo", null, points7, null, null, Blocked: false, null, labels2, null, 18);
		int? points8 = 5;
		labels2 = new string[1] { "backend" };
		array[14] = new TaskSeed("ENG", "Evaluate GraphQL gateway", "Backlog", TaskPriority.Low, "APIV2", null, -1, null, null, points8, null, null, Blocked: false, null, labels2, null, 16);
		int? points9 = 8;
		int? dueInDays = 30;
		labels2 = new string[1] { "feature" };
		array[15] = new TaskSeed("ENG", "Offline mode for mobile app", "Backlog", TaskPriority.Medium, "MOB", null, -1, null, null, points9, null, dueInDays, Blocked: false, null, labels2, null, 15);
		array[16] = new TaskSeed("ENG", "Audit npm dependencies for vulnerabilities", "Todo", TaskPriority.High, null, "ENG:auth", -1, "david", null, 2, 4, -4, Blocked: false, null, new string[2] { "urgent", "tech-debt" }, null, 22);
		int? points10 = 3;
		labels2 = new string[1] { "backend" };
		array[17] = new TaskSeed("ENG", "Automate database backup verification", "Backlog", TaskPriority.Low, null, null, -1, null, null, points10, null, null, Blocked: false, null, labels2, null, 13);
		int? points11 = 5;
		labels2 = new string[2] { "tech-debt", "backend" };
		array[18] = new TaskSeed("ENG", "Migrate legacy cron jobs to Hangfire", "Backlog", TaskPriority.Medium, null, "ENG:perf", -1, null, null, points11, null, null, Blocked: true, "Blocked until the platform rewrite reaches milestone 2.", labels2);
		int? points12 = 3;
		labels2 = new string[1] { "feature" };
		array[19] = new TaskSeed("ENG", "Spike: server-driven UI for mobile", "Canceled", TaskPriority.Low, "MOB", null, -1, null, null, points12, null, null, Blocked: false, null, labels2, null, 26);
		array[20] = new TaskSeed("ENG", "Single sign-on integration", "In Progress", TaskPriority.High, "PLAT", "ENG:auth", -1, "carol", null, 13, 40, 14, Blocked: false, null, new string[2] { "feature", "backend" }, null, 17);
		int? points13 = 3;
		decimal? estHours7 = 8;
		labels2 = new string[1] { "backend" };
		array[21] = new TaskSeed("ENG", "SSO: SAML metadata endpoint", "Done", TaskPriority.High, "PLAT", "ENG:auth", -1, "carol", null, points13, estHours7, null, Blocked: false, null, labels2, "Single sign-on integration", 15);
		array[22] = new TaskSeed("ENG", "SSO: Okta end-to-end test", "Todo", TaskPriority.Medium, "PLAT", "ENG:auth", -1, "david", null, 2, 6, 10, Blocked: false, null, new string[1] { "backend" }, "Single sign-on integration", 14);
		int? points14 = 3;
		decimal? estHours8 = 8;
		labels2 = new string[1] { "design" };
		array[23] = new TaskSeed("DES", "Define color tokens for dark mode", "Done", TaskPriority.High, "DSYS", "DES:components", 0, "alice", null, points14, estHours8, null, Blocked: false, null, labels2, null, 38);
		int? points15 = 3;
		decimal? estHours9 = 10;
		labels2 = new string[2] { "design", "frontend" };
		array[24] = new TaskSeed("DES", "Build button component variants", "Done", TaskPriority.Medium, "DSYS", "DES:components", 0, "emma", null, points15, estHours9, null, Blocked: false, null, labels2, null, 35);
		int? points16 = 5;
		decimal? estHours10 = 16;
		labels2 = new string[1] { "design" };
		array[25] = new TaskSeed("DES", "New logo exploration", "Done", TaskPriority.Medium, null, "DES:brand", 0, "alice", null, points16, estHours10, null, Blocked: false, null, labels2, null, 42);
		array[26] = new TaskSeed("DES", "Design empty states for boards", "In Progress", TaskPriority.Medium, "DSYS", "DES:components", 1, "emma", null, 3, 8, 5, Blocked: false, null, new string[1] { "design" }, null, 10);
		array[27] = new TaskSeed("DES", "Accessibility audit of form controls", "In Progress", TaskPriority.High, "DSYS", null, 1, "carol", new string[1] { "alice" }, 5, 14, 2, Blocked: false, null, new string[2] { "design", "urgent" }, null, 9);
		array[28] = new TaskSeed("DES", "Iconography refresh proposal", "In Review", TaskPriority.Low, null, "DES:brand", 1, "alice", null, 2, 6, 6, Blocked: false, null, new string[1] { "design" }, null, 8);
		int? points17 = 3;
		dueInDays = 9;
		labels2 = new string[2] { "design", "docs" };
		array[29] = new TaskSeed("DES", "Data table component spec", "Todo", TaskPriority.Medium, "DSYS", "DES:components", 1, "emma", null, points17, null, dueInDays, Blocked: false, null, labels2, null, 6);
		int? points18 = 5;
		dueInDays = 20;
		labels2 = new string[1] { "design" };
		array[30] = new TaskSeed("DES", "Marketing site illustrations", "Todo", TaskPriority.Low, null, "DES:brand", -1, "alice", null, points18, null, dueInDays, Blocked: false, null, labels2, null, 12);
		int? points19 = 3;
		labels2 = new string[2] { "design", "docs" };
		array[31] = new TaskSeed("DES", "Motion guidelines for micro-interactions", "Backlog", TaskPriority.Low, "DSYS", "DES:components", -1, null, null, points19, null, null, Blocked: false, null, labels2, null, 11);
		int? points20 = 8;
		labels2 = new string[2] { "design", "feature" };
		array[32] = new TaskSeed("DES", "Mobile app onboarding flow", "Backlog", TaskPriority.Medium, null, null, -1, null, null, points20, null, null, Blocked: false, null, labels2, null, 14);
		array[33] = new TaskSeed("DES", "Dashboard chart styling pass", "In Review", TaskPriority.Medium, "DSYS", null, 1, "carol", null, 2, 5, -3, Blocked: false, null, new string[2] { "design", "frontend" }, null, 13);
		int? points21 = 1;
		labels2 = new string[1] { "design" };
		array[34] = new TaskSeed("DES", "Print stylesheet for reports", "Canceled", TaskPriority.None, null, null, -1, null, null, points21, null, null, Blocked: false, null, labels2, null, 24);
		int? points22 = 2;
		decimal? estHours11 = 6;
		labels2 = new string[1] { "docs" };
		array[35] = new TaskSeed("MKT", "Publish launch blog post", "Done", TaskPriority.High, "WEB", "MKT:content", 0, "emma", null, points22, estHours11, null, Blocked: false, null, labels2, null, 30);
		int? points23 = 2;
		decimal? estHours12 = 4;
		labels2 = new string[1] { "docs" };
		array[36] = new TaskSeed("MKT", "Refresh pricing page copy", "Done", TaskPriority.Medium, "WEB", "MKT:content", 0, "alice", null, points23, estHours12, null, Blocked: false, null, labels2, null, 28);
		int? points24 = 3;
		decimal? estHours13 = 8;
		labels2 = new string[1] { "docs" };
		array[37] = new TaskSeed("MKT", "SEO audit of documentation site", "Done", TaskPriority.Medium, "WEB", null, 0, "demo", null, points24, estHours13, null, Blocked: false, null, labels2, null, 26);
		array[38] = new TaskSeed("MKT", "Q3 campaign creative brief", "In Progress", TaskPriority.High, "Q3C", "MKT:launch", 1, "emma", new string[1] { "alice" }, 3, 10, 3, Blocked: false, null, new string[1] { "feature" }, null, 8);
		array[39] = new TaskSeed("MKT", "Draft July newsletter", "In Progress", TaskPriority.Medium, null, "MKT:content", 1, "alice", null, 2, 5, 1, Blocked: false, null, new string[1] { "docs" }, null, 6);
		array[40] = new TaskSeed("MKT", "Customer case study: Acme Corp", "In Review", TaskPriority.Medium, null, "MKT:content", 1, "demo", null, 3, 8, 5, Blocked: false, null, new string[1] { "docs" }, null, 9);
		int? points25 = 2;
		dueInDays = 7;
		labels2 = new string[1] { "feature" };
		array[41] = new TaskSeed("MKT", "Social media calendar for launch week", "Todo", TaskPriority.High, "Q3C", "MKT:launch", 1, "emma", null, points25, null, dueInDays, Blocked: false, null, labels2, null, 5);
		array[42] = new TaskSeed("MKT", "Update webinar landing page", "Todo", TaskPriority.Low, "WEB", null, -1, "alice", null, 1, 3, -6, Blocked: false, null, new string[1] { "frontend" }, null, 16);
		int? points26 = 3;
		labels2 = new string[1] { "feature" };
		array[43] = new TaskSeed("MKT", "Partner co-marketing outreach", "Backlog", TaskPriority.Medium, "Q3C", "MKT:launch", -1, null, null, points26, null, null, Blocked: false, null, labels2, null, 10);
		int? points27 = 2;
		labels2 = new string[1] { "docs" };
		array[44] = new TaskSeed("MKT", "Competitor messaging analysis", "Backlog", TaskPriority.Low, null, "MKT:content", -1, null, null, points27, null, null, Blocked: false, null, labels2, null, 12);
		int? points28 = 2;
		labels2 = new string[1] { "design" };
		array[45] = new TaskSeed("MKT", "Press kit refresh", "Backlog", TaskPriority.Low, null, null, -1, null, null, points28, null, null, Blocked: false, null, labels2, null, 7);
		int? points29 = 2;
		decimal? estHours14 = 5;
		labels2 = new string[1] { "backend" };
		array[46] = new TaskSeed("ENG", "Instrument feature-flag usage metrics", "Todo", TaskPriority.Low, "PLAT", "ENG:perf", -1, null, null, points29, estHours14, null, Blocked: false, null, labels2, null, 5);
		array[47] = new TaskSeed("ENG", "Harden password reset flow", "In Progress", TaskPriority.High, "PLAT", "ENG:auth", 1, "david", null, 3, 9, 4, Blocked: false, null, new string[2] { "backend", "bug" }, null, 6);
		int? points30 = 2;
		dueInDays = 12;
		labels2 = new string[2] { "design", "feature" };
		array[48] = new TaskSeed("DES", "Design workspace switcher", "Todo", TaskPriority.Medium, "DSYS", "DES:components", -1, "emma", null, points30, null, dueInDays, Blocked: false, null, labels2, null, 4);
		array[49] = new TaskSeed("MKT", "Record product tour video", "Todo", TaskPriority.Medium, "Q3C", "MKT:launch", -1, "demo", null, 5, 12, 15, Blocked: false, null, new string[1] { "feature" }, null, 3);
		TaskSeed[] array2 = array;
		Dictionary<string, TaskItem> dictionary = new Dictionary<string, TaskItem>();
		TaskSeed[] array3 = array2;
		foreach (TaskSeed taskSeed in array3)
		{
			SeedTeam seedTeam = teams[taskSeed.Team];
			seedTeam.Counter.LastNumber++;
			List<Guid> list = new List<Guid>();
			if (taskSeed.Assignee != null)
			{
				list.Add(users[taskSeed.Assignee].User.Id);
			}
			string[] array4 = taskSeed.CoAssignees ?? Array.Empty<string>();
			foreach (string key in array4)
			{
				list.Add(users[key].User.Id);
			}
			DateTime item = _now.AddDays(-taskSeed.AgeDays).AddHours(_random.Next(1, 9));
			string team = taskSeed.Team;
			if (1 == 0)
			{
			}
			string text = ((team == "DES") ? "alice" : ((!(team == "MKT")) ? "demo" : "emma"));
			if (1 == 0)
			{
			}
			string key2 = text;
			TaskItem taskItem = new TaskItem
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				TeamId = seedTeam.Team.Id,
				ProjectId = ((taskSeed.Project == null) ? ((Guid?)null) : new Guid?(projects[taskSeed.Project].Id)),
				WorkflowStateId = seedTeam.StatesByName[taskSeed.State].Id,
				Number = seedTeam.Counter.LastNumber,
				Title = taskSeed.Title,
				Description = taskSeed.Title + ". Seeded demo task with realistic context for the " + seedTeam.Team.Name + " team.",
				Priority = taskSeed.Priority,
				AssigneeId = ((list.Count > 0) ? new Guid?(list[0]) : ((Guid?)null)),
				CreatorId = users[key2].User.Id,
				DueDate = (taskSeed.DueInDays.HasValue ? new DateOnly?(Today.AddDays(taskSeed.DueInDays.Value)) : ((DateOnly?)null)),
				ParentTaskId = ((taskSeed.Parent == null) ? ((Guid?)null) : new Guid?(dictionary[taskSeed.Parent].Id)),
				SortOrder = seedTeam.Counter.LastNumber,
				StoryPoints = taskSeed.Points,
				EstimatedHours = taskSeed.EstHours,
				IsBlocked = taskSeed.Blocked,
				BlockedReason = taskSeed.BlockedReason,
				EpicId = ((taskSeed.Epic == null) ? ((Guid?)null) : new Guid?(epics[taskSeed.Epic].Id)),
				SprintId = ((taskSeed.Sprint >= 0) ? new Guid?(sprints[taskSeed.Team][taskSeed.Sprint].Id) : ((Guid?)null))
			};
			dbContext.TaskItems.Add(taskItem);
			_createdAt.Add((taskItem, item));
			foreach (Guid item2 in list)
			{
				TaskAssignee taskAssignee = new TaskAssignee
				{
					Id = Guid.NewGuid(),
					TenantId = companyId,
					TaskId = taskItem.Id,
					UserId = item2
				};
				dbContext.TaskAssignees.Add(taskAssignee);
				_createdAt.Add((taskAssignee, item.AddHours(1.0)));
			}
			string[] array5 = taskSeed.Labels ?? Array.Empty<string>();
			foreach (string key3 in array5)
			{
				TaskLabel taskLabel = new TaskLabel
				{
					Id = Guid.NewGuid(),
					TenantId = companyId,
					TaskId = taskItem.Id,
					LabelId = labels[key3].Id
				};
				dbContext.TaskLabels.Add(taskLabel);
				_createdAt.Add((taskLabel, item.AddHours(1.0)));
			}
			dictionary[taskSeed.Title] = taskItem;
		}
		CreateDependencies(companyId, dictionary);
		return dictionary;
	}

	private void CreateDependencies(Guid companyId, Dictionary<string, TaskItem> tasks)
	{
		(string, string, TaskDependencyType)[] array = new(string, string, TaskDependencyType)[4]
		{
			("Rate limiting for public API", "Design API v2 versioning strategy", TaskDependencyType.BlockedBy),
			("Migrate legacy cron jobs to Hangfire", "Optimize dashboard query performance", TaskDependencyType.BlockedBy),
			("Social media calendar for launch week", "Q3 campaign creative brief", TaskDependencyType.BlockedBy),
			("Implement two-factor authentication", "Single sign-on integration", TaskDependencyType.RelatesTo)
		};
		(string, string, TaskDependencyType)[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			(string, string, TaskDependencyType) tuple = array2[i];
			string item = tuple.Item1;
			string item2 = tuple.Item2;
			TaskDependencyType item3 = tuple.Item3;
			TaskDependency taskDependency = new TaskDependency
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				TaskId = tasks[item].Id,
				DependsOnTaskId = tasks[item2].Id,
				Type = item3
			};
			dbContext.TaskDependencies.Add(taskDependency);
			_createdAt.Add((taskDependency, _now.AddDays(-_random.Next(3, 15))));
		}
	}

	private void CreateCollaboration(Guid companyId, Dictionary<string, TaskItem> tasks, Dictionary<string, SeedUser> users)
	{
		(string, string, string)[] array = new(string, string, string)[12]
		{
			("Implement two-factor authentication", "demo", "Let's start with TOTP and add WebAuthn in a follow-up."),
			("Implement two-factor authentication", "carol", "Agreed. I have the enrollment flow working locally."),
			("Optimize dashboard query performance", "bob", "Profiling shows the workload query is missing an index on (tenant_id, assignee_id)."),
			("Optimize dashboard query performance", "demo", "Nice find — can you add it in the next migration?"),
			("Fix memory leak in realtime hub", "demo", "Reproduced under load. Suspect group subscriptions are never disposed."),
			("Mobile push notification support", "david", "APNs certificates are in the vault; FCM config is checked in."),
			("Accessibility audit of form controls", "alice", "Focus rings fail contrast in dark mode — logging fixes as subtasks of the audit."),
			("Design empty states for boards", "emma", "First drafts are in Figma, link in the description."),
			("Q3 campaign creative brief", "emma", "Target persona section still needs input from sales."),
			("Customer case study: Acme Corp", "demo", "Quotes approved by the customer, ready for final review."),
			("Draft July newsletter", "alice", "Waiting on the product update section before sending to review."),
			("Single sign-on integration", "carol", "SAML metadata endpoint is done; Okta test env requested.")
		};
		(string, string, string)[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			(string, string, string) tuple = array2[i];
			string item = tuple.Item1;
			string item2 = tuple.Item2;
			string item3 = tuple.Item3;
			TaskItem taskItem = tasks[item];
			TaskComment taskComment = new TaskComment
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				TaskId = taskItem.Id,
				AuthorId = users[item2].User.Id,
				Body = item3
			};
			dbContext.TaskComments.Add(taskComment);
			_createdAt.Add((taskComment, _now.AddDays(-_random.Next(1, 8)).AddHours(_random.Next(1, 10))));
		}
		(string, string)[] array3 = new(string, string)[7]
		{
			("Implement two-factor authentication", "demo"),
			("Implement two-factor authentication", "bob"),
			("Fix memory leak in realtime hub", "bob"),
			("Optimize dashboard query performance", "carol"),
			("Q3 campaign creative brief", "demo"),
			("Accessibility audit of form controls", "emma"),
			("Single sign-on integration", "demo")
		};
		(string, string)[] array4 = array3;
		for (int j = 0; j < array4.Length; j++)
		{
			(string, string) tuple2 = array4[j];
			string item4 = tuple2.Item1;
			string item5 = tuple2.Item2;
			TaskWatcher taskWatcher = new TaskWatcher
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				TaskId = tasks[item4].Id,
				UserId = users[item5].User.Id
			};
			dbContext.TaskWatchers.Add(taskWatcher);
			_createdAt.Add((taskWatcher, _now.AddDays(-_random.Next(2, 12))));
		}
	}

	private void CreateWorkLogs(Guid companyId, Dictionary<string, TaskItem> tasks, Dictionary<string, SeedUser> users)
	{
		(string, string, decimal, int, string)[] array = new(string, string, decimal, int, string)[15]
		{
			("Migrate user store to new identity schema", "carol", 6.0m, 20, "Schema mapping and data migration script."),
			("Migrate user store to new identity schema", "carol", 8.0m, 18, "Backfill run and verification."),
			("Implement refresh token rotation", "demo", 4.5m, 17, "Token store changes."),
			("Fix login redirect loop on expired session", "david", 3.0m, 16, "Repro + fix + regression test."),
			("Set up CI pipeline for platform services", "bob", 7.5m, 22, "Pipeline definition and caching."),
			("Implement two-factor authentication", "carol", 5.0m, 3, "TOTP enrollment flow."),
			("Implement two-factor authentication", "david", 4.0m, 2, "Recovery codes."),
			("Optimize dashboard query performance", "bob", 6.5m, 2, "Query profiling and index work."),
			("Mobile push notification support", "david", 8.0m, 4, "Push token registration on both platforms."),
			("Define color tokens for dark mode", "alice", 5.5m, 19, "Token definitions and Figma sync."),
			("Build button component variants", "emma", 6.0m, 15, "All variants plus stories."),
			("Accessibility audit of form controls", "carol", 4.0m, 1, "Keyboard navigation pass."),
			("Publish launch blog post", "emma", 3.5m, 14, "Draft, edits, and publishing."),
			("Q3 campaign creative brief", "emma", 4.0m, 2, "Brief outline and moodboard."),
			("Harden password reset flow", "david", 3.5m, 1, "Token expiry + rate limit.")
		};
		Dictionary<Guid, decimal> dictionary = new Dictionary<Guid, decimal>();
		(string, string, decimal, int, string)[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			(string, string, decimal, int, string) tuple = array2[i];
			string item = tuple.Item1;
			string item2 = tuple.Item2;
			decimal item3 = tuple.Item3;
			int item4 = tuple.Item4;
			string item5 = tuple.Item5;
			TaskItem taskItem = tasks[item];
			WorkLog workLog = new WorkLog
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				TaskId = taskItem.Id,
				UserId = users[item2].User.Id,
				Hours = item3,
				Description = item5,
				LoggedDate = Today.AddDays(-item4)
			};
			dbContext.WorkLogs.Add(workLog);
			_createdAt.Add((workLog, _now.AddDays(-item4).AddHours(18.0)));
			dictionary[taskItem.Id] = dictionary.GetValueOrDefault(taskItem.Id) + item3;
		}
		foreach (TaskItem value2 in tasks.Values)
		{
			if (dictionary.TryGetValue(value2.Id, out var value))
			{
				value2.LoggedHours = value;
			}
		}
	}

	private void CreateNotifications(Guid companyId, Dictionary<string, TaskItem> tasks, Dictionary<string, SeedUser> users)
	{
		(string, NotificationType, string, string, string, string?, bool, int)[] array = new(string, NotificationType, string, string, string, string?, bool, int)[6]
		{
			("demo", NotificationType.TaskComment, "New comment on ENG task", "Carol commented on 'Implement two-factor authentication'.", "Implement two-factor authentication", "carol", false, 1),
			("demo", NotificationType.DeadlineReminder, "Task due soon", "'Q3 campaign creative brief' is due in 3 days.", "Q3 campaign creative brief", null, false, 0),
			("demo", NotificationType.TaskStatusChanged, "Task moved to In Review", "'Mobile push notification support' was moved to In Review.", "Mobile push notification support", "david", true, 2),
			("carol", NotificationType.TaskAssigned, "You were assigned a task", "Demo assigned you 'Implement two-factor authentication'.", "Implement two-factor authentication", "demo", true, 6),
			("bob", NotificationType.TaskMention, "You were mentioned", "Demo mentioned you on 'Fix memory leak in realtime hub'.", "Fix memory leak in realtime hub", "demo", false, 3),
			("emma", NotificationType.DeadlineReminder, "Task overdue", "'Update webinar landing page' is overdue.", "Update webinar landing page", null, false, 1)
		};
		(string, NotificationType, string, string, string, string?, bool, int)[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			(string, NotificationType, string, string, string, string?, bool, int) tuple = array2[i];
			string item = tuple.Item1;
			NotificationType item2 = tuple.Item2;
			string item3 = tuple.Item3;
			string item4 = tuple.Item4;
			string item5 = tuple.Item5;
			string? item6 = tuple.Item6;
			bool item7 = tuple.Item7;
			int item8 = tuple.Rest.Item1;
			Notification notification = new Notification
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				UserId = users[item].User.Id,
				Type = item2,
				Title = item3,
				Body = item4,
				IsRead = item7,
				RelatedEntityType = ((item5 == null) ? null : "Task"),
				RelatedEntityId = ((item5 == null) ? ((Guid?)null) : new Guid?(tasks[item5].Id)),
				ActorId = ((item6 == null) ? ((Guid?)null) : new Guid?(users[item6].User.Id))
			};
			dbContext.Notifications.Add(notification);
			_createdAt.Add((notification, _now.AddDays(-item8).AddHours(-_random.Next(1, 6))));
		}
	}

	private void CreateSavedViews(Guid companyId, Dictionary<string, SeedUser> users)
	{
		(string, string, SavedViewEntityType, string, bool)[] array = new(string, string, SavedViewEntityType, string, bool)[4]
		{
			("demo", "My urgent tasks", SavedViewEntityType.Tasks, "{\"priority\":\"Urgent\",\"assignee\":\"me\"}", false),
			("demo", "Overdue work", SavedViewEntityType.Tasks, "{\"due\":\"overdue\"}", true),
			("alice", "Design in review", SavedViewEntityType.Tasks, "{\"team\":\"DES\",\"state\":\"In Review\"}", true),
			("emma", "Active projects", SavedViewEntityType.Projects, "{\"status\":\"Active\"}", false)
		};
		(string, string, SavedViewEntityType, string, bool)[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			(string, string, SavedViewEntityType, string, bool) tuple = array2[i];
			string item = tuple.Item1;
			string item2 = tuple.Item2;
			SavedViewEntityType item3 = tuple.Item3;
			string item4 = tuple.Item4;
			bool item5 = tuple.Item5;
			SavedView savedView = new SavedView
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				UserId = users[item].User.Id,
				Name = item2,
				EntityType = item3,
				FiltersJson = item4,
				SortJson = "{\"sortBy\":\"createdAt\",\"desc\":true}",
				IsShared = item5
			};
			dbContext.SavedViews.Add(savedView);
			_createdAt.Add((savedView, _now.AddDays(-_random.Next(5, 25))));
		}
	}

	private void CreateAutomationRules(Guid companyId)
	{
		AutomationRule automationRule = new AutomationRule
		{
			Id = Guid.NewGuid(),
			TenantId = companyId,
			Name = "Notify team on urgent task creation",
			TriggerType = AutomationTriggerType.TaskCreated,
			TriggerConfigJson = "{\"priority\":\"Urgent\"}",
			ActionType = AutomationActionType.SendNotification,
			ActionConfigJson = "{\"audience\":\"team\"}",
			IsEnabled = true
		};
		AutomationRule automationRule2 = new AutomationRule
		{
			Id = Guid.NewGuid(),
			TenantId = companyId,
			Name = "Raise priority when a task is blocked",
			TriggerType = AutomationTriggerType.TaskStatusChanged,
			TriggerConfigJson = "{\"toState\":\"Blocked\"}",
			ActionType = AutomationActionType.UpdatePriority,
			ActionConfigJson = "{\"priority\":\"High\"}",
			IsEnabled = false
		};
		dbContext.AutomationRules.AddRange(automationRule, automationRule2);
		_createdAt.Add((automationRule, _now.AddDays(-33.0)));
		_createdAt.Add((automationRule2, _now.AddDays(-21.0)));
	}

	private void CreateAuditLog(Guid companyId, Dictionary<string, TaskItem> tasks, Dictionary<string, SeedTeam> teams, Dictionary<string, SeedUser> users)
	{
		(string, Guid, string, string, int)[] array = new(string, Guid, string, string, int)[4]
		{
			("Team", teams["ENG"].Team.Id, "Created", "demo", 55),
			("Task", tasks["Implement two-factor authentication"].Id, "Created", "demo", 12),
			("Task", tasks["Fix memory leak in realtime hub"].Id, "Updated", "demo", 4),
			("Task", tasks["Q3 campaign creative brief"].Id, "Created", "emma", 8)
		};
		(string, Guid, string, string, int)[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			(string, Guid, string, string, int) tuple = array2[i];
			string item = tuple.Item1;
			Guid item2 = tuple.Item2;
			string item3 = tuple.Item3;
			string item4 = tuple.Item4;
			int item5 = tuple.Item5;
			AuditLogEntry auditLogEntry = new AuditLogEntry
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				EntityType = item,
				EntityId = item2,
				Action = item3,
				UserId = users[item4].User.Id,
				Timestamp = _now.AddDays(-item5)
			};
			dbContext.AuditLogEntries.Add(auditLogEntry);
			_createdAt.Add((auditLogEntry, _now.AddDays(-item5)));
		}
	}

	private async Task ApplyOrganicTimestampsAsync(Guid companyId, CancellationToken ct)
	{
		foreach (var item in _createdAt)
		{
			AuditableEntity entity = item.Entity;
			DateTime createdAtUtc = item.CreatedAtUtc;
			entity.CreatedAtUtc = createdAtUtc;
		}
		await dbContext.SaveChangesAsync(ct);
		await dbContext.Database.ExecuteSqlAsync($"UPDATE tasks\r\nSET updated_at_utc = LEAST(\r\n    now() AT TIME ZONE 'utc',\r\n    created_at_utc + (random() * interval '9 days') + interval '4 hours')\r\nWHERE tenant_id = {companyId}", ct);
	}
}
