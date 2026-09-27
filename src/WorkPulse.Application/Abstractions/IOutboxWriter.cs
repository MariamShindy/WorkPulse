namespace WorkPulse.Application.Abstractions;

public interface IOutboxWriter
{
	Task EnqueueAsync<TEvent>(TEvent integrationEvent, CancellationToken ct = default(CancellationToken)) where TEvent : class;
}
