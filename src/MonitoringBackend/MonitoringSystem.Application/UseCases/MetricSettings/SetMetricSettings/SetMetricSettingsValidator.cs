using FluentValidation;
using MonitoringSystem.Domain.Models;
namespace MonitoringSystem.Application.UseCases.MetricSettings.SetMetricSettings;

public class SetMetricSettingsValidator : AbstractValidator<SetMetricSettingsCommand>
{
    public SetMetricSettingsValidator()
    {
        RuleFor(s => s.MetricName)
            .NotEmpty()
            .Must(name => Enum.TryParse<MetricName>(name, out _))
            .WithMessage("Invalid metric name");
        
        RuleFor(s => s.IsEnabled)
            .NotEmpty();
        
        RuleFor(s => s.DisplayOrder)
            .GreaterThan(-1);
    } 
}