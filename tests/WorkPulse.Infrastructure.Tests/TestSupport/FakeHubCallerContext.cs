using System.Security.Claims;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

namespace WorkPulse.Infrastructure.Tests.TestSupport;

internal sealed class FakeHubCallerContext(Guid? userId) : HubCallerContext
{
	public bool Aborted { get; private set; }

	public override string ConnectionId => "test-connection";

	public override string? UserIdentifier => userId?.ToString();

	public override ClaimsPrincipal? User { get; } = new(new ClaimsIdentity(
		userId.HasValue ? [new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())] : [],
		userId.HasValue ? "test" : null));

	public override IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();

	public override IFeatureCollection Features { get; } = new FeatureCollection();

	public override CancellationToken ConnectionAborted => CancellationToken.None;

	public override void Abort() => Aborted = true;
}

/// <summary>Records group membership so tests can assert what a hub call actually joined.</summary>
internal sealed class RecordingGroupManager : IGroupManager
{
	public List<string> Joined { get; } = [];
	public List<string> Left { get; } = [];

	public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken ct = default)
	{
		Joined.Add(groupName);
		return Task.CompletedTask;
	}

	public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken ct = default)
	{
		Left.Add(groupName);
		return Task.CompletedTask;
	}
}
