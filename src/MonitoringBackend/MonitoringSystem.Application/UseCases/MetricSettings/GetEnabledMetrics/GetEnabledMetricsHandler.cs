using MediatR;
using Microsoft.EntityFrameworkCore;
using MonitoringSystem.Application.Interfaces;
using MonitoringSystem.Domain.Models;

namespace MonitoringSystem.Application.UseCases.MetricSettings.GetEnabledMetrics;

public class GetEnabledMetricsHandler(IAppDbContext context)
    : IRequestHandler<GetEnabledMetricsQuery, Dictionary<string, bool>>
{
    public async Task<Dictionary<string, bool>> Handle(GetEnabledMetricsQuery request, CancellationToken cancellationToken)
    {
        List<string> metricsNames =
            await context.MetricSettings
                .Where(s => s.IsEnabled == true)
                .Select(s => s.MetricName)
                .ToListAsync(cancellationToken);
        
        Dictionary<string, bool>  enabledMetrics = new Dictionary<string, bool>();
        foreach (var metric in Enum.GetValues<MetricName>())
        {
            var metricName = metric.ToString();
            if(metricsNames.Contains(metricName)) 
            {
                enabledMetrics.Add(metricName, true);
            }
            else
            {
                enabledMetrics.Add(metricName, false);
            }
        }
        return enabledMetrics;
    }
}