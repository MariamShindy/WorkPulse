using System;

namespace WorkPulse.Application.IntegrationEvents;

public sealed record TaskCreatedIntegrationEvent(Guid TenantId, Guid TaskId, Guid TeamId) : IntegrationEvent;
