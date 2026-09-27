
namespace WorkPulse.Application.IntegrationEvents;

public sealed record TaskStatusChangedIntegrationEvent(Guid TenantId, Guid TaskId, Guid PreviousStateId, Guid NewStateId) : IntegrationEvent;
