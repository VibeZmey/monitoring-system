using MediatR;

namespace MonitoringSystem.Application.UseCases.MetricSettings.SetMetricSettings;

public class SetMetricSettingsCommand : IRequest<Unit>
{
    public string MetricName {get;init;}
    public bool IsEnabled {get;set;}
    public int DisplayOrder {get;set;}
}