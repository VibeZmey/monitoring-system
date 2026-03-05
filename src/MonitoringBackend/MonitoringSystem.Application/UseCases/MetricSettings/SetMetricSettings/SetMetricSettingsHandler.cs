using MediatR;
using Microsoft.EntityFrameworkCore;
using MonitoringSystem.Application.Interfaces;
using MonitoringSystem.Domain.Models;

namespace MonitoringSystem.Application.UseCases.MetricSettings.SetMetricSettings;

public class SetMetricSettingsHandler(IAppDbContext context)
    : IRequestHandler<SetMetricSettingsCommand, Unit>
{
    public async Task<Unit> Handle(SetMetricSettingsCommand request, CancellationToken cancellationToken)
    {
        MetricSetting setting = new MetricSetting()
        {
            MetricName = request.MetricName,
            IsEnabled = request.IsEnabled,
            DisplayOrder = request.DisplayOrder,
            CreatedAt = DateTime.UtcNow,
        };
        await context.MetricSettings.AddAsync(setting, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}