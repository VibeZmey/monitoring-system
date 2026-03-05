using MediatR;

namespace MonitoringSystem.Application.UseCases.Rollups.CreateRollup1M;

public class CreateRollup1MCommand : IRequest<Unit>
{
    public DateTimeOffset MetricTs {get;init;}
}