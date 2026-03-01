using MediatR;

namespace MonitoringSystem.Application.UseCases.MetricSettings.GetEnabledMetrics;

public class GetEnabledMetricsQuery : IRequest<Dictionary<string, bool>>
{
    
}